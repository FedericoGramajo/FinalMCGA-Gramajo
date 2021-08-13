using BlazorWeb.Shared.Objetos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeb.Client.Auth
{
    public interface ILoginService
    {
        Task Login(UserToken token);
        Task Logout();
        Task ManejarRenovacionToken();
    }
}
