using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_comp1640_be.Model
{
    public class Users
    {
        [Key]
        public int user_id { get; set; }

        public string? user_username { get; set; }
        public string? user_email { get; set; }
        public string? user_password { get; set; }
        public string? user_confirm_password { get; set; }
        public string? token { get; set; }
        public string? refesh_token { get; set; }
        public DateTime? refesh_token_exprytime { get; set; }
        public string? reset_password_token { get; set; }
        public DateTime? reset_password_exprytime { get; set; }
        public user_status user_status { get; set; } = user_status.Unlock;
        public string? user_avatar { get; set; }

        [ForeignKey("role")]
        public int user_role_id { get; set; }
        public virtual Roles? role { get; set; }

        [ForeignKey("faculties")]
        public int user_faculty_id { get; set; }
        public virtual Faculties? faculties { get; set; }

        public ICollection<Contributions>? Contributions { get; set; }

        public ICollection<Marketing_Comments>? Marketing_Comments { get; set; }

    }

    public enum user_status
    {
        Lock, Unlock
    }
}