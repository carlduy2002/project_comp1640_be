using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Contributions
    {
        [Key]
        public int contribution_id { get; set; }
        public string? contribution_title { get; set; }
        public string? contribution_content { get; set; }
        public string? contribution_image { get; set; }
        public DateTime? contribution_submition_date { get; set; }
        public IsEnabled? IsEnabled { get; set; }
        public IsSelected? IsSelected { get; set; }
        public IsView? IsView { get; set; }

        [ForeignKey("users")]
        public int contribution_user_id { get; set; }
        public virtual Users? users { get; set; }

        [ForeignKey("academic_years")]
        public int contribution_academic_years { get; set; }
        public virtual Academic_Years? academic_years { get; set; }

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
