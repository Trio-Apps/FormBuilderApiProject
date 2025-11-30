

using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    public class DocumentSeriesDto
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string SeriesCode { get; set; }
        public int NextNumber { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDocumentSeriesDto
    {
        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required, StringLength(50)]
        public string SeriesCode { get; set; }

        public int NextNumber { get; set; } = 1;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateDocumentSeriesDto
    {
        public int? DocumentTypeId { get; set; }
        public int? ProjectId { get; set; }
        [StringLength(50)]
        public string SeriesCode { get; set; }
        public int? NextNumber { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
    }

    public class DocumentSeriesNumberDto
    {
        public int SeriesId { get; set; }
        public string SeriesCode { get; set; }
        public int NextNumber { get; set; }
        public string FullNumber { get; set; }
    }
}