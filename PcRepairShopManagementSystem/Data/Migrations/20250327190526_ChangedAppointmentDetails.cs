using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PcRepairShopManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAppointmentDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SoftwareTitle",
                table: "AppointmentDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftwareTitle",
                table: "AppointmentDetails");
        }
    }
}
