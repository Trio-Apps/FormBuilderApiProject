using System;

namespace FormBuilder.API.DTOs
{
    public class FormGridDto
    {
        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public string FormBuilderName { get; set; } = string.Empty;
        public string GridName { get; set; } = string.Empty;
        public string GridCode { get; set; } = string.Empty;
        public int? TabId { get; set; }
        public string TabName { get; set; } = string.Empty;
        public int GridOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateFormGridDto
    {
        public int FormBuilderId { get; set; }
        public string GridName { get; set; } = string.Empty;
        public string GridCode { get; set; } = string.Empty;
        public int? TabId { get; set; }
        public int? GridOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateFormGridDto
    {
        public string GridName { get; set; } = string.Empty;
        public string GridCode { get; set; } = string.Empty;
        public int? TabId { get; set; }
        public int? GridOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}