using System.ComponentModel.DataAnnotations;

namespace FormBuilder.API.Models
{
    public class FieldDataSourceDto
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public string SourceType { get; set; }
        public string ApiUrl { get; set; }
        public string HttpMethod { get; set; }
        public string RequestBodyJson { get; set; }
        public string ValuePath { get; set; }
        public string TextPath { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateFieldDataSourceDto
    {
        [Required]
        public int FieldId { get; set; }

        [Required, StringLength(50)]
        public string SourceType { get; set; }

        [StringLength(500)]
        public string ApiUrl { get; set; }

        [StringLength(10)]
        public string HttpMethod { get; set; }

        public string RequestBodyJson { get; set; }

        [StringLength(200)]
        public string ValuePath { get; set; }

        [StringLength(200)]
        public string TextPath { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateFieldDataSourceDto
    {
        [Required, StringLength(50)]
        public string SourceType { get; set; }

        [StringLength(500)]
        public string ApiUrl { get; set; }

        [StringLength(10)]
        public string HttpMethod { get; set; }

        public string RequestBodyJson { get; set; }

        [StringLength(200)]
        public string ValuePath { get; set; }

        [StringLength(200)]
        public string TextPath { get; set; }

        public bool IsActive { get; set; }
    }
}