using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class Remove_DuplicateColumns_HoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF EXISTS(SELECT 1 FROM sys.columns 
                  WHERE Name = N'ChiSoDienNuocMaChiSo' AND Object_ID = Object_ID(N'HoaDon'))
        BEGIN
            ALTER TABLE HoaDon DROP COLUMN ChiSoDienNuocMaChiSo
        END");

            migrationBuilder.Sql(@"
        IF EXISTS(SELECT 1 FROM sys.columns 
                  WHERE Name = N'KhachThueMaKhachThue' AND Object_ID = Object_ID(N'HoaDon'))
        BEGIN
            ALTER TABLE HoaDon DROP COLUMN KhachThueMaKhachThue
        END");

            migrationBuilder.Sql(@"
        IF EXISTS(SELECT 1 FROM sys.columns 
                  WHERE Name = N'KhachThueMaKhachThue' AND Object_ID = Object_ID(N'HopDong'))
        BEGIN
            ALTER TABLE HopDong DROP COLUMN KhachThueMaKhachThue
        END");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
