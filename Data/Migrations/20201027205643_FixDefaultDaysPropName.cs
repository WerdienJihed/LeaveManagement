using Microsoft.EntityFrameworkCore.Migrations;

namespace Leave_management.Data.Migrations
{
    public partial class FixDefaultDaysPropName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultsDays",
                table: "LeaveTypes");

            migrationBuilder.AddColumn<int>(
                name: "DefaultDays",
                table: "LeaveTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultDays",
                table: "DetailsLeaveTypeViewModel",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultDays",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "DefaultDays",
                table: "DetailsLeaveTypeViewModel");

            migrationBuilder.AddColumn<int>(
                name: "DefaultsDays",
                table: "LeaveTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
