using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileService
{
    class HorarioData
    {
        public string NombreLinea { get; set; }
        public List<Ramales> Ramales { get; set; }
    }

    class Ramales
    {
        public string NombreLinea { get; set; }
        public string Ramal { get; set; }
        public string Distancia { get; set; }
        public string Tiempo { get; set; }
        public string DistanciaProximo { get; set; }
        public string TiempoProximo { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
    }
}