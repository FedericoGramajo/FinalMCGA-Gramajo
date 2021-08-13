﻿using Blazor.FileReader;
using BlazorWeb.Client.Auth;
using BlazorWeb.Client.Helpers;
using BlazorWeb.Client.Repositorios;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorWeb.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
         
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddScoped<IRepositorio, Repositorio>();
            services.AddScoped<DialogService>();
            services.AddScoped<IMostrarMensajes, MostrarMensajes>();
            services.AddAuthorizationCore();

            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);
          
            //token en json de forma local
            services.AddScoped<ProveedorAutenticacionJWT>();

            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());
            services.AddScoped<ILoginService, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());

            services.AddScoped<RenovadorToken>();

        }
    }
}
