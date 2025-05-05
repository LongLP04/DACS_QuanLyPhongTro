using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class XoaMaChuTroToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaChuTro",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaChuTro",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
