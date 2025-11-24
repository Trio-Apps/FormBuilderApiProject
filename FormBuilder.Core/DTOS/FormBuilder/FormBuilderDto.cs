using System;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    public class FormBuilderDto
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }

        // Auditing Fields
        public string CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}