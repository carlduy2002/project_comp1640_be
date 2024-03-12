﻿using System.ComponentModel.DataAnnotations;

namespace project_comp1640_be.Model
{
    public class Roles
    {
        [Key]
        public int role_id {  get; set; }
        public string? role_name { get; set; }

        public ICollection<Users>? users { get; set; }
    }
}
