using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _09082026_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegressionTailSetEntities",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    PortfolioName = table.Column<string>(type: "text", nullable: false),
                    TickerFirst = table.Column<string>(type: "text", nullable: false),
                    TickerSecond = table.Column<string>(type: "text", nullable: false),
                    Tails = table.Column<string>(type: "text", nullable: false),
                    Slope = table.Column<double>(type: "double precision", nullable: false),
                    Intercept = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegressionTailSetEntities", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegressionTailSetEntities",
                schema: "public");
        }
    }
}
