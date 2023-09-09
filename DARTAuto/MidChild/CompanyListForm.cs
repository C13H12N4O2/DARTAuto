using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Filtering;

namespace DARTAuto
{
    public partial class CompanyListForm : Form
    {
        private GridView mainView;
        private DataTable dataTable = new DataTable();

        public CompanyListForm()
        {
            InitializeComponent();
            InitGlobalVariables();
            SetEvent();
            SetGrid();
        }

        private void InitGlobalVariables()
        {
            mainView = CompanyListControl.MainView as GridView;
        }

        private void SetGrid()
        {
            mainView.BeginDataUpdate();

            DataColumn column = new DataColumn();
            column.Caption = "고유번호";
            column.ColumnName = "corp_code";
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.Caption = "정식회사명칭";
            column.ColumnName = "corp_name";
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.Caption = "종목코드";
            column.ColumnName = "stock_code";
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.Caption = "최종변경일자";
            column.ColumnName = "modify_date";
            dataTable.Columns.Add(column);

            CompanyListControl.DataSource = dataTable;

            //DataTable dt = new DataTable();

            //dt.Columns.Add("corp_code");
            //dt.Columns[0].ColumnName = "고유번호";

            //dt.Columns.Add("corp_name");
            //dt.Columns[1].ColumnName = "정식회사명칭";

            //dt.Columns.Add("stock_code");
            //dt.Columns[2].ColumnName = "종목코드";

            //dt.Columns.Add("modify_date");
            //dt.Columns[3].ColumnName = "최종변경일자";

            //CompanyListControl.DataSource = dt;

            //mainView.AddUnboundColumn("corp_code", "고유번호");
            //mainView.AddUnboundColumn("corp_name", "정식회사명칭");
            //mainView.AddUnboundColumn("stock_code", "종목코드");
            //mainView.AddUnboundColumn("modify_date", "최종변경일자");

            mainView.EndDataUpdate();
        }

        private void SetEvent()
        {
            this.Shown += new EventHandler(CompanyListForm_Shown);
        }

        private void GetData()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Master.CorpCodePath);

                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("list");

                for (int i = 0; i != nodeList.Count; ++i)
                {
                    string corpCode = nodeList[i].SelectSingleNode("corp_code").InnerText;
                    string corpName = nodeList[i].SelectSingleNode("corp_name").InnerText;
                    string stockCode = nodeList[i].SelectSingleNode("stock_code").InnerText;
                    string modifyDate = nodeList[i].SelectSingleNode("modify_date").InnerText;
                    
                    dataTable.Rows.Add(corpCode, corpName, stockCode, modifyDate);
                }

                CompanyListControl.DataSource = dataTable;

                //foreach (XmlNode node in nodeList)
                //{
                //    string corpCode = node.SelectSingleNode("corp_code").InnerText;
                //    string corpName = node.SelectSingleNode("corp_name").InnerText;
                //    string stockCode = node.SelectSingleNode("stock_code").InnerText;
                //    string modifyDate = node.SelectSingleNode("modify_date").InnerText;

                //    //DataRow row = dt.NewRow();
                //    //row["corp_code"] = corpCode;
                //    //row["corp_name"] = corpName;
                //    //row["stock_code"] = stockCode;
                //    //row["modify_date"] = modifyDate;
                //    //dt.Rows.Add(row);
                //}
            }
            catch (Exception e)
            {

            }
        }

        private async Task DownloadCompanyData()
        {
            try
            {
                string url = Master.OpenApiUrl + Master.CorpCodeUrlPath;
                HttpResponseMessage response = await HttpMaster.SendAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream zipFileSteam = File.Create("temp.zip"))
                        {
                            await contentStream.CopyToAsync(zipFileSteam);
                        }

                        using (ZipArchive archive = ZipFile.OpenRead("temp.zip"))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                string entryPath = Path.Combine(entry.FullName);
                                Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                                if (File.Exists(entryPath)) continue;
                                entry.ExtractToFile(entryPath, true);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {

            }
        }

        private void CompanyListForm_Shown(object sender, EventArgs e)
        {
            GetData();
        }
    }
}
