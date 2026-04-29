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
    public partial class frmPoker : Form
    {

        #region 
        /// <summary>
        /// 存放五張撲克牌的 PictureBox 陣列
        /// </summary>
        PictureBox[] pic = new PictureBox[5];
        #endregion
        public frmPoker()
        {
            InitializeComponent();
            InitializePoker();
        }

        // 金額相關變數
        int totalMoney = 1000000; // 總資金
        int betAmount = 0;        // 本局押注金額

        // 定義賠率字典 (根據圖片中的賠率表)
        Dictionary<string, int> oddsTable = new Dictionary<string, int>
    {
      { "皇家同花順", 250 },
      { "同花順", 50 },
      { "鐵支", 25 },
      { "葫蘆", 9 },
      { "同花", 6 },
      { "順子", 4 },
      { "三條", 3 },
      { "兩對", 2 },
      { "一對", 1 },
      { "雜牌", 0 }
    };

        private void btnBet_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtBet.Text, out betAmount))
            {
                if (betAmount > 0 && betAmount <= totalMoney)
                {
                    // 扣除押注金並更新顯示
                    totalMoney -= betAmount;
                    txtTotal.Text = totalMoney.ToString();

                    // --- 關鍵流程切換 ---
                    btnBet.Enabled = false;      // 鎖定押注，防止重複扣錢
                    txtBet.ReadOnly = true;      // 鎖定金額輸入
                    btnDealCard.Enabled = true;  // 允許開始發牌
                                                 // --------------------

                    MessageBox.Show("已押注，請點擊發牌！");
                }
                else
                {
                    MessageBox.Show("資金不足或押注金額無效！");
                }
            }
        }


        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        private Image GetImage(int num)
        {
            return GetImage("pic" + num);
        }

        #region 自定義方法
        private void InitializePoker()
        {
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = GetImage("back");
                pic[i].Name = "pic" + i;
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Top = 30;
                pic[i].Left = 10 + ((pic[i].Width + 10) * i);
                pic[i].Visible = true;
                pic[i].Enabled = false;
                pic[i].Tag = "back";
                // 將 pic 丟至到 grpPorker 內
                this.grpPoker.Controls.Add(pic[i]);
                pic[i].MouseClick += new MouseEventHandler(pic_Click);


            }
        }
        #endregion
        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            //MessageBox.Show(pic.Name);

        }

        int[] allPoker = new int[52];
        int[] playerPoker = new int[5];
        private async void btnDealCard_Click(object sender, EventArgs e)
        {
            this.lblResult.Text = "";
            // 先將牌面蓋掉
            for (int i = 0; i < 5; i++)
            {
                pic[i].Image = GetImage("back");
            }
            // 初始化52張牌
            for (int i = 0; i < 52; i++)
            {
                allPoker[i] = i;
            }
            // 洗牌
            Shuffle();
            // 暫停0.5秒
            await Task.Delay(500);
            // 發牌
            for (int i = 0; i < 5; i++)
            {
                pic[i].Image = GetImage("pic" + (allPoker[i] + 1));
                playerPoker[i] = allPoker[i];
            }

            // 啟用所有牌的點擊事件
            for (int i = 0; i < 5; i++)
            {
                pic[i].Enabled = true;
                pic[i].Tag = "front";
            }
            btnChangeCard.Enabled = true;
            btnDealCard.Enabled = false;
            btnCheck.Enabled = true;

        }

        private void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < allPoker.Length; i++)
            {
                int r = rand.Next(allPoker.Length);
                int temp = allPoker[r];
                allPoker[r] = allPoker[0];
                allPoker[0] = temp;
            }
        }

        private void pic_Click(object sender, MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            // 取得 pic 的索引值
            int index = int.Parse(pic.Name.Replace("pic", ""));
            int cardNum = playerPoker[index] + 1;
            // 如果 pic 的 Tag 為 back，則將顯示撲克牌
            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage(cardNum);
            }
            else
            {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }
        }

        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int cardIndex = 5;
            for (int i = 0; i < pic.Length; i++)
            {
                if (pic[i].Tag.ToString() == "back")
                {
                    playerPoker[i] = allPoker[cardIndex];
                    pic[i].Image = GetImage("pic" + (playerPoker[i] + 1));
                    pic[i].Tag = "front";
                    cardIndex++;
                }
            }
            // 禁用所有牌的點擊事件
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = false;
            }
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = true;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            // --- 1. 使用區域副本，防止「五張A」顯示錯誤 ---
            string[] tempPointNames = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            int[] pokerColor = new int[5];
            int[] pokerPoint = new int[5];
            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            for (int i = 0; i < 5; i++)
            {
                pokerColor[i] = playerPoker[i] % 4;
                pokerPoint[i] = playerPoker[i] / 4;
                colorCount[pokerColor[i]]++;
                pointCount[pokerPoint[i]]++;
            }

            // --- 2. 牌型邏輯判斷 ---
            bool isFlush = (colorCount.Max() == 5);
            bool isStraight = (pokerPoint.Max() - pokerPoint.Min() == 4 && pointCount.Max() == 1) ||
                    (pokerPoint.Contains(0) && pokerPoint.Contains(9) && pokerPoint.Contains(10) &&
                     pokerPoint.Contains(11) && pokerPoint.Contains(12));
            bool isRoyal = isStraight && pokerPoint.Contains(0) && pokerPoint.Contains(12);

            // 排序副本以取得出現次數最多的點數名稱
            Array.Sort(pointCount, tempPointNames);
            Array.Reverse(pointCount);
            Array.Reverse(tempPointNames);

            // --- 3. 核心：賠率對應與字串處理 ---
            string resultKey = "雜牌"; // 用於查找 oddsTable
            string displayMsg = "";    // 用於顯示在 lblResult

            if (isFlush && isRoyal) resultKey = "皇家同花順";
            else if (isFlush && isStraight) resultKey = "同花順";
            else if (pointCount[0] == 4) resultKey = "鐵支";
            else if (pointCount[0] == 3 && pointCount[1] == 2) resultKey = "葫蘆";
            else if (isFlush) resultKey = "同花";
            else if (isStraight) resultKey = "順子";
            else if (pointCount[0] == 3) resultKey = "三條";
            else if (pointCount[0] == 2 && pointCount[1] == 2) resultKey = "兩對";
            else if (pointCount[0] == 2) resultKey = "一對";

            // 建立顯示訊息（包含點數名稱，增加遊戲感）
            if (resultKey == "鐵支") displayMsg = $"{tempPointNames[0]} 鐵支";
            else if (resultKey == "一對") displayMsg = $"{tempPointNames[0]} 一對";
            else displayMsg = resultKey;

            // --- 4. 計算獎金 ---
            // 從字典抓取倍數
            int multiplier = oddsTable[resultKey];
            int winMoney = betAmount * multiplier;

            // 更新資金
            totalMoney += winMoney;
            txtTotal.Text = totalMoney.ToString();

            // 顯示結果
            if (winMoney > 0)
                lblResult.Text = $"{displayMsg}！獲利：{winMoney} (x{multiplier})";
            else
                lblResult.Text = $"{displayMsg}。沒中獎";

            // --- 5. 流程控制：回到押注階段 ---
            btnBet.Enabled = true;       // 下一局要重新按押注
            txtBet.ReadOnly = false;     // 解鎖輸入框
            btnDealCard.Enabled = false; // 沒押注不能發牌
            btnCheck.Enabled = false;
            btnChangeCard.Enabled = false;
        }

        /// <summary>
        /// 顯示五張撲克牌到桌面上
        /// </summary>
        private void ShowCards()
        {
            for (int i = 0; i < 5; i++)
            {
                pic[i].Image = this.GetImage($"pic{playerPoker[i] + 1}");
            }
        }
        private void frmPoker_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 只有在已經押注但還沒發牌/換牌的狀態下才允許作弊碼（根據你的邏輯 btnDealCard.Enabled == false）
            if (this.btnDealCard.Enabled == false)
            {
                bool isCheatKey = true; // 新增一個標記，判斷是否按下了有效的作弊鍵

                switch (e.KeyChar)
                {
                    case 'q': // 同花大順
                        playerPoker[0] = 51; playerPoker[1] = 47; playerPoker[2] = 43; playerPoker[3] = 39; playerPoker[4] = 3;
                        break;
                    case 'w': // 同花順
                        playerPoker[0] = 37; playerPoker[1] = 33; playerPoker[2] = 29; playerPoker[3] = 25; playerPoker[4] = 21;
                        break;
                    case 'e': // 同花
                        playerPoker[0] = 50; playerPoker[1] = 38; playerPoker[2] = 34; playerPoker[3] = 22; playerPoker[4] = 18;
                        break;
                    case 'r': // 鐵支
                        playerPoker[0] = 48; playerPoker[1] = 39; playerPoker[2] = 38; playerPoker[3] = 37; playerPoker[4] = 36;
                        break;
                    case 't': // 葫蘆
                        playerPoker[0] = 30; playerPoker[1] = 29; playerPoker[2] = 6; playerPoker[3] = 5; playerPoker[4] = 4;
                        break;
                    case 'y': // 三條
                        playerPoker[0] = 48; playerPoker[1] = 39; playerPoker[2] = 15; playerPoker[3] = 14; playerPoker[4] = 13;
                        break;
                    default:
                        isCheatKey = false; // 按了其他的鍵，標記為 false
                        break;
                }

                // 關鍵修正：只有按下正確的快捷鍵，才更新畫面
                if (isCheatKey)
                {
                    this.ShowCards();

                    // 建議同時更新 Tag，否則換牌邏輯可能會誤判這些牌是「背面」
                    for (int i = 0; i < pic.Length; i++)
                    {
                        pic[i].Tag = "front";
                    }
                }
            }
        }

    }
}