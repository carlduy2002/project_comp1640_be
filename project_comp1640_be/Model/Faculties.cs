using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Faculties
    {
        [Key]
        public int faculty_id { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string faculty_name { get; set; }

        public ICollection<Users>? users { get; set; }   
    }
}
