using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    public class UpdateFormBuilderDto
    {
        [Required]
        [StringLength(200)]
        public string FormName { get; set; }

        [Required]
        [StringLength(100)]
        public string FormCode { get; set; }

        public string Description { get; set; }

        public bool? IsPublished { get; set; }

        public bool? IsActive { get; set; }
    }
}