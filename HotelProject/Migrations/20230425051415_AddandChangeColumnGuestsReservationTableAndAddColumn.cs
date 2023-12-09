using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelProject.Migrations
{
    public partial class AddandChangeColumnGuestsReservationTableAndAddColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Guests",
                newName: "Executor");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Executor",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Guests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Executor",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Guests");

            migrationBuilder.RenameColumn(
                name: "Executor",
                table: "Guests",
                newName: "Number");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
