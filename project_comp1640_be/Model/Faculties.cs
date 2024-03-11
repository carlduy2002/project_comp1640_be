using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Faculties
    {
        [Key]
        public int faculty_id { get; set; }
        public string? faculty_name { get; set; }

        public ICollection<Users>? users { get; set; }   
    }
}
