using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedDetailsToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorFirstName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorLastName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientFirstName",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "PatientLastName",
                table: "Appointments",
                newName: "Details");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Appointments",
                newName: "PatientLastName");

            migrationBuilder.AddColumn<string>(
                name: "DoctorFirstName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorLastName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientFirstName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
