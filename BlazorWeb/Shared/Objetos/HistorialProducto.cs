using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWeb.Shared.Objetos
{
    public class HistorialProducto 
    {
        public int Id { get; set; }
        public string Movimiento { get; set; }
        public DateTime FechaEdicion { get; set; }
        public string UsuarioId { get; set; }
        public String NomUsusario { get; set; }

        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Ant_Stock { get; set; }
        public decimal Stock { get; set; }
        public decimal Precio_compra { get; set; }
        public decimal Ant_Precio_venta { get; set; }
        public decimal Precio_venta { get; set; }
        public string Codigo { get; set; }

    }
}
