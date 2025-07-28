using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNet.API.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIdentityUserIdToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Hospitals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_IdentityUserId",
                table: "Hospitals",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_IdentityUserId",
                table: "Doctors",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_IdentityUser_IdentityUserId",
                table: "Doctors",
                column: "IdentityUserId",
                principalTable: "IdentityUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_IdentityUser_IdentityUserId",
                table: "Hospitals",
                column: "IdentityUserId",
                principalTable: "IdentityUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_IdentityUser_IdentityUserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_IdentityUser_IdentityUserId",
                table: "Hospitals");

            migrationBuilder.DropIndex(
                name: "IX_Hospitals_IdentityUserId",
                table: "Hospitals");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_IdentityUserId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Doctors");
        }
    }
}
