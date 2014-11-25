using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace MobileService
{
    public class Helper
    {
        private const string API_KEY = "AIzaSyAHMcqlO5o7GrGXpf6gD_-ndY1wxLQp_t4";
        private const string cnnStringGCM_News =
                    "Server=08b75c75-cfac-4d9b-b023-a39b01057665.sqlserver.sequelizer.com;Database=db08b75c75cfac4d9bb023a39b01057665;User ID=dkeybpcggpoutvaf;Password=CJPQEYNWiXiAY5TUxzy8DHJ3sbQDHbPEGZkyK3ZrTvYnAMytZWzuzbR4aVwCiing;";


        public static string SendGCMNotification(string apiKey, string postData, string postDataContentType = "application/json")
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //
            //  MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.ContentLength = byteArray.Length;

            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = Request.GetResponse();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                    return "Unauthorized - invalid token";
                }
                if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    return "An error ocurred processing the request";
                }

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
                Reader.Close();

                return responseLine;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static bool ValidateServerCertificate(
                                            object sender,
                                            X509Certificate certificate,
                                            X509Chain chain,
                                            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Lee los feeds de cada url
        /// </summary>
        /// <returns>Cantidad de feeds validos</returns>
        public static int ReadFeeds()
        {
            int cant = 0;
            List<string> rssUrlsList = new List<string> { "http://www.lavoz.com.ar/rss.xml", "http://www.diaadia.com.ar/rss.xml",
                "http://www.lmcordoba.com.ar/rss/ultimas.xml", "http://www.cba24n.com.ar/publico/tda_xml.php" }; //, "http://ersaurbano.com/feed"};

            foreach (string url in rssUrlsList)
            {
                try
                {
                    string result = GetPageAsString(new Uri(url));
                    if (string.IsNullOrEmpty(result)) continue;
                    DataSet rssData = new DataSet();
                    rssData.ReadXml(new StringReader(result), XmlReadMode.Auto);
                    cant += rssData.Tables["item"].Rows.Cast<DataRow>().Count(dataRow => ValidTitle(dataRow["title"].ToString()));
                }
                catch (Exception ex)
                {
                    cant = 0;
                }
            }

            return cant;
        }

        private static string GetPageAsString(Uri address)
        {
            string result = "";
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
            }
            return result;
        }

        private static bool ValidTitle(string title)
        {
            if (!title.Contains("paro") && !title.Contains("Paro")) return false;
            return title.Contains("transporte") || title.Contains("UTA") || title.Contains("colectivo") ||
                   title.Contains("colectivos") || title.Contains("choferes")
                   || title.Contains("chofer") || title.Contains("transportes");
        }

        public static void GCMNotification(int cant)
        {
            try
            {
                string DEVICE_TOKEN = "";
                SqlConnection cnn = new SqlConnection(cnnStringGCM_News);
                SqlCommand cmd = new SqlCommand("SELECT * FROM GCM_News", cnn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cnn.Open();
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                cnn.Close();

                const string message = "Nuevas Noticias Disponibles";
                const string tickerText = "Nuevas Noticias Transporte Córdoba";
                const string contentTitle = "Informe Transporte Cordoba";
                string subText = cant + " noticias";
                if (cant == 1) { subText = cant + " noticia"; }

                if (ds.Tables.Count <= 0) return;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DEVICE_TOKEN = row["GCMToken"].ToString() + row["GCMToken2"].ToString();

                    string postData =
                        "{ \"registration_ids\": [ \"" + DEVICE_TOKEN + "\" ], " +
                        "\"data\": {\"tickerText\":\"" + tickerText + "\", " +
                        "\"contentTitle\":\"" + contentTitle + "\", " +
                        "\"subText\":\"" + subText + "\", " +
                        "\"message\": \"" + message + "\"}}";

                    string response = Helper.SendGCMNotification(API_KEY, postData);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}