using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorWeb.Shared.Objetos
{
    public class UserInfo
    {
       
        [Required]
        public string UserName { get; set; }

 
        [Required]
        public string Password { get; set; }
    }
}
