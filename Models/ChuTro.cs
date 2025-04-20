using DACS_QuanLyPhongTro.Models;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("ChuTro")]
    public class ChuTro
    {
        [Key]
        public int MaChuTro { get; set; }

        [Required]
        public string HoTen { get; set; }

        [Required, StringLength(12)]
        public string CCCD { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string SoDienThoai { get; set; }

        public ICollection<ToaNha> ToaNhas { get; set; } = new List<ToaNha>();
        public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
    }
}