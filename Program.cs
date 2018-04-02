using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    class Program
    {
        const string baseURL = "http://uat-intranet.nomuranow.com/TCAU/ticketid/rest/reserveid.php";
        //NEFURL= "http://uat-intranet.nomuranow.com/TCAU/ticketid/rest/reserveid.php?group=mtn&idtype=NEF";
        //const string MTNURL = "http://uat-intranet.nomuranow.com/TCAU/ticketid/rest/reserveid.php?group=mtn&idtype=MEIGARAMTN";
        static void Main(string[] args)
        {
            //Console.WriteLine("Meigara sample:");
            //Console.WriteLine(getIDfromWeb("meigara"));
            Console.WriteLine("NEF No. sample:");
            Console.WriteLine(getIDfromWeb("nef"));
            Console.WriteLine("NEF No. sample2:");
            Console.WriteLine(getIDfromWeb("nef"));
            Console.ReadKey();
        }
        static string getIDfromWeb(string idType)
        {
            int startPos = 0;
            string url = "";
            switch (idType)
            {
                case "nef":
                    startPos = 19;
                    url = baseURL + "?group=mtn&idtype=NEF";
                    break;
                case "meigara":
                    startPos = 15;
                    url = baseURL + "?group=mtn&idtype=MEIGARAMTN";
                    break;
            }

            // CookieContainer myContainer = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            CookieContainer cookies = new CookieContainer();
            request.CookieContainer = cookies;
            request.PreAuthenticate = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            reader.Close();
            response.Close();
            return responseFromServer.Substring(startPos, (responseFromServer.IndexOf("}]") - startPos - 1));

        }
    }
}
//}
