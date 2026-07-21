using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delegates.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditInPhase_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Warehouse",
                table: "Warehouse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitDestination",
                table: "VisitDestination");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitOfMeasure",
                table: "UnitOfMeasure");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategory",
                table: "ProductCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.RenameTable(
                name: "Warehouse",
                newName: "Warehouses");

            migrationBuilder.RenameTable(
                name: "VisitDestination",
                newName: "VisitDestinations");

            migrationBuilder.RenameTable(
                name: "UnitOfMeasure",
                newName: "UnitOfMeasures");

            migrationBuilder.RenameTable(
                name: "ProductCategory",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Warehouses",
                table: "Warehouses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitDestinations",
                table: "VisitDestinations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitOfMeasures",
                table: "UnitOfMeasures",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SecondaryPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Governorate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AccountId_Code",
                table: "Clients",
                columns: new[] { "AccountId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AccountId_PhoneNumber",
                table: "Clients",
                columns: new[] { "AccountId", "PhoneNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Warehouses",
                table: "Warehouses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitDestinations",
                table: "VisitDestinations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitOfMeasures",
                table: "UnitOfMeasures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.RenameTable(
                name: "Warehouses",
                newName: "Warehouse");

            migrationBuilder.RenameTable(
                name: "VisitDestinations",
                newName: "VisitDestination");

            migrationBuilder.RenameTable(
                name: "UnitOfMeasures",
                newName: "UnitOfMeasure");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "ProductCategory");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Warehouse",
                table: "Warehouse",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitDestination",
                table: "VisitDestination",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitOfMeasure",
                table: "UnitOfMeasure",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategory",
                table: "ProductCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");
        }
    }
}
