using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPOS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseRecurringFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "OrderItems",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Expenses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecurringFrequency",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurringFrequency",
                table: "Expenses");
        }
    }
}
