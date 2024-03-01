using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Faculties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int faculty_id { get; set; }

        [Required]
        public string faculty_name { get; set; }

        public ICollection<Users> users { get; set; }
    }
}
