using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Academic_Years
    {
        [Key]
        public int academic_year_id { get; set;}

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string academic_year_title { get; set; }

        [Required]
        public DateTime academic_year_ClosureDate { get; set; }

        [Required]
        public DateTime academic_year_FinalClosureDate { get; set; }
    
        public ICollection<Contributions>? contributions { get; set; }
    }
}
