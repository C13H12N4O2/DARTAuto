using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base.Handler;
using System.Security.Policy;
using System.Xml;
using System.IO;
using DevExpress.XtraEditors;

namespace DARTAuto
{
    public partial class Form2 : Form
    {
        private const string corp_code = "corp_code";
        private const string corp_name = "corp_name";
        private const string stock_code = "stock_code";
        private const string modify_date = "modify_date";

        public Form2()
        {
            InitializeComponent();
            SetControls();
            SetEvent();
            SetCompanyGrid();
            SetReportGrid();
            GetCompanyData();
        }

        private void SetControls()
        {
            searchLookUpEdit1.Properties.NullText = string.Empty;
            searchLookUpEdit1.Properties.DisplayMember = "corp_name";
            searchLookUpEdit1.Properties.ValueMember = "corp_code";
        }

        private void SetEvent()
        {
            gridView1.DoubleClick += new EventHandler(gridView1_DoubleClick);
            searchLookUpEdit1.EditValueChanged += new EventHandler(searchLookUpEdit1_EditValueChanged);
        }

        private void SetCompanyGrid()
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

        private void SetReportGrid()
        {
            gridControl1.BeginUpdate();

            DataTable dataTable = new DataTable();

            DataColumn column = new DataColumn();
            column.ColumnName = "corp_cls";
            column.Caption = "법인구분";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "corp_name";
            column.Caption = "법인명";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "corp_code";
            column.Caption = "고유번호";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "stock_code";
            column.Caption = "종목코드";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "report_nm";
            column.Caption = "보고서명";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "rcept_no";
            column.Caption = "접수번호";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "flr_nm";
            column.Caption = "공시 제출인명";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "rcept_dt";
            column.Caption = "접수일자";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "rm";
            column.Caption = "비고";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);

            dataTable.Columns["corp_cls"].ColumnMapping = MappingType.Hidden;
            dataTable.Columns["corp_code"].ColumnMapping = MappingType.Hidden;
            dataTable.Columns["stock_code"].ColumnMapping = MappingType.Hidden;
            dataTable.Columns["rm"].ColumnMapping = MappingType.Hidden;

            gridControl1.DataSource = dataTable;

            gridControl1.EndUpdate();

            for (int i = 0; i != gridView1.Columns.Count; ++i)
            {
                gridView1.Columns[i].OptionsColumn.AllowEdit = false;
            }
        }

        private async void GetReportData()
        {
            await GetData();
        }

        private async Task GetData()
        {
            try
            {
                string corpCode = searchLookUpEdit1.EditValue.ToString();
                string beginDate = "20150117";
                string endDate = "20991231";
                string pageNo = "1";
                string pageCount = "100";
                string pblntfDetailTy = "A002";
                string test = "A003";

                string pathUrl = $"/api/list.json?crtfc_key={Master.ApiKey}&corp_code={corpCode}&bgn_de={beginDate}&end_de={endDate}&page_no={pageNo}&page_count={pageCount}&pblntf_detail_ty={pblntfDetailTy}&pblntf_detail_ty={test}";
                string url = Master.OpenApiUrl + pathUrl;

                HttpResponseMessage response = await HttpMaster.SendAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var data = JsonSerializer.Deserialize<DisclosureInformation.ResultData>(responseBody);

                    DataTable dataTable = gridControl1.DataSource as DataTable;

                    foreach (var item in data.list)
                    {
                        dataTable.Rows.Add(item.corp_cls, item.corp_name, item.corp_code,
                                                item.stock_code, item.report_nm, item.rcept_no,
                                                item.flr_nm, item.rcept_dt, item.rm);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetCompanyData()
        {
            try
            {
                var companyDataTable = searchLookUpEdit1.Properties.DataSource as DataTable;
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(Master.CorpCodePath);

                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("list");
                foreach (XmlNode node in nodeList)
                {
                    string corpCode = node.SelectSingleNode(corp_code).InnerText;
                    string corpName = node.SelectSingleNode(corp_name).InnerText;
                    string stockCode = node.SelectSingleNode(stock_code).InnerText;
                    string modifyDate = node.SelectSingleNode(modify_date).InnerText;

                    companyDataTable.Rows.Add(corpCode, corpName, stockCode, modifyDate);
                }

                searchLookUpEdit1.Properties.DataSource = companyDataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void gridView1_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as GridView;

            if (view == null) return;

            GridHitInfo hitInfo = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));

            if (!hitInfo.InRowCell) return;

            var data = view.GetRowCellValue(hitInfo.RowHandle, "rcept_no");

            string pathUrl = $"/dsaf001/main.do?rcpNo={data}";
            string url = Master.BaseUrl + pathUrl;
            
            HttpResponseMessage response = await HttpMaster.SendAsync(url);

            if (!response.IsSuccessStatusCode) return;

            string responseBody = await response.Content.ReadAsStringAsync();
            string pattern = @"node1\['dcmNo'\] = ""\d+"";";

            Match nodeData = Regex.Match(responseBody, pattern);
            if (!nodeData.Success) return;

            pattern = @"\b\d{7}\b";

            nodeData = Regex.Match(nodeData.Value, pattern);
            if (!nodeData.Success) return;

            var form = new Form3(data.ToString(), nodeData.Value);
            form.Show();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            GetReportData();
        }
    }
}
