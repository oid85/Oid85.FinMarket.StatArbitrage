using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "CorrelationEntities",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    PortfolioName = table.Column<string>(type: "text", nullable: false),
                    TickerFirst = table.Column<string>(type: "text", nullable: false),
                    TickerSecond = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrelationEntities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ParameterEntities",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterEntities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StrategyExecuteResultEntities",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TickerFirst = table.Column<string>(type: "text", nullable: false),
                    TickerSecond = table.Column<string>(type: "text", nullable: false),
                    PortfolioName = table.Column<string>(type: "text", nullable: false),
                    ProcessName = table.Column<string>(type: "text", nullable: false),
                    StrategyDescription = table.Column<string>(type: "text", nullable: false),
                    StrategyName = table.Column<string>(type: "text", nullable: false),
                    StrategyParams = table.Column<string>(type: "text", nullable: false),
                    StrategyParamsHash = table.Column<string>(type: "text", nullable: false),
                    NumberPositions = table.Column<int>(type: "integer", nullable: false),
                    CurrentPositionFirst = table.Column<int>(type: "integer", nullable: false),
                    CurrentPositionSecond = table.Column<int>(type: "integer", nullable: false),
                    CurrentPositionCost = table.Column<double>(type: "double precision", nullable: false),
                    ProfitFactor = table.Column<double>(type: "double precision", nullable: false),
                    RecoveryFactor = table.Column<double>(type: "double precision", nullable: false),
                    NetProfit = table.Column<double>(type: "double precision", nullable: false),
                    AverageNetProfit = table.Column<double>(type: "double precision", nullable: false),
                    AverageNetProfitPercent = table.Column<double>(type: "double precision", nullable: false),
                    Drawdown = table.Column<double>(type: "double precision", nullable: false),
                    MaxDrawdown = table.Column<double>(type: "double precision", nullable: false),
                    MaxDrawdownPercent = table.Column<double>(type: "double precision", nullable: false),
                    WinningPositions = table.Column<int>(type: "integer", nullable: false),
                    WinningTradesPercent = table.Column<double>(type: "double precision", nullable: false),
                    StartMoney = table.Column<double>(type: "double precision", nullable: false),
                    EndMoney = table.Column<double>(type: "double precision", nullable: false),
                    TotalReturn = table.Column<double>(type: "double precision", nullable: false),
                    AnnualYieldReturn = table.Column<double>(type: "double precision", nullable: false),
                    ResultMessage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyExecuteResultEntities", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrelationEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ParameterEntities",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StrategyExecuteResultEntities",
                schema: "public");
        }
    }
}
