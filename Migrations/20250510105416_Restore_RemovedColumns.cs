using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class Restore_RemovedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
    name: "ChiSoDienNuocMaChiSo",
    table: "HoaDon",
    type: "int",
    nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KhachThueMaKhachThue",
                table: "HoaDon",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KhachThueMaKhachThue",
                table: "HopDong",
                type: "int",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
