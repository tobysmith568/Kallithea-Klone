using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone.Other_Classes
{
    public class CMDProcess
    {
        //  Variables
        //  =========

        private const string CmdExe = "cmd.exe";
        private readonly string arguments;

        //  Properties
        //  ==========

        public int ExitCode { get; private set; } = -1;
        public string StandardOut { get; private set; } = "";
        public string ErrorOut { get; private set; } = "";

        //  Constructors
        //  ============

        public CMDProcess(string command) : this(new string[] { command })
        {

        }

        public CMDProcess(string[] commands)
        {
            if (commands.Length < 1)
                throw new ArgumentException("The commands parameter cannot be empty");

            arguments = "/C " + string.Join("&", commands);
        }

        //  Methods
        //  =======

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="SystemException"></exception>
        public async Task Run()
        {
            await Task.Run(() =>
            {
                using (Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = CmdExe,
                        Arguments = arguments
                    }
                })
                {
                    process.Start();
                    process.WaitForExit();
                    ExitCode = process.ExitCode;
                    StandardOut += process.StandardOutput.ReadToEnd();
                    ErrorOut += process.StandardError.ReadToEnd();
                }
            });
        }

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        public void ReportErrorsAsync(string action)
        {
            try
            {
                if (ErrorOut.Length > 0)
                {
                    throw new MainActionException($"Finished with the exit code: {ExitCode}\n\n" +
                        $"And the error messages: {ErrorOut}");
                }
            }
            catch (Exception e) when (!(e is MainActionException))
            {
                throw new MainActionException($"Unable to read the process used for {action}. This means Kallithea" +
                    " Klone is unable to tell if it was successful or not.", e);
            }
        }
    }
}
