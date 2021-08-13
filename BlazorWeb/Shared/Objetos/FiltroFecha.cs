using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
namespace BlazorWeb.Shared.Objetos
{
    public class FiltroFecha
    {

        public DateTime FechaInicio { get; set; } = DateTime.Now.Date;
        public DateTime FechaFinal { get; set; } = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

        public string DevolverFechas {
            get => "FechaInicio=" + JsonConvert.SerializeObject(FechaInicio).Replace("\"", "").Replace("+00:00", "") + " &FechaFinal=" + JsonConvert.SerializeObject(FechaFinal).Replace("\"", "").Replace("+00:00", "");
            set => DevolverFechas = value;
        }

        public string InicioString
        {
            get => JsonConvert.SerializeObject(FechaInicio).Replace("\"", "").Replace("+00:00", "");
            set => InicioString = value;
        }

        public string FinalString
        {
            get => JsonConvert.SerializeObject(FechaFinal).Replace("\"", "").Replace("+00:00", "");
            set => FinalString = value;
        }

        public  DateTime ConvertirFecha(string value)
        {
            try
            {
                DateTime fecha = JsonConvert.DeserializeObject<DateTime>("\"" + value + "\"");
                return fecha;
            }
            catch  
            {

                return Convert.ToDateTime("01/01/0001");
            }   
        }
    }
}
