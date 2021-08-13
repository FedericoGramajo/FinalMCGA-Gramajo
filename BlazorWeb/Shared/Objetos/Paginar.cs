using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWeb.Shared.Objetos
{
    public class Paginar
    {
        public int Pagina { get; set; } = 1;
        public int CantidadRegistros { get; set; } = 10;

    }
}
