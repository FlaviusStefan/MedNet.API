using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNet.API.Migrations
{
    /// <inheritdoc />
    public partial class OnDeleteRestToCascQual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update Doctor -> Qualification relationship to Cascade
            migrationBuilder.DropForeignKey(
                name: "FK_Qualifications_Doctors_DoctorId",
                table: "Qualifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualifications_Doctors_DoctorId",
                table: "Qualifications",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Doctor -> Qualification relationship to Restrict
            migrationBuilder.DropForeignKey(
                name: "FK_Qualifications_Doctors_DoctorId",
                table: "Qualifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualifications_Doctors_DoctorId",
                table: "Qualifications",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
