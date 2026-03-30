using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnergyUtilityApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "all_postcode_dnos",
                columns: table => new
                {
                    postcode = table.Column<string>(type: "character(8)", fixedLength: true, maxLength: 8, nullable: false),
                    dno_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("postcodes_with_dno_pkey", x => x.postcode);
                });

            migrationBuilder.CreateTable(
                name: "dno",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    region_code = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true),
                    region = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    @operator = table.Column<string>(name: "operator", type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dno_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dno_price_cap_rates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    meter_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    annual_standing_charge = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    unit_rate_pence = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    dno_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("regional_price_cap_rates_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "elec_cons_postcodes",
                columns: table => new
                {
                    postcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    num_meters = table.Column<int>(type: "integer", nullable: true),
                    mean_cons = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    median_cons = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("postcode_cons_pkey", x => x.postcode);
                });

            migrationBuilder.CreateTable(
                name: "weight_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("weight_categories_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dno_need_regions",
                columns: table => new
                {
                    dno_id = table.Column<int>(type: "integer", nullable: false),
                    need_region_source_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "dno_fk",
                        column: x => x.dno_id,
                        principalTable: "dno",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "regional_weights",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    region = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    value = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    multiplier = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: true),
                    region_id = table.Column<int>(type: "integer", nullable: true),
                    value_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("regional_weights_pkey", x => x.id);
                    table.ForeignKey(
                        name: "category_fk",
                        column: x => x.category_id,
                        principalTable: "weight_categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_dno_need_regions_dno_id",
                table: "dno_need_regions",
                column: "dno_id");

            migrationBuilder.CreateIndex(
                name: "idx_postcode_lookup",
                table: "elec_cons_postcodes",
                column: "postcode");

            migrationBuilder.CreateIndex(
                name: "IX_regional_weights_category_id",
                table: "regional_weights",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "all_postcode_dnos");

            migrationBuilder.DropTable(
                name: "dno_need_regions");

            migrationBuilder.DropTable(
                name: "dno_price_cap_rates");

            migrationBuilder.DropTable(
                name: "elec_cons_postcodes");

            migrationBuilder.DropTable(
                name: "regional_weights");

            migrationBuilder.DropTable(
                name: "dno");

            migrationBuilder.DropTable(
                name: "weight_categories");
        }
    }
}
