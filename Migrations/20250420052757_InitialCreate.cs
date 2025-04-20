using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuTro",
                columns: table => new
                {
                    MaChuTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuTro", x => x.MaChuTro);
                });

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaDichVu = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "PhuongThucThanhToan",
                columns: table => new
                {
                    MaPhuongThuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhuongThuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongThucThanhToan", x => x.MaPhuongThuc);
                });

            migrationBuilder.CreateTable(
                name: "ToaNha",
                columns: table => new
                {
                    MaToaNha = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenToaNha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSoTang = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChuTro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNha", x => x.MaToaNha);
                    table.ForeignKey(
                        name: "FK_ToaNha_ChuTro_MaChuTro",
                        column: x => x.MaChuTro,
                        principalTable: "ChuTro",
                        principalColumn: "MaChuTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhongTro",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    DienTich = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    GiaThue = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaToaNha = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongTro", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_PhongTro_ToaNha_MaToaNha",
                        column: x => x.MaToaNha,
                        principalTable: "ToaNha",
                        principalColumn: "MaToaNha",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoDienNuoc",
                columns: table => new
                {
                    MaChiSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    ChiSoDienCu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChiSoDienMoi = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChiSoNuocCu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChiSoNuocMoi = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SoDienTieuThu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SoNuocTieuThu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DonGiaDien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoDienNuoc", x => x.MaChiSo);
                    table.ForeignKey(
                        name: "FK_ChiSoDienNuoc_PhongTro_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTro",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhachThue",
                columns: table => new
                {
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTenKhachThue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachThue", x => x.MaKhachThue);
                    table.ForeignKey(
                        name: "FK_KhachThue_PhongTro_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTro",
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
                    TienDien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TienNuoc = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TienPhong = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TienDichVu = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    HanThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChiSo = table.Column<int>(type: "int", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    ChiSoDienNuocMaChiSo = table.Column<int>(type: "int", nullable: true),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDons_ChiSoDienNuoc_ChiSoDienNuocMaChiSo",
                        column: x => x.ChiSoDienNuocMaChiSo,
                        principalTable: "ChiSoDienNuoc",
                        principalColumn: "MaChiSo");
                    table.ForeignKey(
                        name: "FK_HoaDons_ChiSoDienNuoc_MaChiSo",
                        column: x => x.MaChiSo,
                        principalTable: "ChiSoDienNuoc",
                        principalColumn: "MaChiSo");
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachThue_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                });

            migrationBuilder.CreateTable(
                name: "HopDong",
                columns: table => new
                {
                    MaHopDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    NoiDungHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    MaChuTro = table.Column<int>(type: "int", nullable: false),
                    ChuTroMaChuTro = table.Column<int>(type: "int", nullable: true),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDong", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HopDong_ChuTro_ChuTroMaChuTro",
                        column: x => x.ChuTroMaChuTro,
                        principalTable: "ChuTro",
                        principalColumn: "MaChuTro");
                    table.ForeignKey(
                        name: "FK_HopDong_ChuTro_MaChuTro",
                        column: x => x.MaChuTro,
                        principalTable: "ChuTro",
                        principalColumn: "MaChuTro");
                    table.ForeignKey(
                        name: "FK_HopDong_KhachThue_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HopDong_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                });

            migrationBuilder.CreateTable(
                name: "PhieuDangKyDichVu",
                columns: table => new
                {
                    MaDangKyDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuDangKyDichVu", x => x.MaDangKyDichVu);
                    table.ForeignKey(
                        name: "FK_PhieuDangKyDichVu_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuGhiNhanSuCo",
                columns: table => new
                {
                    MaPhieuSuCo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayGhiNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoTaSuCo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HienTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuGhiNhanSuCo", x => x.MaPhieuSuCo);
                    table.ForeignKey(
                        name: "FK_PhieuGhiNhanSuCo_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuHienTrangNhanPhong",
                columns: table => new
                {
                    MaPhieuHienTrang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayNhanPhong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuHienTrangNhanPhong", x => x.MaPhieuHienTrang);
                    table.ForeignKey(
                        name: "FK_PhieuHienTrangNhanPhong_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDonDichVu",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaChiTiet = table.Column<int>(type: "int", nullable: false),
                    HoaDonMaHoaDon = table.Column<int>(type: "int", nullable: false),
                    DichVuMaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuongPhieuDangKy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHoaDonDichVu", x => new { x.MaHoaDon, x.MaDichVu });
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDonDichVu_DichVu_DichVuMaDichVu",
                        column: x => x.DichVuMaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDonDichVu_HoaDons_HoaDonMaHoaDon",
                        column: x => x.HoaDonMaHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThanhToan",
                columns: table => new
                {
                    MaPhieuThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTienThanhToan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaPhuongThuc = table.Column<int>(type: "int", nullable: false),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThanhToan", x => x.MaPhieuThanhToan);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_HoaDons_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_KhachThue_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_PhuongThucThanhToan_MaPhuongThuc",
                        column: x => x.MaPhuongThuc,
                        principalTable: "PhuongThucThanhToan",
                        principalColumn: "MaPhuongThuc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuDangKyDichVu",
                columns: table => new
                {
                    MaDangKyDichVu = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaChiTiet = table.Column<int>(type: "int", nullable: false),
                    PhieuDangKyDichVuMaDangKyDichVu = table.Column<int>(type: "int", nullable: false),
                    DichVuMaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuongDichVu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuDangKyDichVu", x => new { x.MaDangKyDichVu, x.MaDichVu });
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuDangKyDichVu_DichVu_DichVuMaDichVu",
                        column: x => x.DichVuMaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuDangKyDichVu_PhieuDangKyDichVu_PhieuDangKyDichVuMaDangKyDichVu",
                        column: x => x.PhieuDangKyDichVuMaDangKyDichVu,
                        principalTable: "PhieuDangKyDichVu",
                        principalColumn: "MaDangKyDichVu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HienTrangVatDung",
                columns: table => new
                {
                    MaVatDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVatDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhieuHienTrang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HienTrangVatDung", x => x.MaVatDung);
                    table.ForeignKey(
                        name: "FK_HienTrangVatDung_PhieuHienTrangNhanPhong_MaPhieuHienTrang",
                        column: x => x.MaPhieuHienTrang,
                        principalTable: "PhieuHienTrangNhanPhong",
                        principalColumn: "MaPhieuHienTrang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoDienNuoc_MaPhong",
                table: "ChiSoDienNuoc",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDonDichVu_DichVuMaDichVu",
                table: "ChiTietHoaDonDichVu",
                column: "DichVuMaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDonDichVu_HoaDonMaHoaDon",
                table: "ChiTietHoaDonDichVu",
                column: "HoaDonMaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuDangKyDichVu_DichVuMaDichVu",
                table: "ChiTietPhieuDangKyDichVu",
                column: "DichVuMaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuDangKyDichVu_PhieuDangKyDichVuMaDangKyDichVu",
                table: "ChiTietPhieuDangKyDichVu",
                column: "PhieuDangKyDichVuMaDangKyDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_HienTrangVatDung_MaPhieuHienTrang",
                table: "HienTrangVatDung",
                column: "MaPhieuHienTrang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_ChiSoDienNuocMaChiSo",
                table: "HoaDons",
                column: "ChiSoDienNuocMaChiSo");

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
                name: "IX_HopDong_ChuTroMaChuTro",
                table: "HopDong",
                column: "ChuTroMaChuTro");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_KhachThueMaKhachThue",
                table: "HopDong",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaChuTro",
                table: "HopDong",
                column: "MaChuTro");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaKhachThue",
                table: "HopDong",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_KhachThue_MaPhong",
                table: "KhachThue",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDangKyDichVu_MaKhachThue",
                table: "PhieuDangKyDichVu",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGhiNhanSuCo_MaKhachThue",
                table: "PhieuGhiNhanSuCo",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuHienTrangNhanPhong_MaKhachThue",
                table: "PhieuHienTrangNhanPhong",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_KhachThueMaKhachThue",
                table: "PhieuThanhToan",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_MaHoaDon",
                table: "PhieuThanhToan",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_MaPhuongThuc",
                table: "PhieuThanhToan",
                column: "MaPhuongThuc");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTro_MaToaNha",
                table: "PhongTro",
                column: "MaToaNha");

            migrationBuilder.CreateIndex(
                name: "IX_ToaNha_MaChuTro",
                table: "ToaNha",
                column: "MaChuTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietHoaDonDichVu");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuDangKyDichVu");

            migrationBuilder.DropTable(
                name: "HienTrangVatDung");

            migrationBuilder.DropTable(
                name: "HopDong");

            migrationBuilder.DropTable(
                name: "PhieuGhiNhanSuCo");

            migrationBuilder.DropTable(
                name: "PhieuThanhToan");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "PhieuDangKyDichVu");

            migrationBuilder.DropTable(
                name: "PhieuHienTrangNhanPhong");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "PhuongThucThanhToan");

            migrationBuilder.DropTable(
                name: "ChiSoDienNuoc");

            migrationBuilder.DropTable(
                name: "KhachThue");

            migrationBuilder.DropTable(
                name: "PhongTro");

            migrationBuilder.DropTable(
                name: "ToaNha");

            migrationBuilder.DropTable(
                name: "ChuTro");
        }
    }
}
