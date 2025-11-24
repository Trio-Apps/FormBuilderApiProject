namespace FormBuilder.Core.DTOS.FormTabs
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// DTO لإنشاء تبويب جديد (HTTP POST).
    /// </summary>
    public class CreateFormTabDto
    {
        [Required(ErrorMessage = "FormBuilder ID is required.")]
        public int FormBuilderId { get; set; }

        [Required]
        [StringLength(200)]
        public string TabName { get; set; }

        [StringLength(100)]
        public string TabCode { get; set; }

        [Required]
        public int TabOrder { get; set; } // يستخدم لترتيب التبويبات داخل النموذج
    }

    /// <summary>
    /// DTO لتحديث تبويب موجود (HTTP PUT).
    /// </summary>
    public class UpdateFormTabDto
    {
        [Required]
        [StringLength(200)]
        public string TabName { get; set; }

        [StringLength(100)]
        public string TabCode { get; set; }

        [Required]
        public int TabOrder { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO لقراءة البيانات وعرضها في الاستجابة (HTTP GET).
    /// </summary>
    public class FormTabDto
    {
        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public string TabName { get; set; }
        public string TabCode { get; set; }
        public int TabOrder { get; set; }
        public bool IsActive { get; set; }

        // حقول التدقيق (Audit Fields)
        public string CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}