using DACS_QuanLyPhongTro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("ToaNha")]
    public class ToaNha
    {
        [Key]
        public int MaToaNha { get; set; }

        [Required]
        public string TenToaNha { get; set; }
        public string DiaChi { get; set; }
        public int TongSoTang { get; set; }
        public string MoTa { get; set; }

        [Required]
        public int MaChuTro { get; set; }

        [ForeignKey("MaChuTro")]
        public ChuTro ChuTro { get; set; }

        public ICollection<PhongTro> PhongTros { get; set; } = new List<PhongTro>();
        // Thêm trường lưu tọa độ (ví trí) dạng string "longitude,latitude"
        public string ViTri { get; set; }
    }
}