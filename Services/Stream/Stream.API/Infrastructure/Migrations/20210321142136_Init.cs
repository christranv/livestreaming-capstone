using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stream.API.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stream_session_category",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    CategoryGuid = table.Column<string>(type: "varchar(36) CHARACTER SET utf8mb4", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stream_session_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stream_session_status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stream_session_status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "streamer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    IdentityGuid = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: false),
                    AuthToken = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: false),
                    StreamKey = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streamer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stream_session",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    StreamerId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    StreamerName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    StreamerImageUrl = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    StreamSessionStatusId1 = table.Column<int>(type: "int", nullable: true),
                    StreamSessionStatusId = table.Column<int>(type: "int", nullable: false),
                    SubEventId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    Title = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: true),
                    Announcement = table.Column<string>(type: "varchar(200) CHARACTER SET utf8mb4", maxLength: 200, nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ThumbnailImage = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stream_session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stream_session_language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stream_session_stream_session_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "stream_session_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stream_session_stream_session_status_StreamSessionStatusId",
                        column: x => x.StreamSessionStatusId,
                        principalTable: "stream_session_status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stream_session_stream_session_status_StreamSessionStatusId1",
                        column: x => x.StreamSessionStatusId1,
                        principalTable: "stream_session_status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stream_session_streamer_StreamerId",
                        column: x => x.StreamerId,
                        principalTable: "streamer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stream_session_tag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    TagGuid = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    StreamSessionId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stream_session_tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stream_session_tag_stream_session_StreamSessionId",
                        column: x => x.StreamSessionId,
                        principalTable: "stream_session",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_CategoryId",
                table: "stream_session",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_LanguageId",
                table: "stream_session",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_StreamerId",
                table: "stream_session",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_StreamSessionStatusId",
                table: "stream_session",
                column: "StreamSessionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_StreamSessionStatusId1",
                table: "stream_session",
                column: "StreamSessionStatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_SubEventId",
                table: "stream_session",
                column: "SubEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stream_session_tag_StreamSessionId",
                table: "stream_session_tag",
                column: "StreamSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_streamer_IdentityGuid",
                table: "streamer",
                column: "IdentityGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_streamer_StreamKey",
                table: "streamer",
                column: "StreamKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "request");

            migrationBuilder.DropTable(
                name: "stream_session_tag");

            migrationBuilder.DropTable(
                name: "stream_session");

            migrationBuilder.DropTable(
                name: "language");

            migrationBuilder.DropTable(
                name: "stream_session_category");

            migrationBuilder.DropTable(
                name: "stream_session_status");

            migrationBuilder.DropTable(
                name: "streamer");
        }
    }
}
