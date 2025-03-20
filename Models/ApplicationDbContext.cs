using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Định nghĩa DbSet cho các bảng
        public DbSet<ChiSoDienNuoc> ChiSoDienNuocs { get; set; }
        public DbSet<ChuTro> ChuTros { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<KhachThue> KhachThues { get; set; }
        public DbSet<PhieuDangKyDichVu> PhieuDangKyDichVus { get; set; }
        public DbSet<PhieuGhiNhanSuCo> PhieuGhiNhanSuCos { get; set; }
        public DbSet<PhieuThanhToan> PhieuThanhToans { get; set; }
        public DbSet<PhongTro> PhongTros { get; set; }
        public DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }
        public DbSet<ToaNha> ToaNhas { get; set; }
        public DbSet<ChiTietHoaDonDichVu> ChiTietHoaDonDichVus { get; set; }
        public DbSet<ChiTietPhieuDangKyDichVu> ChiTietPhieuDangKyDichVus { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChiTietHoaDonDichVu>()
                .HasKey(c => new { c.MaHoaDon, c.MaDichVu });  // Khóa chính kép
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChiTietPhieuDangKyDichVu>()
                .HasKey(c => new { c.MaDangKyDichVu, c.MaDichVu });
            modelBuilder.Entity<HoaDon>()
        .HasOne(h => h.ChiSoDienNuoc)
        .WithMany()
        .HasForeignKey(h => h.MaChiSo)
        .OnDelete(DeleteBehavior.NoAction); // Thay vì Cascade

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.KhachThue)
                .WithMany()
                .HasForeignKey(h => h.MaKhachThue)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
