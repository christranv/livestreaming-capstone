using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Event.API.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    LogoImageFilePath = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    CategoryId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    EventId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    StreamSessionId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubEventFollower",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    UserId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    SubEventId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubEventFollower", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubEventFollower_SubEvent_SubEventId",
                        column: x => x.SubEventId,
                        principalTable: "SubEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubEvent_EventId",
                table: "SubEvent",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SubEventFollower_SubEventId",
                table: "SubEventFollower",
                column: "SubEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubEventFollower");

            migrationBuilder.DropTable(
                name: "SubEvent");

            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}
