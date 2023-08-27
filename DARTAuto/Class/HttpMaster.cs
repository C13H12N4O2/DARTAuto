using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

namespace DARTAuto
{
    internal class HttpMaster
    {
        public static HttpClient httpClient = new HttpClient();

        private static string accept = "*/*";
        private static string acceptEncoding = "gzip, deflate, br";
        private static string userAgent = Master.Name;

        public static string Accept
        {
            get { return accept; }
            set { accept = value; }
        }

        public static string AcceptEncoding
        {
            get { return acceptEncoding; }
            set { acceptEncoding = value; }
        }

        public static string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public static async Task<HttpResponseMessage> SendAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            SetRequestHeaders(request);

            return await httpClient.SendAsync(request);
        }

        private static void SetRequestHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Accept", accept);
            request.Headers.Add("Accept-Encoding", AcceptEncoding);
            request.Headers.Add("User-Agent", userAgent);
        }
    }
}
