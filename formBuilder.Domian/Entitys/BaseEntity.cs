using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formBuilder.Domian.Entitys
{
    public class BaseEntity
    {
        public int id { get; set; }
        // يمكنك إضافة حقول التدقيق المشتركة هنا
        // ✅ هذه يجب أن تكون الأسماء الصحيحة المطابقة لقاعدة البيانات
        [StringLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
