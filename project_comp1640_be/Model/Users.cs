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
        [StringLength(20)]
        public string user_username { get; set; }

        [Required]
        [StringLength(50)]
        public string user_email { get; set; }

        [Required]
        public string user_password { get; set; }

        public string user_confirm_password { get; set; }

        public string? token { get; set; }

        public string? refesh_token { get; set; }

        public DateTime refesh_token_exprytime { get; set; }

        public string? reset_password_token { get; set; }

        public DateTime reset_password_exprytime { get; set; }

        [Required]
        public user_status user_status { get; set; } = user_status.Unlock;

        public string? user_avatar { get; set; }

        [Required]
        public virtual Roles? role { get; set; }

        [Required]
        public virtual Faculties? faculties { get; set; }

        public ICollection<Contributions>? Contributions { get; set; }

        public ICollection<Marketing_Comments>? Marketing_Comments { get; set; }

    }

    public enum user_status
    {
        Lock, Unlock
    }
}
