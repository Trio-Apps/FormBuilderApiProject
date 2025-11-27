using formBuilder.Domian.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Entitys.FromBuilder
{

    [Table("ADOBE_SIGNATURE_CONFIG")]
    public class ADOBE_SIGNATURE_CONFIG : BaseEntity
    {
        

        [ForeignKey("DOCUMENT_TYPES")]
        public int DocumentTypeId { get; set; }
        public virtual DOCUMENT_TYPES DOCUMENT_TYPES { get; set; }

        public bool Enabled { get; set; }
        public string ConfigJson { get; set; }
    }
}