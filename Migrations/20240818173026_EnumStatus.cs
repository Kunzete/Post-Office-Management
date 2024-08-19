using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Post_Office_Management.Migrations
{
    /// <inheritdoc />
    public partial class EnumStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing data to match new data type
            migrationBuilder.Sql("UPDATE Deliverables SET Status = '1' WHERE Status = 'Posted'");
            migrationBuilder.Sql("UPDATE Deliverables SET Status = '2' WHERE Status = 'InTransit'");
            migrationBuilder.Sql("UPDATE Deliverables SET Status = '3' WHERE Status = 'Delivered'");
            // Add other updates as needed

            // Alter column to int type
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Deliverables",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Alter column back to string type
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Deliverables",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Update existing data to match old data type
            migrationBuilder.Sql("UPDATE Deliverables SET Status = 'Posted' WHERE Status = '1'");
            migrationBuilder.Sql("UPDATE Deliverables SET Status = 'InTransit' WHERE Status = '2'");
            migrationBuilder.Sql("UPDATE Deliverables SET Status = 'Delivered' WHERE Status = '3'");
            // Add other updates as needed
        }
    }
}