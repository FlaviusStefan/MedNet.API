using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedNet.API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertGenderToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add a temporary column to hold integer values
            migrationBuilder.AddColumn<int>(
                name: "GenderTemp",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenderTemp",
                table: "Doctors",
                type: "int",
                nullable: true);

            // Step 2: Convert existing string data to integer enum values
            // Male -> 0, Female -> 1
            migrationBuilder.Sql(@"
                UPDATE [Patients] 
                SET [GenderTemp] = CASE 
                    WHEN [Gender] = 'Male' THEN 0 
                    WHEN [Gender] = 'Female' THEN 1 
                    ELSE 0 
                END
            ");

            migrationBuilder.Sql(@"
                UPDATE [Doctors] 
                SET [GenderTemp] = CASE 
                    WHEN [Gender] = 'Male' THEN 0 
                    WHEN [Gender] = 'Female' THEN 1 
                    ELSE 0 
                END
            ");

            // Step 3: Drop the old Gender column
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Doctors");

            // Step 4: Rename GenderTemp to Gender
            migrationBuilder.RenameColumn(
                name: "GenderTemp",
                table: "Patients",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "GenderTemp",
                table: "Doctors",
                newName: "Gender");

            // Step 5: Make the new Gender column NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Patients",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Doctors",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary string column
            migrationBuilder.AddColumn<string>(
                name: "GenderTemp",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenderTemp",
                table: "Doctors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            // Step 2: Convert integer back to string
            migrationBuilder.Sql(@"
                UPDATE [Patients] 
                SET [GenderTemp] = CASE 
                    WHEN [Gender] = 0 THEN 'Male' 
                    WHEN [Gender] = 1 THEN 'Female' 
                    ELSE 'Male' 
                END
            ");

            migrationBuilder.Sql(@"
                UPDATE [Doctors] 
                SET [GenderTemp] = CASE 
                    WHEN [Gender] = 0 THEN 'Male' 
                    WHEN [Gender] = 1 THEN 'Female' 
                    ELSE 'Male' 
                END
            ");

            // Step 3: Drop integer column
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Doctors");

            // Step 4: Rename back
            migrationBuilder.RenameColumn(
                name: "GenderTemp",
                table: "Patients",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "GenderTemp",
                table: "Doctors",
                newName: "Gender");

            // Step 5: Make NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Doctors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
