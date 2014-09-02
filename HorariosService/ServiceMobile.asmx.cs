using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;

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

        [WebMethod]
        public string TestService()
        {
            return "OK";
        }

        [WebMethod]
        public string GetHourMobile(string stop, string bus)
        {
            string res = GetData(stop, bus);
            return res;
        }

        private string GetData(string parada, string linea)
        {
            string idGeo = "dd540f1f-44ac-4929-a4e8-777a5d9b66b3";
            string key = "n9tNJicGUqLc6KbDzeGoVNoDcNC70rjEgrXrKM8a";

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

            string uri = "http://ws_geosolution.geosolution.com.ar/mobile_test/Mobile/CalcularMobile?" +
                         "parada=" + parada + "&linea=" + linea +
                         "&id=" + idGeo +
                         "&hash=" + hashBuild + "&fecha=" + dayToSend;

            WebRequest req = WebRequest.Create(uri);
            WebResponse resp = req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }
    }
}