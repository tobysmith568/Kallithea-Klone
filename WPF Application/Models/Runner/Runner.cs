using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Runner
{
    public class Runner : IRunner
    {
        //  Constants
        //  =========

        private const string CMDEXE = "cmd.exe";

        //  Methods
        //  =======

        public async Task<IRunResult> Run(string repositoryLocation, params string[] commands)
        {
            repositoryLocation = repositoryLocation ?? throw new ArgumentNullException(nameof(repositoryLocation));
            commands = commands ?? throw new ArgumentNullException(nameof(commands));

            if (!Directory.Exists(repositoryLocation))
            {
                throw new ArgumentException("Not a valid directory", nameof(repositoryLocation));
            }

            if (commands.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(commands));
            }

            string arguments = "/C" + string.Join("&", commands);

            try
            {
                return await Task.Run(() => RunProcess(repositoryLocation, arguments));
            }
            catch (Exception e)
            {
#warning TODO LOG
#warning TODO MESSAGE BOX

                return new RunResult()
                {
                    ExitCode = -1
                };
            }
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="SystemException"></exception>
        private IRunResult RunProcess(string location, string arguments)
        {
            IRunResult result = new RunResult();

            void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                result.AllOut.Add(e.Data);
                result.StandardOut.Add(e.Data);
            }

            void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
            {
                result.AllOut.Add(e.Data);
                result.ErrorOut.Add(e.Data);
            }

            using (Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = location,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = CMDEXE,
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

                result.ExitCode = process.ExitCode;
            }

            return result;
        }
    }
}