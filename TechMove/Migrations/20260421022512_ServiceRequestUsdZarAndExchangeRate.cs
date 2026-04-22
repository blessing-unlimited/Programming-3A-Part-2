using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMove.Migrations
{
    /// <inheritdoc />
    public partial class ServiceRequestUsdZarAndExchangeRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "ServiceRequests",
                newName: "CostZar");

            migrationBuilder.AddColumn<decimal>(
                name: "CostUsd",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateFetchAtUtc",
                table: "ServiceRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsdToZar",
                table: "ServiceRequests",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostUsd",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ExchangeRateFetchAtUtc",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsdToZar",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "CostZar",
                table: "ServiceRequests",
                newName: "Cost");
        }
    }
}
