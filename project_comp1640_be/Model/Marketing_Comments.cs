using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Marketing_Comments
    {
        [Key]
        public int comment_id { get; set; }
        public string? comment { get; set; }
        public DateTime? comment_date { get; set; }

        [ForeignKey("users")]
        public int comment_user_id { get; set; }
        public virtual Users? users { get; set; }

        [ForeignKey("contributions")]
        public int comment_contribution_id { get; set; }
        public virtual Contributions? contributions { get; set; }
    }
}
