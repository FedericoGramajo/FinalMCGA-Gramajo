using BlazorWeb.Shared.Objetos;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorWeb.Client.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {

        public async static ValueTask<bool> Confirm(this IJSRuntime js, string titulo, string mensaje, TipoMensajeSweetAlert tipoMensajeSweetAlert)
        {
            return await js.InvokeAsync<bool>("CustomConfirm", titulo, mensaje, tipoMensajeSweetAlert.ToString());
        }
 
     
        //desde aca todas funciones que controlan el socket del login
        public static async ValueTask InicializarTimerInactivo<T>(this IJSRuntime js,
          DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            await js.InvokeVoidAsync("timerInactivo", dotNetObjectReference);
        }

        public static ValueTask<object> SetInLocalStorage(this IJSRuntime js, string key, string content)
   => js.InvokeAsync<object>(
       "localStorage.setItem",
       key, content
       );

        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
            => js.InvokeAsync<string>(
                "localStorage.getItem",
                key
                );

        public static ValueTask<object> RemoveItem(this IJSRuntime js, string key)
            => js.InvokeAsync<object>(
                "localStorage.removeItem",
                key);
    }

    public enum TipoMensajeSweetAlert
    {
        question, warning, error, success, info
    }
}
