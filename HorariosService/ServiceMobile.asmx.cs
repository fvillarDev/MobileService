using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
//using Newtonsoft.Json;
using System.Xml;

namespace MobileService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://androiddev.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class ServiceMobile : System.Web.Services.WebService
    {
        private string idGeo = "dd540f1f-44ac-4929-a4e8-777a5d9b66b3";
        private string key = "n9tNJicGUqLc6KbDzeGoVNoDcNC70rjEgrXrKM8a";
        string BASE_URL = "http://ws_geosolution.geosolution.com.ar/mobile_test/Mobile/";

        [WebMethod]
        public string GetHourMobile(string stop, string bus)
        {
            bool isNoData = true;
            string res = "";
            while (isNoData)
            {
                res = GetData(stop, bus);
                if (res.Contains("ErrorMessage") && res.Contains("nivel"))
                { }
                else
                    isNoData = false;
                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(res);
                //if (!doc.DocumentElement.FirstChild.Value.Contains("Error"))
                //{
                //    isNoData = false;
                //    res = doc.DocumentElement.FirstChild.Value.ToString();
                //}
            }
            return res;
        }

        [WebMethod]
        public string GetNearStopsByBus(string bus, string lat, string lng)
        {
            bool isNoData = true;
            string res = "";
            while (isNoData)
            {
                res = StopsByBus(bus, lat, lng);
                if (res.Contains("ErrorMessage") && res.Contains("nivel"))
                { }
                else
                    isNoData = false;
            }
            return res;
        }

        [WebMethod]
        public string GetPositionByStop(string stop)
        {
            bool isNoData = true;
            string res = "";
            while (isNoData)
            {
                res = GetParadaStop(stop);
                if (res.Contains("ErrorMessage") && res.Contains("nivel"))
                { }
                else
                    isNoData = false;
            }
            return res;
        }


        private string GetData(string parada, string linea)
        {
            DateTime date = DateTime.Now;
            string dayToSend = date.ToString("yyyy-MM-ddThh:mm:ss");
            string mensajeFecha = date.ToString("yyyyMMddhhmmss");
            string hashBuild = "";
            string mensaje = linea + parada + mensajeFecha;

            byte[] keyArray;
            using (HMACMD5 m = new HMACMD5(UTF8Encoding.UTF8.GetBytes(key)))
            {
                keyArray = m.ComputeHash(UTF8Encoding.UTF8.GetBytes(mensaje));
            }

            hashBuild = Convert.ToBase64String(keyArray, 0, keyArray.Length);

            string uri = BASE_URL + "CalcularMobile?" +
                         "parada=" + parada + "&linea=" + linea +
                         "&id=" + idGeo +
                         "&hash=" + hashBuild + "&fecha=" + dayToSend;

            WebRequest req = WebRequest.Create(uri);
            WebResponse resp = req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        private string StopsByBus(string linea, string lat, string lng)
        { 
            DateTime date = DateTime.Now;
            string dayToSend = date.ToString("yyyy-MM-ddThh:mm:ss");
            string mensajeFecha = date.ToString("yyyyMMddhhmmss");
            string hashBuild = "";
            byte[] keyArray;
            using (HMACMD5 m = new HMACMD5(UTF8Encoding.UTF8.GetBytes(key)))
            {
                keyArray = m.ComputeHash(UTF8Encoding.UTF8.GetBytes(mensajeFecha));
            }

            hashBuild = Convert.ToBase64String(keyArray, 0, keyArray.Length);

            string uri = BASE_URL + "ObtenerParadasCercanasPorLinea?" +
                         "linea=" + linea + 
                         "&longitud=" + lng + "&latitud=" + lat + 
                         "&id=" + idGeo +
                         "&hash=" + hashBuild + "&fecha=" + dayToSend;

            WebRequest req = WebRequest.Create(uri);
            WebResponse resp = req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        private string GetParadaStop(string parada)
        { 
            DateTime date = DateTime.Now;
            string dayToSend = date.ToString("yyyy-MM-ddThh:mm:ss");
            string mensajeFecha = date.ToString("yyyyMMddhhmmss");
            string hashBuild = "";
            byte[] keyArray;
            using (HMACMD5 m = new HMACMD5(UTF8Encoding.UTF8.GetBytes(key)))
            {
                keyArray = m.ComputeHash(UTF8Encoding.UTF8.GetBytes(mensajeFecha));
            }

            hashBuild = Convert.ToBase64String(keyArray, 0, keyArray.Length);

            string uri = BASE_URL + "ObtenerParada?" +
                         "parada=" + parada +
                         "&id=" + idGeo +
                         "&hash=" + hashBuild + "&fecha=" + dayToSend;

            WebRequest req = WebRequest.Create(uri);
            WebResponse resp = req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }
    }
}