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
        private readonly Process process;

        //  Properties
        //  ==========

        public int ExitCode => process.ExitCode;

        //  Constructors
        //  ============

        public CMDProcess(string command) : this(new string[] { command })
        {

        }

        public CMDProcess(string[] commands)
        {
            if (commands.Length < 1)
                throw new ArgumentException("The commands parameter cannot be empty");

            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = CmdExe,
                    Arguments = "/C " + string.Join("&", commands)
                }
            };           
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
                process.Start();
                process.WaitForExit();
            });
        }

        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> GetStandardOutAsync()
        {
            return await process.StandardOutput.ReadToEndAsync();
        }

        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> GetErrorOutAsync()
        {
            return await process.StandardError.ReadToEndAsync();
        }
    }
}
