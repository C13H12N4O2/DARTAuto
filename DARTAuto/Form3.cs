using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DARTAuto
{
    public partial class Form3 : Form
    {
        private const string length = "99999";
        private const string offset = "99999";

        private string rcpNo = string.Empty;
        private string dcmNo = string.Empty;

        private enum FinancialReport
        {
            재무제표 = 21,
            연결재무제표 = 19
        }

        public Form3(string rcpNo, string dcmNo)
        {
            InitializeComponent();
            SetGlobalVariables(rcpNo, dcmNo);
            SetControls();
            SetEvent();
            OpenDocument();
        }

        private void SetGlobalVariables(string rcpNo, string dcmNo)
        {
            this.rcpNo = rcpNo;
            this.dcmNo = dcmNo;
        }

        private void SetControls()
        {
            comboBoxEdit1.Properties.Items.Add(FinancialReport.재무제표);
            comboBoxEdit1.Properties.Items.Add(FinancialReport.연결재무제표);

            comboBoxEdit1.SelectedIndex = 0;
        }

        private void SetEvent()
        {
            gridView1.RowCellStyle += new RowCellStyleEventHandler(gridView1_RowCellStyle);
            comboBoxEdit1.EditValueChanged += new EventHandler(comboBoxEdit1_EditValueChanged);
            simpleButton1.Click += new EventHandler(simpleButton1_Click);
        }

        private async void OpenDocument()
        {
            await GetAsyncData();
        }

        private async Task GetAsyncData()
        {
            int eleId = Convert.ToInt32(comboBoxEdit1.SelectedItem);
            string DocPathUrl = $"/report/viewer.do?rcpNo={rcpNo}&dcmNo={dcmNo}&eleId={eleId}&offset={offset}&length={length}&dtd=dart3.xsd";
            string url = Master.BaseUrl + DocPathUrl;

            var response = await HttpMaster.SendAsync(url);

            if (!response.IsSuccessStatusCode) return;

            string responseBody = await response.Content.ReadAsStringAsync();

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            var table = htmlDocument.DocumentNode.SelectNodes("//table");
            var thead = htmlDocument.DocumentNode.SelectSingleNode(".//thead");

            var dataTable = new DataTable();

            var head = thead.InnerText.Replace("&nbsp;", string.Empty).Trim().Split('\n');
            foreach (var data in head) dataTable.Columns.Add(new DataColumn(data));

            dataTable.Columns.Add(new DataColumn("growth", typeof(int)));
            dataTable.Columns.Add(new DataColumn("growthRate", typeof(double)));

            var header = table[0].SelectNodes(".//td");
            labelControl1.Text = header[0].InnerText.Replace("&nbsp;", string.Empty);
            labelControl2.Text = header[1].InnerText.Replace("&nbsp;", string.Empty);
            labelControl3.Text = header[2].InnerText.Replace("&nbsp;", string.Empty);
            labelControl4.Text = header[3].InnerText.Replace("&nbsp;", string.Empty);

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

            GridColumn column = gridView1.Columns["growth"];
            column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            column.DisplayFormat.FormatString = "#,##0"; 
            
            column = gridView1.Columns["growthRate"];
            column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            column.DisplayFormat.FormatString = "P2";

            CalcData();
        }

        private void CalcData()
        {
            gridControl1.BeginUpdate();

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

                gridView1.SetRowCellValue(i, "growth", left - right);
                gridView1.SetRowCellValue(i++, "growthRate", (left - right) / right);
            }

            var gridView = gridControl1.MainView as GridView;

            foreach (GridColumn column in gridView.Columns)
            {
                column.OptionsColumn.AllowEdit = false;
            }

            gridControl1.EndUpdate();
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            string columnName = e.Column.FieldName;

            if (columnName == "growth")
            {
                if (e.CellValue == null || string.IsNullOrEmpty(e.CellValue.ToString())) return;

                int cellValue = Convert.ToInt32(e.CellValue);
                if (cellValue > 0) {        e.Appearance.ForeColor = Color.Red; }
                else if (cellValue == 0) {  e.Appearance.ForeColor = Color.Black; }
                else {                      e.Appearance.ForeColor = Color.Blue; }
            }
            else if (columnName == "growthRate")
            {
                if (e.CellValue == null || string.IsNullOrEmpty(e.CellValue.ToString())) return;

                double cellValue = Convert.ToDouble(e.CellValue);
                if (cellValue > 0) { e.Appearance.ForeColor = Color.Red; }
                else if (cellValue == 0) { e.Appearance.ForeColor = Color.Black; }
                else { e.Appearance.ForeColor = Color.Blue; }
            }
        }

        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string pathUrl = $"/dsaf001/main.do?rcpNo={rcpNo}";
            string url = Master.BaseUrl + pathUrl;

            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
