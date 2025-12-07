using System;
using System.Collections.Generic;

namespace FormBuilder.Application.DTOs.ApprovalWorkflow
{
    // DTO للـ Stage داخل الـ Workflow
   

    // DTO للـ Workflow لعمليات القراءة
    public class ApprovalWorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } // للعرض
        public bool IsActive { get; set; }
        public List<ApprovalStageDto> Stages { get; set; }
    }

    // DTO لإنشاء Workflow جديد
    public class ApprovalWorkflowCreateDto
    {
        public string Name { get; set; }
        public int DocumentTypeId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // DTO لتحديث Workflow موجود
    public class ApprovalWorkflowUpdateDto
    {
        public string Name { get; set; }
        public int? DocumentTypeId { get; set; }
        public bool? IsActive { get; set; }
    }
}
