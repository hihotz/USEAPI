using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USEAPI
{
    public partial class Main : Form
    {
        private Setting setting = null;
        public Main()
        {
            InitializeComponent();
        }

        public static string HomeURL;       //기본 홈 설정, Setting.cs에서 이용
        private static int flag = 1;        //플래그 .

        #region Form1 로드시 기본 세팅
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] saveurl = File.ReadAllLines(@"..\..\textFile\URL.txt");
                HomeURL = saveurl[0];
            }
            catch
            {
                //웹브라우저 기본홈 설정안되있을경우 구글 기본로딩
                HomeURL = "https://www.google.co.kr/";
            }
            
            Web_URL.Text = HomeURL;
            Web_Search.Navigate(Web_URL.Text);
            this.Invalidate();

            //before 기본텍스트 입력
            before.Text = "입력하세요";
        }
        #endregion

        #region 파일열기 탭
        private void FindText_Click(object sender, EventArgs e)
        {
            try
            {
                String file_path = null;
                OpenFileDialog openFileDialog2 = new OpenFileDialog();
                openFileDialog2.Filter = "텍스트파일(*.txt)|*.txt|C파일(*.c)|*.c|C++파일(*.cpp)|*.cpp|C#파일(*.cs)|*.cs";
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    file_path = openFileDialog2.FileName;       //선택된 파일의 풀 경로를 불러와 저장
                    FindText_URL.Text = file_path;
                }
                string textValue = File.ReadAllText(FindText_URL.Text); // 파일  읽어 오기
                richTextBox1.Text = textValue;
            }
            catch (Exception)
            {
                richTextBox1.Text = " 읽을 수 없는 파일입니다.";
            }
        }
        private void FindFile_Click(object sender, EventArgs e)
        {
            FindFile_URL.Clear();

            String file_path = null;
            //openFileDialog1.InitialDirectory = FullPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file_path = openFileDialog1.FileName;       //선택된 파일의 풀 경로를 불러와 저장
                //FindFile_URL.Text = "file:\\\\\\" + file_path;
                FindFile_URL.Text = file_path;
            }

            string url = FindFile_URL.Text;
            web.Navigate(url);
        }
        #endregion
        
        #region 마우스 클릭 이벤트
        //마우스 우클릭
        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //마우스 우클릭
            if (e.Button == MouseButtons.Right)
            {
                before.Text = richTextBox1.SelectedText;
            }
            //마우스 휠클릭(한글 클릭시 오류)
            if (e.Button == MouseButtons.Middle)
            {
                //멀티바이트
                Web_URL.Text = "https://search.naver.com/search.naver?ie=MBCS&query=" + richTextBox1.SelectedText;
                //UTF-8
                //Web_URL.Text = "https://search.naver.com/search.naver?ie=UTF-8&query=" + richTextBox1.SelectedText;
                Web_URL_Btn_Click(sender, e);
            }
        }
        #endregion

        #region 웹검색 버튼
        private void GoBack_Click(object sender, EventArgs e)
        {
            Web_Search.GoBack();
        }
        private void GoForward_Click(object sender, EventArgs e)
        {
            Web_Search.GoForward();
        }
        private void GoHome_Click(object sender, EventArgs e)
        {
            Web_URL.Text = HomeURL;
            Web_URL_Btn_Click(sender, e);
        }
        private void Web_URL_Btn_Click(object sender, EventArgs e)
        {
            Web_Search.Navigate(Web_URL.Text);
        }
        private void Web_URL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Web_URL_Btn_Click(sender, e);
            }
        }
        private void Web_Search_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Web_URL.Text = Web_Search.Url.AbsoluteUri.ToString();
        }
        #endregion

        #region 번역기 버튼
        private void Clear_before_Btn_Click(object sender, EventArgs e)
        {
            before.Clear();
        }

        private void Translate_Btn_Click(object sender, EventArgs e)
        {
            Papago_Api();
        }
        private void Change_Btn_Click(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                flag = 2;
                Change_Btn.Text = "영어 -> 한글";
            }
            else if (flag == 2)
            {
                flag = 1;
                Change_Btn.Text = "한글 -> 영어";
            }
        }
        private void before_KeyDown(object sender, KeyEventArgs e)
        {
            //번역 텍스트칸에서 엔터 입력시 번역 작동
            if (e.KeyCode == Keys.Enter)
            {
                Translate_Btn_Click(sender, e);
            }
        }
        #endregion

        #region 파파고 번역
        //파파고
        private void Papago_Api()
        {
            try
            {
                string url = "https://openapi.naver.com/v1/papago/n2mt";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("X-Naver-Client-Id", "");             //개인키코드, papago에서 api 발급받아 입력
                request.Headers.Add("X-Naver-Client-Secret", "");         //개인키코드, papago에서 api 발급받아 입력
                request.Method = "POST";
                if (flag == 1)
                {
                    //before textbox의 텍스트를 전달
                    string query = before.Text;
                    byte[] byteDataParams = Encoding.UTF8.GetBytes("source=ko&target=en&text=" + query);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteDataParams.Length;
                    Stream st = request.GetRequestStream();
                    st.Write(byteDataParams, 0, byteDataParams.Length);
                    st.Close();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string text = reader.ReadToEnd();
                    stream.Close();
                    response.Close();
                    reader.Close();
                    //after textbox에 처리된 데이터 출력
                    text = CutText(text);
                    after.Text = text;
                }
                else if (flag == 2)
                {
                    //before textbox의 텍스트를 전달
                    string query = before.Text;
                    byte[] byteDataParams = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + query);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteDataParams.Length;
                    Stream st = request.GetRequestStream();
                    st.Write(byteDataParams, 0, byteDataParams.Length);
                    st.Close();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string text = reader.ReadToEnd();
                    stream.Close();
                    response.Close();
                    reader.Close();
                    //after textbox에 처리된 데이터 출력
                    text = CutText(text);
                    after.Text = text;
                }
            }
            catch(Exception)
            {
                MessageBox.Show("에러");
            }
        }

        //불필요 정보 삭제
        private string CutText(string text)
        {
            //앞부분 삭제
            int f1 = text.LastIndexOf("translatedText\":\"");
            string f2 = text.Substring(f1 + 17);
            //뒤부분 삭제
            int b1 = f2.LastIndexOf("\",\"engineType");
            string b2 = f2.Remove(b1);           

            return b2;
        }
        #endregion
        
        #region 환경설정 - Setting
        private void 검색ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (setting == null)
            {
                setting = new Setting();
                setting.Owner = this;
                setting.Changed += new EventHandler(SettingApply);  // 컨트롤 등의 변경
                setting.CloseSetting += new EventHandler(SettingClose); // 닫음
                setting.Show();
            }
            else
            {
                setting.Focus();
            }
        }

        public void SettingApply(object sender, EventArgs e)
        {
            HomeURL = setting.SetURL;     // pathSetting Form에서  Rootpath 정보를 가져와서 ~
        }

        public void SettingClose(object sender, EventArgs e)
        {
            //모달리스 종료 처리
            setting.Dispose();
            setting = null;
        }

        #endregion
    }
}

/*todo
 * 수정됨 - 번역 영어>한글 기능 추가
 * 수정됨 - 오류 수정 try/catch 추가함
 * 수정됨 - 웹페이지 기본 사이트 설정
 * 수정됨 - 웹피이지 이동시 표시되는 url경로 수정
 * 수정됨 - 홈페이지 url 외부 입출력
 * 수정됨 - Main폼에서 Setting 폼 연동
 * 
 */
