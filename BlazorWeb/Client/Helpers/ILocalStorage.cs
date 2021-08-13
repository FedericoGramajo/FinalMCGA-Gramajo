using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeb.Client.Helpers
{
    public interface ILocalStorage
    {
        Task GuardarArchivo(string file, string Nombre);

        Task RecuperarArchivo(string nombre);

    }
}
