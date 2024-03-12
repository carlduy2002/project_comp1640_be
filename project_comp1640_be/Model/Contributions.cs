using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Contributions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int contribution_id { get; set; }

        [Required]
        public string contribution_title { get; set; }

        [Required]
        public string contribution_content { get; set; }

        [Required]
        public string contribution_image { get; set; }

        [Required]
        public DateTime contribution_submition_date { get; set; }

        public IsEnabled IsEnabled { get; set; }

        public IsSelected IsSelected { get; set; }

        public IsView IsView { get; set; }

        public int contribution_user_id { get; set; }
        [ForeignKey("contribution_user_id")]
        public virtual Users users { get; set; }

        public int contribution_academic_id { get; set; }
        [ForeignKey("contribution_academic_id")]
        public virtual Academic_Years academic_years { get; set; }

        public ICollection<Marketing_Comments> Marketing_Comments { get; set; }
    }

    public enum IsEnabled
    {
        Enabled, Unenabled
    }

    public enum IsSelected
    {
        Selected, Unselected
    }

    public enum IsView 
    {
        View, Unview
    }


}
