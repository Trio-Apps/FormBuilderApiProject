using System;
using System.Collections.Generic;

namespace FormBuilder.Application.DTOs.ApprovalWorkflow
{
    // ==========================
    // DTO لعرض البيانات
    // ==========================
    public class ApprovalStageDto
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsFinalStage { get; set; }
        public bool IsActive { get; set; }

        // يمكن إضافة معلومات عن الـ Workflow المرتبط
        public string WorkflowName { get; set; }
    }

    // ==========================
    // DTO لإنشاء Stage جديد
    // ==========================
    public class ApprovalStageCreateDto
    {
        public int WorkflowId { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsFinalStage { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ==========================
    // DTO لتحديث Stage موجود
    // ==========================
    public class ApprovalStageUpdateDto
    {
        public int? WorkflowId { get; set; }
        public string StageName { get; set; }
        public int? StageOrder { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IsFinalStage { get; set; }
        public bool? IsActive { get; set; }
    }
}
