using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net;

namespace ScheduleCheck
{
    public partial class Form1 : Form
    {
        private const string controlFile = @"ftp://tp-share/sda1/control/allowed_time.txt";

        private static readonly (DateTime start, DateTime end) defaultAllowedTime = 
            (DateTime.Parse("8:00am"), DateTime.Parse("8:00pm"));

        private static readonly TimeSpan checkInterval = TimeSpan.FromSeconds(3);

        private readonly Timer timer = new Timer();

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        private bool IsAllowedTime
        {
            get
            {
                var now = DateTime.Now;
                var (start, end) = GetAllowedTime();
                return (start <= now) && (now <= end);
            }
        }

        private bool IsLockEnabled => this.IsEqual(Program.Args.FirstOrDefault(), "lock");

        private bool IsEqual(string first, string second)
        {
            return StringComparer.CurrentCultureIgnoreCase.Compare(first, second) == 0;
        }

        public Form1()
        {
            InitializeComponent();

            this.timer.Interval = (int)checkInterval.TotalMilliseconds;
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private (DateTime start, DateTime end) GetAllowedTime()
        {
            try
            {
                using var client = new WebClient();
                var text = client.DownloadString(controlFile);
                var lines = text.Split(
                    new[] { "\r\n" }, 
                    StringSplitOptions.RemoveEmptyEntries);
                return (DateTime.Parse(lines[0]), DateTime.Parse(lines[1]));
            }
            catch (Exception)
            {
                return defaultAllowedTime;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.IsAllowedTime)
            {
                if (this.IsLockEnabled)
                {
                    LockWorkStation();
                }
                else
                {
                    MessageBox.Show("Lock");
                }
            }
        }
    }
}
