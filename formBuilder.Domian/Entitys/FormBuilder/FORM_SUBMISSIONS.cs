using DocumentFormat.OpenXml.Wordprocessing;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("FORM_SUBMISSIONS")]
public class FORM_SUBMISSIONS
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("FORM_BUILDER")]
    public int FormBuilderId { get; set; }
    public virtual FORM_BUILDER FORM_BUILDER { get; set; }

    public int Version { get; set; }

    [ForeignKey("DOCUMENT_TYPES")]
    public int DocumentTypeId { get; set; }
    public virtual DOCUMENT_TYPES DOCUMENT_TYPES { get; set; }

    [ForeignKey("DOCUMENT_SERIES")]
    public int SeriesId { get; set; }
    public virtual DOCUMENT_SERIES DOCUMENT_SERIES { get; set; }

    [Required, StringLength(50)]
    public string DocumentNumber { get; set; }

    [ForeignKey("AppUser")]
    public string SubmittedByUserId { get; set; }
    public virtual AppUser SubmittedByUser { get; set; }

    public DateTime SubmittedDate { get; set; }

    [Required, StringLength(50)]
    public string Status { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }

    public virtual ICollection<FORM_SUBMISSION_VALUES> FORM_SUBMISSION_VALUES { get; set; }
    public virtual ICollection<FORM_SUBMISSION_ATTACHMENTS> FORM_SUBMISSION_ATTACHMENTS { get; set; }
    public virtual ICollection<FORM_SUBMISSION_GRID_ROWS> FORM_SUBMISSION_GRID_ROWS { get; set; }
    public virtual ICollection<DOCUMENT_APPROVAL_HISTORY> DOCUMENT_APPROVAL_HISTORY { get; set; }
}
