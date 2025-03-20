using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class khoitao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuTros",
                columns: table => new
                {
                    MaChuTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuTros", x => x.MaChuTro);
                });

            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaDichVu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "PhuongThucThanhToans",
                columns: table => new
                {
                    MaPhuongThuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhuongThuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongThucThanhToans", x => x.MaPhuongThuc);
                });

            migrationBuilder.CreateTable(
                name: "ToaNhas",
                columns: table => new
                {
                    MaToaNha = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenToaNha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSoTang = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaChuTro = table.Column<int>(type: "int", nullable: false),
                    ChuTroMaChuTro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNhas", x => x.MaToaNha);
                    table.ForeignKey(
                        name: "FK_ToaNhas_ChuTros_ChuTroMaChuTro",
                        column: x => x.ChuTroMaChuTro,
                        principalTable: "ChuTros",
                        principalColumn: "MaChuTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhongTros",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    DienTich = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaThue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaToaNha = table.Column<int>(type: "int", nullable: false),
                    ToaNhaMaToaNha = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongTros", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_PhongTros_ToaNhas_ToaNhaMaToaNha",
                        column: x => x.ToaNhaMaToaNha,
                        principalTable: "ToaNhas",
                        principalColumn: "MaToaNha",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoDienNuocs",
                columns: table => new
                {
                    MaChiSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Thang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChiSoDienCu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChiSoDienMoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChiSoNuocCu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChiSoNuocMoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonGiaDien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoDienNuocs", x => x.MaChiSo);
                    table.ForeignKey(
                        name: "FK_ChiSoDienNuocs_PhongTros_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTros",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhachThues",
                columns: table => new
                {
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    PhongTroMaPhong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachThues", x => x.MaKhachThue);
                    table.ForeignKey(
                        name: "FK_KhachThues_PhongTros_PhongTroMaPhong",
                        column: x => x.PhongTroMaPhong,
                        principalTable: "PhongTros",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienDien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienNuoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienDichVu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HanThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChiSo = table.Column<int>(type: "int", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDons_ChiSoDienNuocs_MaChiSo",
                        column: x => x.MaChiSo,
                        principalTable: "ChiSoDienNuocs",
                        principalColumn: "MaChiSo");
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachThues_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThues",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachThues_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThues",
                        principalColumn: "MaKhachThue");
                });

            migrationBuilder.CreateTable(
                name: "HopDongs",
                columns: table => new
                {
                    MaHopDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NoiDungHopDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDongs", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HopDongs_KhachThues_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThues",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuDangKyDichVus",
                columns: table => new
                {
                    MaDangKyDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuDangKyDichVus", x => x.MaDangKyDichVu);
                    table.ForeignKey(
                        name: "FK_PhieuDangKyDichVus_KhachThues_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThues",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuGhiNhanSuCos",
                columns: table => new
                {
                    MaSuCo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGiaiQuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuGhiNhanSuCos", x => x.MaSuCo);
                    table.ForeignKey(
                        name: "FK_PhieuGhiNhanSuCos_KhachThues_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThues",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDonDichVus",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaChiTiet = table.Column<int>(type: "int", nullable: false),
                    HoaDonMaHoaDon = table.Column<int>(type: "int", nullable: false),
                    DichVuMaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHoaDonDichVus", x => new { x.MaHoaDon, x.MaDichVu });
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDonDichVus_DichVus_DichVuMaDichVu",
                        column: x => x.DichVuMaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDonDichVus_HoaDons_HoaDonMaHoaDon",
                        column: x => x.HoaDonMaHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThanhToans",
                columns: table => new
                {
                    MaPhieuThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoTienThanhToan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    HoaDonMaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaPhuongThuc = table.Column<int>(type: "int", nullable: false),
                    PhuongThucThanhToanMaPhuongThuc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThanhToans", x => x.MaPhieuThanhToan);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToans_HoaDons_HoaDonMaHoaDon",
                        column: x => x.HoaDonMaHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToans_PhuongThucThanhToans_PhuongThucThanhToanMaPhuongThuc",
                        column: x => x.PhuongThucThanhToanMaPhuongThuc,
                        principalTable: "PhuongThucThanhToans",
                        principalColumn: "MaPhuongThuc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuDangKyDichVus",
                columns: table => new
                {
                    MaDangKyDichVu = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaChiTiet = table.Column<int>(type: "int", nullable: false),
                    PhieuDangKyDichVuMaDangKyDichVu = table.Column<int>(type: "int", nullable: false),
                    DichVuMaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuDangKyDichVus", x => new { x.MaDangKyDichVu, x.MaDichVu });
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuDangKyDichVus_DichVus_DichVuMaDichVu",
                        column: x => x.DichVuMaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuDangKyDichVus_PhieuDangKyDichVus_PhieuDangKyDichVuMaDangKyDichVu",
                        column: x => x.PhieuDangKyDichVuMaDangKyDichVu,
                        principalTable: "PhieuDangKyDichVus",
                        principalColumn: "MaDangKyDichVu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoDienNuocs_MaPhong",
                table: "ChiSoDienNuocs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDonDichVus_DichVuMaDichVu",
                table: "ChiTietHoaDonDichVus",
                column: "DichVuMaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDonDichVus_HoaDonMaHoaDon",
                table: "ChiTietHoaDonDichVus",
                column: "HoaDonMaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuDangKyDichVus_DichVuMaDichVu",
                table: "ChiTietPhieuDangKyDichVus",
                column: "DichVuMaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuDangKyDichVus_PhieuDangKyDichVuMaDangKyDichVu",
                table: "ChiTietPhieuDangKyDichVus",
                column: "PhieuDangKyDichVuMaDangKyDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_KhachThueMaKhachThue",
                table: "HoaDons",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaChiSo",
                table: "HoaDons",
                column: "MaChiSo");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaKhachThue",
                table: "HoaDons",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_KhachThueMaKhachThue",
                table: "HopDongs",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_KhachThues_PhongTroMaPhong",
                table: "KhachThues",
                column: "PhongTroMaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDangKyDichVus_KhachThueMaKhachThue",
                table: "PhieuDangKyDichVus",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGhiNhanSuCos_KhachThueMaKhachThue",
                table: "PhieuGhiNhanSuCos",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToans_HoaDonMaHoaDon",
                table: "PhieuThanhToans",
                column: "HoaDonMaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToans_PhuongThucThanhToanMaPhuongThuc",
                table: "PhieuThanhToans",
                column: "PhuongThucThanhToanMaPhuongThuc");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTros_ToaNhaMaToaNha",
                table: "PhongTros",
                column: "ToaNhaMaToaNha");

            migrationBuilder.CreateIndex(
                name: "IX_ToaNhas_ChuTroMaChuTro",
                table: "ToaNhas",
                column: "ChuTroMaChuTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietHoaDonDichVus");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuDangKyDichVus");

            migrationBuilder.DropTable(
                name: "HopDongs");

            migrationBuilder.DropTable(
                name: "PhieuGhiNhanSuCos");

            migrationBuilder.DropTable(
                name: "PhieuThanhToans");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "PhieuDangKyDichVus");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "PhuongThucThanhToans");

            migrationBuilder.DropTable(
                name: "ChiSoDienNuocs");

            migrationBuilder.DropTable(
                name: "KhachThues");

            migrationBuilder.DropTable(
                name: "PhongTros");

            migrationBuilder.DropTable(
                name: "ToaNhas");

            migrationBuilder.DropTable(
                name: "ChuTros");
        }
    }
}
