using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delegates.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExecutePhase_4_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositPaymentMethod",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderPostponedCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPostponedCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPostponedCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPostponedCompanies_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPostponedCompanyProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderPostponedCompanyId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPostponedCompanyProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPostponedCompanyProducts_OrderPostponedCompanies_OrderPostponedCompanyId",
                        column: x => x.OrderPostponedCompanyId,
                        principalTable: "OrderPostponedCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPostponedCompanyProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPostponedCompanies_CompanyId",
                table: "OrderPostponedCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPostponedCompanies_OrderId",
                table: "OrderPostponedCompanies",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPostponedCompanyProducts_OrderPostponedCompanyId",
                table: "OrderPostponedCompanyProducts",
                column: "OrderPostponedCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPostponedCompanyProducts_ProductId",
                table: "OrderPostponedCompanyProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPostponedCompanyProducts");

            migrationBuilder.DropTable(
                name: "OrderPostponedCompanies");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DepositPaymentMethod",
                table: "Orders");
        }
    }
}
