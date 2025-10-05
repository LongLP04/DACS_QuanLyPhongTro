using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } // ApplicationUserId

        public string ReceiverId { get; set; } // ApplicationUserId, null nếu là chat nhóm

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsGroup { get; set; } // true: chat chung, false: chat riêng
    }
}
