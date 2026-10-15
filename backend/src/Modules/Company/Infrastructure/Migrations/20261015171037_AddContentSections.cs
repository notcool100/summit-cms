using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummitCms.Modules.Company.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "content_sections",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    page_slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    slot = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    layout = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    eyebrow = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    heading = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    intro = table.Column<string>(type: "text", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    columns = table.Column<int>(type: "integer", nullable: false),
                    numbered = table.Column<bool>(type: "boolean", nullable: false),
                    tinted = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_sections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_items",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    section_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    subtitle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    detail = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    tags = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_items_content_sections_section_id",
                        column: x => x.section_id,
                        principalSchema: "company",
                        principalTable: "content_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_content_items_section_id",
                schema: "company",
                table: "content_items",
                column: "section_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_sections_key",
                schema: "company",
                table: "content_sections",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_sections_page_slug",
                schema: "company",
                table: "content_sections",
                column: "page_slug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_items",
                schema: "company");

            migrationBuilder.DropTable(
                name: "content_sections",
                schema: "company");
        }
    }
}
