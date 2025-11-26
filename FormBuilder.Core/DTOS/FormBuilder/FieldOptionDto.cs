using System.ComponentModel.DataAnnotations;

namespace FormBuilder.API.Models
{
    public class FieldOptionDto
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public string OptionText { get; set; }
        public string OptionValue { get; set; }
        public int OptionOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateFieldOptionDto
    {
        [Required]
        public int FieldId { get; set; }

        [Required, StringLength(200)]
        public string OptionText { get; set; }

        [Required, StringLength(200)]
        public string OptionValue { get; set; }

        [Required]
        public int OptionOrder { get; set; }

        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateFieldOptionDto
    {
        [Required, StringLength(200)]
        public string OptionText { get; set; }

        [Required, StringLength(200)]
        public string OptionValue { get; set; }

        [Required]
        public int OptionOrder { get; set; }

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}