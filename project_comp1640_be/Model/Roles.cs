using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int role_id {  get; set; }

        [Required]
        public string role_name { get; set; }

        public ICollection<Users> users { get; set; }
    }
}
