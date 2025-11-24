using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formBuilder.Domian.Entitys
{
    public class BaseEntity
    {
        public int id { get; set; }
        // يمكنك إضافة حقول التدقيق المشتركة هنا
        public DateTime CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
