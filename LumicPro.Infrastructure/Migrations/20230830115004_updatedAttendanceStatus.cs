using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LumicPro.Infrastructure.Migrations
{
    public partial class updatedAttendanceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AttendanceStatus",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AttendanceStatus",
                table: "AppUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
