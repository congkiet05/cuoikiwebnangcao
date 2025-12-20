using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLiKhoaHocApi.Entities
{
    [Table("Submission")]
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public double? Grade { get; set; }

        // Navigation properties (Nếu cần link dữ liệu)
        [ForeignKey("AssignmentId")]
        public virtual Assignment? Assignment { get; set; }
    }
}