using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLiKhoaHocApi.Entities
{
    [Table("Assignment")]
    public partial class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int ClassId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation Property
        public virtual Class? Class { get; set; }

        [NotMapped]
        public int SubmissionCount { get; set; } // Để chứa số lượng bài đã nộp

        [NotMapped]
        public int TotalStudents { get; set; }   // Để chứa sĩ số lớp (Capacity)

        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        [NotMapped]
        public string? ClassCode { get; set; }   // Để chứa mã lớp (L01, L02...)
    }
}