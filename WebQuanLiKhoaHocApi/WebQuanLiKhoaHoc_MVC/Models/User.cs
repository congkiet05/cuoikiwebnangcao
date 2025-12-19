using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Models
{
    [Table("User")] // Khai báo chính xác tên bảng trong SQL
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // Đây là cột bạn đang thiếu định nghĩa

        public string? Email { get; set; }

        public byte RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với bảng Role
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        // Mối quan hệ 1-1 với Student (nếu cần)
        public virtual Student? Student { get; set; }
        
    }
}