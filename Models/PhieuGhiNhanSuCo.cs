using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("PhieuGhiNhanSuCo")]
    public class PhieuGhiNhanSuCo
    {
        [Key]
        public int MaPhieuSuCo { get; set; }
        public DateTime NgayGhiNhan { get; set; }
        public string MoTaSuCo { get; set; }
        public string HienTrang { get; set; }
        public string GhiChu { get; set; }
        public string TinhTrang { get; set; } = "Chưa xử lý";
        [Required]
        public int MaKhachThue { get; set; }
        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }
    }

}
