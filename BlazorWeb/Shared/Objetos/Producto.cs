using BlazorWeb.Shared.Objetos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlazorWeb.Shared.Objetos
{
   public class Producto
    {
        public int ProductoId { get; set; }
        public Categoria Categoria { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Stock { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Precio_compra { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public  decimal Margen { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public  decimal Precio_venta { get; set; }
        public Proveedor Proveedor { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Codigo { get; set; }
        public string Marca { get; set; }
        public decimal Iva { get; set; } = 21;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CodigoBarra { get; set; }
        public string Discontinuado { get; set; } = "no";
    }

   
    public class Iva
    {
        public string Tipo { get; set; }
        public decimal valor { get; set; }
    }
 

}

