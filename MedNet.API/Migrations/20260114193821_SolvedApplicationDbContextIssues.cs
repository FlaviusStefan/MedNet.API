using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNet.API.Migrations
{
    /// <inheritdoc />
    public partial class SolvedApplicationDbContextIssues : Migration
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

            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Addresses_AddressId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Contacts_ContactId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Addresses_AddressId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Addresses_AddressId",
                table: "Hospitals",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Contacts_ContactId",
                table: "Hospitals",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Addresses_AddressId",
                table: "Patients",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Addresses_AddressId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Contacts_ContactId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Addresses_AddressId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Addresses_AddressId",
                table: "Hospitals",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Contacts_ContactId",
                table: "Hospitals",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Addresses_AddressId",
                table: "Patients",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
