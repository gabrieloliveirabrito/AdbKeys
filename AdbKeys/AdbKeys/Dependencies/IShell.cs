using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AdbKeys.Dependencies
{
    public abstract class ProcessOutputEventArgs : EventArgs
    {
        public string Output { get; set; }

        public ProcessOutputEventArgs(string output)
        {
            Output = output;
        }
    }

    public abstract class IShell : BaseDependency<IShell>
    {

        public abstract Task ExecuteCommand(params string[] cmd);
        public abstract Task ExecuteRootCommand(string cmd);
        public abstract Task<bool> CheckDeviceRooted();

        /// <summary>
        /// Check if file exists from shell command
        /// </summary>
        /// <param name="filename">The file path to check</param>
        /// <returns>Exists or not/true or false</returns>
        public abstract Task<bool> FileExists(string filename);

        /// <summary>
        /// Copy file from root system to temp folder
        /// </summary>
        /// <param name="filename">File path on the root system (ex /data/misc/adb/adb_keys)</param>
        /// <returns>File Stream of temp file</returns>
        public abstract Task<Stream> OpenFile(string filename);
    }
}
