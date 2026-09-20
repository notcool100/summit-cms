using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummitCms.Modules.SiteContent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPageVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "published_at",
                schema: "content",
                table: "pages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "published_version_id",
                schema: "content",
                table: "pages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "page_versions",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    meta_description = table.Column<string>(type: "text", nullable: false),
                    hero_heading = table.Column<string>(type: "text", nullable: false),
                    hero_subheading = table.Column<string>(type: "text", nullable: false),
                    hero_media_id = table.Column<Guid>(type: "uuid", nullable: true),
                    secondary_media_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_page_versions", x => x.id);
                    table.ForeignKey(
                        name: "fk_page_versions_pages_page_id",
                        column: x => x.page_id,
                        principalSchema: "content",
                        principalTable: "pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pages_published_version_id",
                schema: "content",
                table: "pages",
                column: "published_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_page_versions_page_id_version_number",
                schema: "content",
                table: "page_versions",
                columns: new[] { "page_id", "version_number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_pages_page_versions_published_version_id",
                schema: "content",
                table: "pages",
                column: "published_version_id",
                principalSchema: "content",
                principalTable: "page_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pages_page_versions_published_version_id",
                schema: "content",
                table: "pages");

            migrationBuilder.DropTable(
                name: "page_versions",
                schema: "content");

            migrationBuilder.DropIndex(
                name: "ix_pages_published_version_id",
                schema: "content",
                table: "pages");

            migrationBuilder.DropColumn(
                name: "published_at",
                schema: "content",
                table: "pages");

            migrationBuilder.DropColumn(
                name: "published_version_id",
                schema: "content",
                table: "pages");
        }
    }
}
