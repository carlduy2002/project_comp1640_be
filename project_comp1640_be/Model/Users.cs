using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string user_username { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string user_email { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string user_password { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? user_confirm_password { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? token { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? refesh_token { get; set; }

        public DateTime? refesh_token_exprytime { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? reset_password_token { get; set; }

        public DateTime? reset_password_exprytime { get; set; }

        public DateTime? last_login { get; set; }

        public int? total_work_duration { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public user_status user_status { get; set; } = user_status.Unlock;

        [Column(TypeName = "varchar(255)")]
        public string? user_avatar { get; set; }

        [ForeignKey("role")]
        public int user_role_id { get; set; }
        public virtual Roles? role { get; set; }

        [ForeignKey("faculties")]
        public int user_faculty_id { get; set; }
        public virtual Faculties? faculties { get; set; }

        public ICollection<Contributions>? Contributions { get; set; }

        public ICollection<Marketing_Comments>? Marketing_Comments { get; set; }

        public ICollection<Page_Views>? Page_Views { get; set; }

    }

    public enum user_status
    {
        Lock, Unlock
    }
}
