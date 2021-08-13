using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorWeb.Client.Auth
{
    public class RenovadorToken : IDisposable
    {
        public RenovadorToken(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        Timer timer;
        private readonly ILoginService loginService;

        public void Iniciar()
        {
            //timer que renueva el webSocket de ser necesario
            timer = new Timer();
            timer.Interval = 240000; // 4 minutos en mili segundos
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            loginService.ManejarRenovacionToken();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
