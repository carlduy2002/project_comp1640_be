using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Academic_Years
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int academic_year_id { get; set;}

        [Required]
        public string academic_year_title { get; set; }

        [Required]
        public DateTime academic_Year_startClosureDate { get; set; }

        public DateTime? academic_Year_endClosureDate { get; set; }

        public ICollection<Contributions> contributions { get; set; }

    }
}
