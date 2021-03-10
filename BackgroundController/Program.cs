using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackgroundController
{
    static class Program
    {
        public static InterceptKeys Hook = new InterceptKeys();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Hook.SetHook();
            Application.Run(new Form1());
            Hook.UnHook();
        }
    }
}
