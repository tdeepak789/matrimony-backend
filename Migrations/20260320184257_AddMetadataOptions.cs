using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matrimony_api.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadata_options",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_metadata_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_metadata_options_metadata_options_parent_id",
                        column: x => x.parent_id,
                        principalTable: "metadata_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_metadata_options_category_name_parent_id",
                table: "metadata_options",
                columns: new[] { "category", "name", "parent_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_metadata_options_parent_id",
                table: "metadata_options",
                column: "parent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadata_options");
        }
    }
}
