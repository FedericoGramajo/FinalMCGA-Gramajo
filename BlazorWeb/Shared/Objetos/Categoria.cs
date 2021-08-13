using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorWeb.Shared.Objetos
{
   public class Categoria
    {
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Rubro { get; set; }
    }
}
