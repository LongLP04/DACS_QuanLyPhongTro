using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    public class ChiSoDienNuoc
    {
        [Key]
        public int MaChiSo { get; set; }

        [Required]
        public DateTime Thang { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal ChiSoDienCu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal ChiSoDienMoi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal ChiSoNuocCu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal ChiSoNuocMoi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoDienTieuThu => ChiSoDienMoi - ChiSoDienCu; // Tính toán tự động

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoNuocTieuThu => ChiSoNuocMoi - ChiSoNuocCu; // Tính toán tự động

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal DonGiaDien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal DonGiaNuoc { get; set; }

        [Required]
        public DateTime NgayGhi { get; set; }

        // Khóa ngoại liên kết với Phòng Trọ
        [ForeignKey("PhongTro")]
        [Required]
        public int MaPhong { get; set; }

        public PhongTro PhongTro { get; set; } = null!;
    }
}
