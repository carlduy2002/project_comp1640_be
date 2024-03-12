using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Academic_Years
    {
        [Key]
        public int academic_year_id { get; set;}
        public string? academic_year_title { get; set; }
        public DateTime? academic_Year_startClosureDate { get; set; }
        public DateTime? academic_Year_endClosureDate { get; set; }

        public ICollection<Contributions>? contributions { get; set; }

    }
}
