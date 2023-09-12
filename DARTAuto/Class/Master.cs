using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DARTAuto
{
    internal class Master
    {
        private static string openApiUrl = "https://opendart.fss.or.kr";
        private static string baseUrl = "https://dart.fss.or.kr";
        private static string corpCodeUrlPath = "/api/corpCode.xml";
        private static string corpCodePath = "CORPCODE.xml";
        private static string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private static string apiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        public static string OpenApiUrl
        {
            get { return openApiUrl; }
        }

        public static string BaseUrl
        {
            get { return baseUrl; }
        }

        public static string CorpCodeUrlPath
        {
            get { return corpCodeUrlPath; }
        }

        public static string CorpCodePath
        {
            get { return corpCodePath; }
        }

        public static string Name
        {
            get { return assemblyName; }
        }

        public static string ApiKey
        {
            get { return apiKey; }
            set { apiKey = ConfigurationManager.AppSettings["ApiKey"]; }
        }

        //public static DataTable GetCompanyData()
        //{
        //    try 
        //    {
        //        var companyDataTable = new DataTable();
        //        companyDataTable.Columns.Add(corp_code);
        //        companyDataTable.Columns.Add(corp_name);
        //        companyDataTable.Columns.Add(stock_code);
        //        companyDataTable.Columns.Add(modify_date);

        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(corpCodePath);

        //        XmlNodeList nodeList = xmlDoc.GetElementsByTagName("list");
        //        foreach (XmlNode node in nodeList)
        //        {
        //            string corpCode = node.SelectSingleNode(corp_code).InnerText;
        //            string corpName = node.SelectSingleNode(corp_name).InnerText;
        //            string stockCode = node.SelectSingleNode(stock_code).InnerText;
        //            string modifyDate = node.SelectSingleNode(modify_date).InnerText;

        //            companyDataTable.Rows.Add(corpCode, corpName, stockCode, modifyDate);
        //        }

        //        return companyDataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
