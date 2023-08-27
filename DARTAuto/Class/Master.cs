using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DARTAuto
{
    internal class Master
    {
        private static string baseUrl = "https://opendart.fss.or.kr";
        private static string corpCodeUrlPath = "/api/corpCode.xml";
        private static string corpCodePath = "CORPCODE.xml";
        private static string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private static string apiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        public static string BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        public static string CorpCodeUrlPath
        {
            get { return corpCodeUrlPath; }
            set { }
        }

        public static string CorpCodePath
        {
            get { return corpCodePath; }
            set { }
        }

        public static string Name
        {
            get { return assemblyName.Substring(0, assemblyName.IndexOf('.')); }
            set { }
        }

        public static string ApiKey
        {
            get { return apiKey; }
            set { apiKey = ConfigurationManager.AppSettings["ApiKey"]; }
        }  
    }
}
