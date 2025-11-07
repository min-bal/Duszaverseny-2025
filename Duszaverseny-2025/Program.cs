using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Duszaverseny_2025
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            AttachConsole(-1);
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                Console.WriteLine("Használat: WinFormsApp1.exe [--ui | <test_dir_path>]");
                return;
            }

            if (args[1] == "--ui")
            {
                /*
                Application.Configuration.Initialize();
                Application.Run(new Form1());*/

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                return;
            }

            try
            {
                RunAutomatedTest(args[1]);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
            Console.WriteLine(ex);
            }
        }

        private static void RunAutomatedTest(string v)
        {
            // ...
        }

        [DllImport("Kernel32.dll")]
        private static extern bool AttachConsole(int processId);
        }
    }
