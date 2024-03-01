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

        [Required]  
        public virtual Users users { get; set; }

        [Required]
        public virtual Contributions contributions { get; set; }
    }
}
