using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPicTest : Form
    {
        /// <summary>
        /// 建構子
        /// </summary>
        public frmPicTest()
        {
            InitializeComponent();
        }

        #region
        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }
        /// <summary>
        /// 當按下測試按鈕時，隨機產生1~52的數字，並將對應的撲克牌圖片顯示在picTest上，同時在lblNum上顯示該數字。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int picNum = random.Next(1, 53);   
           
            this.picTest.Image = GetImage($"pic{picNum}");

            lblNum.Text = picNum.ToString();
        }
        #endregion
    }
}
