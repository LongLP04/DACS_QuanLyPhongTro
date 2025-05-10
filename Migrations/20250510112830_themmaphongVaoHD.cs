using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class themmaphongVaoHD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaPhong",
                table: "HoaDon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaPhong",
                table: "HoaDon",
                column: "MaPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_PhongTro_MaPhong",
                table: "HoaDon",
                column: "MaPhong",
                principalTable: "PhongTro",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_PhongTro_MaPhong",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_MaPhong",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "MaPhong",
                table: "HoaDon");
        }
    }
}
