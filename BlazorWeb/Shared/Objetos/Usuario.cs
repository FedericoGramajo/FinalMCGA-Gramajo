using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorWeb.Shared.Objetos
{
    public class Usuario : UserInfo
    {
        public string ID { get; set; }
       
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Rol rol { get; set; }
    }
}
