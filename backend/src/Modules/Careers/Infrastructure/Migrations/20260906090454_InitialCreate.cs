using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummitCms.Modules.Careers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "careers");

            migrationBuilder.CreateTable(
                name: "job_openings",
                schema: "careers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    department = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    employment_type = table.Column<int>(type: "integer", nullable: false),
                    track_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    apply_contact = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    posted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    closes_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_openings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_tracks",
                schema: "careers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    path_label = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cta_label = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_tracks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_track_tags",
                schema: "careers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_track_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_track_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_job_track_tags_job_tracks_job_track_id",
                        column: x => x.job_track_id,
                        principalSchema: "careers",
                        principalTable: "job_tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_job_openings_is_active",
                schema: "careers",
                table: "job_openings",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_job_track_tags_job_track_id",
                schema: "careers",
                table: "job_track_tags",
                column: "job_track_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_tracks_page_id",
                schema: "careers",
                table: "job_tracks",
                column: "page_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_openings",
                schema: "careers");

            migrationBuilder.DropTable(
                name: "job_track_tags",
                schema: "careers");

            migrationBuilder.DropTable(
                name: "job_tracks",
                schema: "careers");
        }
    }
}
