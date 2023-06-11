﻿// <auto-generated />
using System;
using Event.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Event.API.Infrastructure.Migrations
{
    [DbContext(typeof(EventContext))]
    partial class EventContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Event.API.Models.Event", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LogoImageFilePath")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("Event.API.Models.SubEvent", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("EventId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("StreamSessionId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("SubEvent");
                });

            modelBuilder.Entity("Event.API.Models.SubEventFollower", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SubEventId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("SubEventId");

                    b.ToTable("SubEventFollower");
                });

            modelBuilder.Entity("Event.API.Models.SubEvent", b =>
                {
                    b.HasOne("Event.API.Models.Event", "Event")
                        .WithMany("SubEvents")
                        .HasForeignKey("EventId");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Event.API.Models.SubEventFollower", b =>
                {
                    b.HasOne("Event.API.Models.SubEvent", "SubEvent")
                        .WithMany("SubEventFollower")
                        .HasForeignKey("SubEventId");

                    b.Navigation("SubEvent");
                });

            modelBuilder.Entity("Event.API.Models.Event", b =>
                {
                    b.Navigation("SubEvents");
                });

            modelBuilder.Entity("Event.API.Models.SubEvent", b =>
                {
                    b.Navigation("SubEventFollower");
                });
#pragma warning restore 612, 618
        }
    }
}
