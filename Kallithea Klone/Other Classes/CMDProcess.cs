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

        private readonly static Logger log = new Logger(typeof(CMDProcess));
        private readonly string arguments;
        private const string CmdExe = "cmd.exe";

        //  Properties
        //  ==========

        public int ExitCode { get; private set; } = -1;
        public ICollection<string> StandardOut { get; private set; } = new List<string>();
        public ICollection<string> ErrorOut { get; private set; } = new List<string>();

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

        //  Events
        //  ======

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                ErrorOut.Add(e.Data);
                log.Error(e.Data);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                StandardOut.Add(e.Data);
                log.Info(e.Data);
            }
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
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    ExitCode = process.ExitCode;
                }
            });
        }

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        public void ReportErrorsAsync(string action)
        {
            try
            {
                if (ErrorOut.Count > 0)
                {
                    throw new MainActionException($"Finished with the exit code: {ExitCode}" +
                        $"{Environment.NewLine}{Environment.NewLine}" +
                        $"And the error messages: {Environment.NewLine}" +
                        $"{string.Join(Environment.NewLine, ErrorOut.ToArray())}");
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