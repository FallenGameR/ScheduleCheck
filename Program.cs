using System;
using System.Threading;
using System.Windows.Forms;

namespace ScheduleCheck
{
    static class Program
    {
        public static string[] Args;

        [STAThread]
        static void Main(string[] args)
        {
            using var mutex = new Mutex(false, "FallenGameR ScheduleCheck");
            Args = args ?? new string[0];

            bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
            if (isAnotherInstanceOpen)
            {
                MessageBox.Show("Already running");
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            mutex.ReleaseMutex();
        }
    }
}
