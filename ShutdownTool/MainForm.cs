using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
namespace ShutdownTool
{
    public partial class MainForm : Form
    {
        int _totalTimeSec;

        public MainForm()
        {
            InitializeComponent();
            for (int h = 0; h < 24; h++) hourComboBox.Items.Add(h);
            for (int m = 0; m < 60; m++) minComboBox.Items.Add(m);
            for (int s = 0; s < 60; s++) secComboBox.Items.Add(s);
        }

        bool unitValueCheck(params String[] values)
        {
            int[] nums = new int[3];
            int index = 0;
            foreach (var value in values)
            {
                if (!Int32.TryParse(value, out nums[index])) return false;
                if (nums[index] < 0 || nums[index] > int.MaxValue) return false;
                index++;
            }
            return true;
        }

        int convertToTotalSec(string hour, string min, string sec)
        {
            _totalTimeSec = 0;
            _totalTimeSec += 60 * 60 * Convert.ToInt32(hour);
            _totalTimeSec += 60 * Convert.ToInt32(min);
            _totalTimeSec += Convert.ToInt32(sec);
            return _totalTimeSec;
        }

        void runProcess(string param)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "shutdown.exe";
            psi.Arguments = param;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!unitValueCheck(hourComboBox.Text, minComboBox.Text, secComboBox.Text))
            {
                errorLabel.Text = "正しい値を入力してください";
                return;
            }
            else errorLabel.Text = String.Empty;
            int totalSec = convertToTotalSec(hourComboBox.Text, minComboBox.Text, secComboBox.Text);
            if(totalSec == 0)
            {
                DialogResult result = MessageBox.Show("直ちにシャットダウンしますか？","タイマーが0秒にセットされています",MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) return;
            }
            runProcess($"/s /t {totalSec}");
            countDownTimer.Start();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            countDownTimer.Stop();
            runProcess("/a");
            hourLabel.Text = minLabel.Text = secLabel.Text = " ";
        }

        private void countDownTimer_Tick(object sender, EventArgs e)
        {
            _totalTimeSec--;
            int hour = (_totalTimeSec / 60) / 60;
            int min = (_totalTimeSec - (hour * 60 * 60)) / 60;
            int sec = (_totalTimeSec - (hour * 60 * 60)) - (min * 60);
            hourLabel.Text = hour.ToString();
            minLabel.Text = min.ToString();
            secLabel.Text = sec.ToString();
        }
    }
}
