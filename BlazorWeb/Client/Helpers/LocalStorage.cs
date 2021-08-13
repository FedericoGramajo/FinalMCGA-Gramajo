
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeb.Client.Helpers
{
    public class LocalStorage : ILocalStorage
    {
        private readonly IJSRuntime js;

        public LocalStorage(IJSRuntime js)
        {
            this.js = js;

        }

        public async Task GuardarArchivo(string file, string Nombre)
        {
             
            string Archivo = JsonConvert.SerializeObject("Certs/cert.pem");
            await js.SetInLocalStorage(Nombre, Archivo);

 
            }



        public async Task RecuperarArchivo(string nombre)
        {
            var Archivo = await js.GetFromLocalStorage(nombre);
            var file = JsonConvert.DeserializeObject<FileStream>(Archivo);
     

        }



    }
}