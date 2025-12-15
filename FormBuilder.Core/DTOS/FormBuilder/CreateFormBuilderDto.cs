using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    public class CreateFormBuilderDto
    {
        [Required]
        [StringLength(200)]
        public string FormName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FormCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublished { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // optional: will be set from authenticated user in controller
        public string? CreatedByUserId { get; set; }
    }
}