using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class themVIETMAP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ViTri",
                table: "ToaNha",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViTri",
                table: "ToaNha");
        }
    }
}
