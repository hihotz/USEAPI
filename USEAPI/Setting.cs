using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USEAPI
{
    public partial class Setting : Form
    {
        public event EventHandler Changed = null;
        public event EventHandler CloseSetting = null;

        public string SetURL
        {
            get { return SetHomeUrl.Text; }
        }

        public Setting()
        {
            InitializeComponent();

            SetText_label2();
            //창 우측상단 x키로 닫는 경우 정상종료시키기
            this.FormClosed += Cancel_Btn_Click;
        }

        private void SetText_label2()
        {
            label2.Text = "설정된 주소 : " + Main.HomeURL;
        }

        #region Ok/Cancel 버튼
        private void OK_Btn_Click(object sender, EventArgs e)
        {
            string[] URL = File.ReadAllLines(@"..\..\textFile\URL.txt");
            URL[0] = SetHomeUrl.Text;
            File.WriteAllLines(@"..\..\textFile\URL.txt", URL);

            if (Changed != null)
                Changed(this, new EventArgs());

            if (CloseSetting != null)
                CloseSetting(this, new EventArgs());
            this.Close();
        }

        private void Cancel_Btn_Click(object sender, EventArgs e)
        {
            if (CloseSetting != null)
                CloseSetting(this, new EventArgs());
            this.Close();

        }
        #endregion

        
        
            
    }
}
