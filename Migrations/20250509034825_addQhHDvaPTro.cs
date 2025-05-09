using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class addQhHDvaPTro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaPhong",
                table: "HopDong",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaPhong",
                table: "HopDong",
                column: "MaPhong");

            migrationBuilder.AddForeignKey(
                name: "FK_HopDong_PhongTro_MaPhong",
                table: "HopDong",
                column: "MaPhong",
                principalTable: "PhongTro",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HopDong_PhongTro_MaPhong",
                table: "HopDong");

            migrationBuilder.DropIndex(
                name: "IX_HopDong_MaPhong",
                table: "HopDong");

            migrationBuilder.DropColumn(
                name: "MaPhong",
                table: "HopDong");
        }
    }
}
