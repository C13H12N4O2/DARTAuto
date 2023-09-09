using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DARTAuto
{
    public partial class CompanyListLookUpEditControl : UserControl
    {
        public CompanyListLookUpEditControl()
        {
            InitializeComponent();

            gridLookUpEdit.Properties.DataSource = Data.GetData();
        }
    }


    public class Data
    {
        private Data(string corpName, string stockCode, string modifyDate)
        {
            CorpName = corpName;
            StockCode = stockCode;
            ModifyDate = modifyDate;
        }

        public static Dictionary<string, Data> GetData()
        {
            try
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("corp_code");
                dataTable.Columns.Add("corp_name");
                dataTable.Columns.Add("stock_code");
                dataTable.Columns.Add("modify_date");

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(Master.CorpCodePath);

                var nodeList = xmlDoc.GetElementsByTagName("list");
                var dataSource = new Dictionary<string, Data>();
                foreach (XmlNode node in nodeList)
                {
                    string corpCode = node.SelectSingleNode("corp_code").InnerText;
                    string corpName = node.SelectSingleNode("corp_name").InnerText;
                    string stockCode = node.SelectSingleNode("stock_code").InnerText;
                    string modifyDate = node.SelectSingleNode("modify_date").InnerText;

                    var data = new Data(corpName, stockCode, modifyDate);
                    dataSource[corpCode] = data;
                }

                return dataSource;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string CorpName { get; set; }
        private string StockCode { get; set; }
        private string ModifyDate { get; set; }
    }
}
