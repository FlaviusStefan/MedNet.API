using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNet.API.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Addresses_AddressId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Addresses_AddressId",
                table: "Doctors",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Addresses_AddressId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Addresses_AddressId",
                table: "Doctors",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
