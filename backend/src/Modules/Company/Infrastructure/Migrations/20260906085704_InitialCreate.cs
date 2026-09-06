using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummitCms.Modules.Company.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "company");

            migrationBuilder.CreateTable(
                name: "awards",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    year = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_awards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "company_values",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_values", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "milestones",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    year = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_milestones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "narrative_blocks",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    eyebrow = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title_line1 = table.Column<string>(type: "text", nullable: false),
                    title_line2 = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: true),
                    image_caption = table.Column<string>(type: "text", nullable: false),
                    image_first = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_narrative_blocks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "office_locations",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    role_description = table.Column<string>(type: "text", nullable: false),
                    is_headquarters = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_office_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                schema: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_members", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_awards_page_id",
                schema: "company",
                table: "awards",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_values_page_id",
                schema: "company",
                table: "company_values",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "ix_milestones_page_id",
                schema: "company",
                table: "milestones",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "ix_narrative_blocks_page_id",
                schema: "company",
                table: "narrative_blocks",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "ix_office_locations_page_id",
                schema: "company",
                table: "office_locations",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_members_page_id",
                schema: "company",
                table: "team_members",
                column: "page_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "awards",
                schema: "company");

            migrationBuilder.DropTable(
                name: "company_values",
                schema: "company");

            migrationBuilder.DropTable(
                name: "milestones",
                schema: "company");

            migrationBuilder.DropTable(
                name: "narrative_blocks",
                schema: "company");

            migrationBuilder.DropTable(
                name: "office_locations",
                schema: "company");

            migrationBuilder.DropTable(
                name: "team_members",
                schema: "company");
        }
    }
}
