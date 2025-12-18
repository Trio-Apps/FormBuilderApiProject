using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    public class UpdateFormBuilderDto
    {
        [Required]
        [StringLength(200)]
        public string FormName { get; set; }

        [StringLength(200)]
        public string? ForeignFormName { get; set; }

        [Required]
        [StringLength(100)]
        public string FormCode { get; set; }

        public string Description { get; set; }

        public string? ForeignDescription { get; set; }

        public bool? IsPublished { get; set; }

        public bool? IsActive { get; set; }
    }
}