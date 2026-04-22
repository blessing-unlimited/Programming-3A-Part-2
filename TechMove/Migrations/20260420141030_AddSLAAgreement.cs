using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMove.Migrations
{
    /// <inheritdoc />
    public partial class AddSLAAgreement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedToContract",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "LinkedToClient",
                table: "Contracts");

            migrationBuilder.CreateTable(
                name: "SLAAgreements",
                columns: table => new
                {
                    SLAAgreementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    SLAName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseTimeHours = table.Column<int>(type: "int", nullable: false),
                    ResolutionPeriod = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLAAgreements", x => x.SLAAgreementId);
                    table.ForeignKey(
                        name: "FK_SLAAgreements_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SLAAgreements_ContractId",
                table: "SLAAgreements",
                column: "ContractId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SLAAgreements");

            migrationBuilder.AddColumn<string>(
                name: "LinkedToContract",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedToClient",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
