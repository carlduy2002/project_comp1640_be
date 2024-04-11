using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Contributions
    {
        [Key]
        public int contribution_id { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string contribution_title { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string contribution_content { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string contribution_image { get; set; }

        [Required]
        public DateTime contribution_submition_date { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string IsEnabled { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string IsSelected { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string IsView { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string IsPublic { get; set; }

        [ForeignKey("users")]
        public int contribution_user_id { get; set; }
        public virtual Users? users { get; set; }

        [ForeignKey("academic_years")]
        public int contribution_academic_years_id { get; set; }
        public virtual Academic_Years? academic_years { get; set; }

        public ICollection<Marketing_Comments>? Marketing_Comments { get; set; }
    }

    public enum IsEnabled
    {
        Enabled, Unenabled
    }

    public enum IsSelected
    {
        Selected, Unselected, Pending
    }

    public enum IsView 
    {
        View, Unview
    }

    public enum IsPublic
    {
        Private, Public, NotPublic
    }
}
