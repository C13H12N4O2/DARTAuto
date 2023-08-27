using System;
using System.Xml;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using DARTAuto.DisclosureInformation;

namespace DARTAuto
{
    public partial class MasterForm : Form
    {
        private const string _baseUrl = "https://opendart.fss.or.kr";
        private const string _name = "DARTAuto";
        private const string _corpCodePath = "CORPCODE.xml";

        private string _apiKey;

        private System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        public MasterForm()
        {
            InitializeComponent();

            LoadApiKey();

            GetData();

            InitEvent();
        }

        private void LoadApiKey()
        {
            _apiKey = ConfigurationManager.AppSettings["ApiKey"];
        }

        private void InitEvent()
        {
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            dataGridView2.CellDoubleClick += dataGridView2_CellDoubleClick;
        }

        private async Task GetReportData()
        {
            try
            {
                string pathUrl = $"/api/list.json?crtfc_key={_apiKey}&corp_code={textBox1.Text}&bgn_de=20150117&end_de=20230827&corp_cls=Y&page_no=1&page_count=100&pblntf_ty=A";

                string url = _baseUrl + pathUrl;
                HttpRequestMessage request = SetRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var data = JsonSerializer.Deserialize<DisclosureInformation.ResultData>(responseBody);

                    // DataGridView에 컬럼 추가
                    dataGridView2.Columns.Add("corp_cls", "법인구분");
                    dataGridView2.Columns.Add("corp_name", "법인명");
                    dataGridView2.Columns.Add("corp_code", "고유번호");
                    dataGridView2.Columns.Add("stock_code", "종목코드");
                    dataGridView2.Columns.Add("report_nm", "보고서명");
                    dataGridView2.Columns.Add("rcept_no", "접수번호");
                    dataGridView2.Columns.Add("flr_nm", "공시 제출인명");
                    dataGridView2.Columns.Add("rcept_dt", "접수일자");
                    dataGridView2.Columns.Add("rm", "비고");

                    foreach (var item in data.list)
                    {
                        dataGridView2.Rows.Add( item.corp_cls, item.corp_name, item.corp_code, 
                                                item.stock_code, item.report_nm, item.rcept_no, 
                                                item.flr_nm, item.rcept_dt, item.rm);
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void SetHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _name);
        }

        private void GetData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("corp_code");
                dataTable.Columns.Add("corp_name");
                dataTable.Columns.Add("stock_code");
                dataTable.Columns.Add("modify_date");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_corpCodePath);

                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("list");
                foreach (XmlNode node in nodeList)
                {
                    string corpCode = node.SelectSingleNode("corp_code").InnerText;
                    string corpName = node.SelectSingleNode("corp_name").InnerText;
                    string stockCode = node.SelectSingleNode("stock_code").InnerText;
                    string modifyDate = node.SelectSingleNode("modify_date").InnerText;

                    dataTable.Rows.Add(corpCode, corpName, stockCode, modifyDate);
                }

                gridControl1.DataSource = dataTable;
            }
            catch (Exception e)
            {

            }
        }

        private HttpRequestMessage SetRequestMessage(HttpMethod method, string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("User-Agent", "TestApp");

            return request;
        }

        private async Task DownloadData(string urlPath, string outputPath, string fileName)
        {
            try
            {
                HttpRequestMessage request = SetRequestMessage(HttpMethod.Get, _baseUrl + urlPath);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

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
                                string entryPath = Path.Combine(outputPath, entry.FullName);
                                Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                                if (File.Exists(entryPath))
                                {
                                    continue;
                                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            GetReportData();
        }

        private async void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            await OpenDocument();
        }

        private async Task OpenDocument()
        {
            string recptNo = (dataGridView2.Rows)[0].Cells[5].Value.ToString();

            string pathUrl = $"/dsaf001/main.do?rcpNo={recptNo}";
            string url = _baseUrl + pathUrl;
            HttpRequestMessage request = SetRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseBody);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CompanyListForm companyListForm = new CompanyListForm();

            DialogResult result = companyListForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                return;
            }
        }
    }

    namespace DisclosureInformation
    {
        [DataContract]
        public class CorpData
        {
            [DataMember(Name = "corp_cls")]
            public string corp_cls { get; set; }
            [DataMember(Name = "corp_name")]
            public string corp_name { get; set; }
            [DataMember(Name = "corp_code")]
            public string corp_code { get; set; }
            [DataMember(Name = "stock_code")]
            public string stock_code { get; set; }
            [DataMember(Name = "report_nm")]
            public string report_nm { get; set; }
            [DataMember(Name = "rcept_no")]
            public string rcept_no { get; set; }
            [DataMember(Name = "flr_nm")]
            public string flr_nm { get; set; }
            [DataMember(Name = "rcept_dt")]
            public string rcept_dt { get; set; }
            [DataMember(Name = "rm")]
            public string rm { get; set; }
        }

        [DataContract]
        public class ResultData
        {
            public string status { get; set; }
            public string message { get; set; }
            public int page_no { get; set; }
            public int page_count { get; set; }
            public int total_count { get; set; }
            public int total_page { get; set; }

            [DataMember(Name = "list")]
            public List<CorpData> list { get; set; }
        }
    }
}
