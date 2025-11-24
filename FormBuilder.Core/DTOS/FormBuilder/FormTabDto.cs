namespace FormBuilder.Core.DTOS.FormTabs
{
    using System.ComponentModel.DataAnnotations;
    using System;

    namespace FormBuilder.Core.DTOS.FormTabs
    {
        public class FormTabDto
        {
            public int Id { get; set; }
            public int FormBuilderId { get; set; }
            public string TabName { get; set; }
            public string TabCode { get; set; }
            public int TabOrder { get; set; }
            public bool IsActive { get; set; }
            public string CreatedByUserId { get; set; }
            public DateTime CreatedDate { get; set; }
         
        }
    }

namespace FormBuilder.Core.DTOS.FormTabs
    {
        public class UpdateFormTabDto
        {
      

            [Required]
            [StringLength(100)]
            public string TabName { get; set; }

            [Required]
            [StringLength(50)]
            public string TabCode { get; set; }

            public int TabOrder { get; set; }

            public bool IsActive { get; set; }
        }
    }

namespace FormBuilder.Core.DTOS.FormTabs
    {
        public class CreateFormTabDto
        {
            [Required]
            public int FormBuilderId { get; set; }

            [Required]
            [StringLength(100)]
            public string TabName { get; set; }

            [Required]
            [StringLength(50)]
            public string TabCode { get; set; }

            public int TabOrder { get; set; } = 0;

            public bool IsActive { get; set; } = true;
        }
    }
}