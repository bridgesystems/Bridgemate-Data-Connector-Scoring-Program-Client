using System;
using System.Collections.Generic;
using SIO=System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using NLog;

namespace BridgeSystems.BCS.Net.Application.Common.DataConnector
{
    
    public static class DataConnectorManager
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The name of the executable of the Bridgemate DataConnectorService
        /// </summary>
        public const string FullDataConnectorName = "BridgeSystems.Bridgemate.DataConnectorService.exe";

        /// <summary>
        /// Restarts the Bridgemate Data Connector if it is not running.
        /// </summary>
        /// <param name="dataconnectorExePath"></param>
        /// <param name="forceRestart"></param>
        /// <returns></returns>
        public static bool EnsureDataConnectorServiceIsRunning(string dataconnectorExePath,bool forceRestart)
        {
            try
            {
                if (forceRestart)
                    return Restart(force:true);
                else
                {
                    Process process = GetProcess(Path.GetFileNameWithoutExtension(dataconnectorExePath));
                    if (process != null) return true;

                    return Restart();
                }
            }
            catch
            {
                return false;
            }

            bool Restart(bool force = false)
            {
                return StartProcess(dataconnectorExePath, $"-i{FullDataConnectorName} {(force?"-c":"")}");
            }
        }

        public static bool StartProcess(string path, string parameters = "", string workingDirectory = null)
        {
            var process = new Process();
            try
            {
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = workingDirectory ?? System.IO.Path.GetDirectoryName(path);
                process.StartInfo.Arguments = parameters;
                process.Start();
                //process.WaitForInputIdle();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public static Process GetProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0) return null;
            return processes[0];
        }
    }

}
