using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingHours",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "HospitalInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "HospitalInfos");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "HospitalInfos");

            migrationBuilder.DropColumn(
                name: "OperatingHours",
                table: "HospitalInfos");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "HospitalInfos");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "HospitalInfos");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "HospitalInfos");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "HospitalInfos");
        }
    }
}
