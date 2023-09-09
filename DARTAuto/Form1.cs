using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

namespace DARTAuto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetEvent();
            SetControlSetting();
            //SetCompanyLookUpEdit();
            //GetCompanyInfo();
            Test();
        }

        private void SetEvent()
        {
            gridView3.RowCellStyle += new RowCellStyleEventHandler(gridView3_RowCellStyle);
            //searchLookUpEdit1.EditValueChanged += SearchLookUpEdit1_EditValueChanged;
        }

        private void SetControlSetting()
        {
            searchLookUpEdit1.Properties.NullText = string.Empty;
            searchLookUpEdit1.Properties.DisplayMember = "corp_name";
            searchLookUpEdit1.Properties.ValueMember = "corp_code";
        }

        private void SetCompanyLookUpEdit()
        {
            searchLookUpEdit1.Properties.BeginUpdate();

            DataTable dataTable = new DataTable();

            DataColumn column = new DataColumn();
            column.Caption = "고유번호";
            column.ColumnName = "corp_code";
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.Caption = "회사명";
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

            dataTable.Columns["corp_code"].ColumnMapping = MappingType.Hidden;
            dataTable.Columns["stock_code"].ColumnMapping = MappingType.Hidden;
            dataTable.Columns["modify_date"].ColumnMapping = MappingType.Hidden;

            searchLookUpEdit1.Properties.DataSource = dataTable;

            searchLookUpEdit1.Properties.EndUpdate();
        }

        private void SetReportCodeLookUpEdit()
        {

        }

        private void GetCompanyInfo()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(Master.CorpCodePath);

                DataTable dataTable = searchLookUpEdit1.Properties.DataSource as DataTable;

                var nodeList = xmlDoc.GetElementsByTagName("list");
                foreach (XmlNode node in nodeList)
                {
                    string corpCode = node.SelectSingleNode("corp_code").InnerText;
                    string corpName = node.SelectSingleNode("corp_name").InnerText;
                    string stockCode = node.SelectSingleNode("stock_code").InnerText;
                    string modifyDate = node.SelectSingleNode("modify_date").InnerText;

                    dataTable.Rows.Add(corpCode, corpName, stockCode, modifyDate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void Test()
        {
            await OpenDocument();
        }

        private async Task OpenDocument()
        {
            string rcpNo = "20230811001703";
            string length = "99999";
            string offset = "99999";
            string dcmNo = "9383599";
            string eleId = "19";
            string url = $"https://dart.fss.or.kr/report/viewer.do?rcpNo={rcpNo}&dcmNo={dcmNo}&eleId={eleId}&offset={offset}&length={length}&dtd=dart3.xsd";
            //url = "https://dart.fss.or.kr/report/viewer.do?rcpNo=20230814002534&dcmNo=9393213&eleId=19&offset=99999&length=99999";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("User-Agent", "TestApp");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "TestApp");

            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(responseBody);

                var table = htmlDocument.DocumentNode.SelectNodes("//table");
                var thead = htmlDocument.DocumentNode.SelectSingleNode(".//thead");

                var dataTable = new DataTable();
                var head = thead.InnerText.Replace("&nbsp;", string.Empty).Trim().Split('\n');
                foreach (var data in head)
                {
                    dataTable.Columns.Add(new DataColumn(data));
                }

                dataTable.Columns.Add(new DataColumn("test1", typeof(int)));
                dataTable.Columns.Add(new DataColumn("test2", typeof(double)));

                var tbody = table[1].SelectNodes(".//tbody");

                for (int i = 0; i != tbody.Count; ++i)
                {
                    var tr = tbody[i].SelectNodes(".//tr");
                    foreach (var data in tr)
                    {
                        var list = data.InnerText.Replace("&nbsp;", string.Empty).Trim().Split('\n');
                        var row = dataTable.NewRow();
                        for (int j = 0; j != list.Length; ++j)
                        {
                            var text = list[j];
                            if (string.IsNullOrEmpty(text)) continue;
                            row[head[j]] = text;
                        }

                        dataTable.Rows.Add(row);
                    }
                }

                gridControl1.DataSource = dataTable;

                GridColumn column = gridView3.Columns["test2"];
                column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                column.DisplayFormat.FormatString = "P2";
                CalcData();
            }
        }

        private void CalcData()
        {
            gridControl1.BeginUpdate();
            GridColumn column1 = gridView1.Columns.AddVisible("test1", string.Empty);
            GridColumn column2 = gridView1.Columns.AddVisible("test2", string.Empty);

            int i = 0;
            foreach (DataRow row in (gridControl1.DataSource as DataTable).Rows)
            {
                string test1 = Convert.ToString(row.ItemArray[2]);
                string test2 = Convert.ToString(row.ItemArray[3]);

                if (Convert.ToString(row.ItemArray[2]).Contains("-") ||
                    Convert.ToString(row.ItemArray[3]).Contains("-") ||
                    Convert.ToString(row.ItemArray[2]).Contains("(") ||
                    Convert.ToString(row.ItemArray[3]).Contains("(") ||
                    Convert.ToString(row.ItemArray[2]) == string.Empty || 
                    Convert.ToString(row.ItemArray[3]) == string.Empty)
                {
                    ++i;
                    continue;
                }
                double left = Convert.ToDouble(row.ItemArray[2]);
                double right = Convert.ToDouble(row.ItemArray[3]);

                gridView3.SetRowCellValue(i, "test1", left - right);
                gridView3.SetRowCellValue(i++, "test2", (left - right) / right);
            }

            gridControl1.EndUpdate();
        }

        private void SearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            searchLookUpEdit1.Text = searchLookUpEdit1.EditValue.ToString();
        }

        private void gridView3_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "test2")
            {
                if (e.CellValue == null || string.IsNullOrEmpty(e.CellValue.ToString())) return;

                double cellValue = Convert.ToDouble(e.CellValue);
                if (cellValue > 0)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else if (cellValue == 0)
                {
                    e.Appearance.ForeColor = Color.Black;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
            else if (e.Column.FieldName == "test1")
            {
                if (e.CellValue == null || string.IsNullOrEmpty(e.CellValue.ToString())) return;

                int cellValue = Convert.ToInt32(e.CellValue);
                if (cellValue > 0)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else if (cellValue == 0)
                {
                    e.Appearance.ForeColor = Color.Black;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
        }
    }
}
