using System;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.API.DTOs
{
    public class FormSubmissionGridRowDto
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public string SubmissionNumber { get; set; }
        public int GridId { get; set; }
        public string GridName { get; set; }
        public int RowIndex { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateFormSubmissionGridRowDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public int GridId { get; set; }

        [Range(0, int.MaxValue)]
        public int? RowIndex { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateFormSubmissionGridRowDto
    {
        public int? RowIndex { get; set; }
        public bool? IsActive { get; set; }
    }
}