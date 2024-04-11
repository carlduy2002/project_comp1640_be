using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Page_Views
    {
        [Key]
        public int page_view_id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string page_view_name { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string browser_name { get; set; }

        [Required]
        public DateTime time_stamp { get; set; }

        [Required]
        public int total_time_access { get; set; }

        [ForeignKey("users")]
        public int page_view_user_id { get; set; }
        public virtual Users? users { get; set; }
    }
}
