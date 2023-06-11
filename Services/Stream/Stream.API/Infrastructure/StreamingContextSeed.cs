using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Stream.Infrastructure;

namespace Stream.API.Infrastructure
{
    public class StreamContextSeed
    {
        public async Task SeedAsync(StreamContext context, IWebHostEnvironment env, IOptions<StreamSettings> settings,
            ILogger<StreamContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(StreamContextSeed));
            
            await policy.ExecuteAsync(async () =>
            {
                await using (context)
                {
                    await context.Database.MigrateAsync();
            
                    if (!context.Languages.Any())
                    {
                        await context.Languages.AddRangeAsync(GetPredefinedLanguage());
                    }
            
                    if (!context.StreamStatus.Any())
                    {
                        await context.StreamStatus.AddRangeAsync(GetPredefinedStreamStatus());
                    }
            
                    await context.SaveChangesAsync();
                }
            });
        }

        private static IEnumerable<Language> GetPredefinedLanguage()
        {
            return new List<Language>()
            {
                Language.Eng,
                Language.Vie,
            };
        }

        private static IEnumerable<StreamSessionStatus> GetPredefinedStreamStatus()
        {
            return new List<StreamSessionStatus>()
            {
                StreamSessionStatus.Created,
                StreamSessionStatus.Published,
                StreamSessionStatus.Banned,
                StreamSessionStatus.Finished,
            };
        }

        private AsyncRetryPolicy CreatePolicy(ILogger logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().WaitAndRetryAsync(
                retries,
                _ => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception,
                        "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
        }
    }
}