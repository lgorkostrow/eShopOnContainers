using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class AddDiscountValueObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount_Amount",
                schema: "ordering",
                table: "orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discount_DiscountCode",
                schema: "ordering",
                table: "orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Discount_DiscountConfirmed",
                schema: "ordering",
                table: "orders",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount_Amount",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Discount_DiscountCode",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Discount_DiscountConfirmed",
                schema: "ordering",
                table: "orders");
        }
    }
}
