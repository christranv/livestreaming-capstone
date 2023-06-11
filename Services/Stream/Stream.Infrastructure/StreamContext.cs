using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Stream.Domain.Seedwork;
using Stream.Infrastructure.EntityConfigurations;

namespace Stream.Infrastructure
{
    public class StreamContext : DbContext, IUnitOfWork
    {
        public DbSet<StreamSession> StreamSessions { get; set; }
        public DbSet<StreamSessionStatus> StreamStatus { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<StreamSessionTag> StreamSessionTags { get; set; }
        public DbSet<StreamSessionCategory> StreamSessionCategories { get; set; }
        public DbSet<StreamSessionTag> Tags { get; set; }
        public DbSet<Streamer> Streamers { get; set; }

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public StreamContext(DbContextOptions<StreamContext> options) : base(options)
        {
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public StreamContext(DbContextOptions<StreamContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            System.Diagnostics.Debug.WriteLine("StreamingContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StreamSessionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StreamSessionTagEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StreamSessionCategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StreamSessionStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StreamerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event.API Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        private void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}