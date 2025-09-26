using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace cse325Team6Project.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_firstname = table.Column<string>(type: "text", nullable: false),
                    account_lastname = table.Column<string>(type: "text", nullable: false),
                    account_email = table.Column<string>(type: "text", nullable: false),
                    account_password = table.Column<string>(type: "text", nullable: false),
                    account_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.account_id);
                });

            migrationBuilder.CreateTable(
                name: "make",
                columns: table => new
                {
                    make_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    make_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_make", x => x.make_id);
                });

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    inv_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    inv_make = table.Column<string>(type: "text", nullable: false),
                    inv_model = table.Column<string>(type: "text", nullable: false),
                    inv_year = table.Column<string>(type: "text", nullable: false),
                    inv_description = table.Column<string>(type: "text", nullable: false),
                    inv_image = table.Column<string>(type: "text", nullable: false),
                    inv_thumbnail = table.Column<string>(type: "text", nullable: false),
                    inv_price = table.Column<decimal>(type: "numeric", nullable: false),
                    inv_miles = table.Column<int>(type: "integer", nullable: false),
                    inv_color = table.Column<string>(type: "text", nullable: false),
                    make_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory", x => x.inv_id);
                    table.ForeignKey(
                        name: "FK_inventory_make_make_id",
                        column: x => x.make_id,
                        principalTable: "make",
                        principalColumn: "make_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_make_id",
                table: "inventory",
                column: "make_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "inventory");

            migrationBuilder.DropTable(
                name: "make");
        }
    }
}
