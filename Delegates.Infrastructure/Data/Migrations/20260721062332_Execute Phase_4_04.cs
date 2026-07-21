using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delegates.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExecutePhase_4_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderPostponedCompanyProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderPostponedCompanyProducts");
        }
    }
}
