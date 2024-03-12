using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Marketing_Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int comment_id { get; set; }

        [Required]
        public string comment { get; set; }

        [Required]
        public DateTime comment_date { get; set; }

        public int comments_user_id { get; set; }
        [ForeignKey("comments_user_id")]
        public virtual Users users { get; set; }

        public int comments_contribution_id { get; set; }
        [ForeignKey("comments_contribution_id")]
        public virtual Contributions contributions { get; set; }
    }
}
