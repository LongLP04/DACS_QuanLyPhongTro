using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int MaKhachThue { get; set; }

        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }
    }
}