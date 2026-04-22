using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMove.Migrations
{
    /// <inheritdoc />
    public partial class ServiceRequestExchangeTimestampFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeRateFetchAtUtc",
                table: "ServiceRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateFetchedAtUtc",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeRateFetchedAtUtc",
                table: "ServiceRequests");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateFetchAtUtc",
                table: "ServiceRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
