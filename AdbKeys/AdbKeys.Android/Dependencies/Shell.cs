using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdbKeys.Dependencies;
using Java.Lang;
using System.Threading.Tasks;
using System.IO;
using Java.IO;

using SIO = System.IO;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(AdbKeys.Droid.Dependencies.Shell))]
namespace AdbKeys.Droid.Dependencies
{
    public class Shell : IShell
    {
        private Java.Lang.Process CreateSUProcess()
        {
            var builder = new ProcessBuilder("su");
            return builder.Start();
        }

        public override async Task ExecuteCommand(string[] cmd)
        {
            var builder = new ProcessBuilder(cmd);
            var process = builder.Start();

            await process.WaitForAsync();
        }

        public override async Task ExecuteRootCommand(string cmd)
        {
            using (var process = CreateSUProcess())
            using (var output = new DataOutputStream(process.OutputStream))
            {
                await output.WriteBytesAsync(cmd + "\n");
                await output.FlushAsync();

                await output.WriteBytesAsync("exit\n");
                await output.FlushAsync();

                await process.WaitForAsync();
            }
        }

        public override async Task<bool> CheckDeviceRooted()
        {
            try
            {
                using (var process = CreateSUProcess())
                using (var input = new DataInputStream(process.InputStream))
                using (var output = new DataOutputStream(process.OutputStream))
                {
                    await output.WriteBytesAsync("id\n");
                    await output.FlushAsync();

                    var id = await input.ReadLineAsync();
                    var res = id != null && id.Contains("uid=0");

                    await output.WriteBytesAsync("exit\n");
                    await output.FlushAsync();

                    await process.WaitForAsync();
                    return res;
                }
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> FileExists(string filename)
        {
            var exists = false;

            using (var process = CreateSUProcess())
            using (var input = new DataInputStream(process.InputStream))
            using (var output = new DataOutputStream(process.OutputStream))
            {
                await output.WriteBytesAsync($"[ -f \"{filename}\" ] && echo 1 || echo 0\n");
                await output.FlushAsync();

                await output.WriteBytesAsync("exit\n");
                await output.FlushAsync();

                var res = await input.ReadLineAsync();
                exists = res == "1";

                await process.WaitForAsync();
            }

            return exists;
        }

        public override async Task<Stream> OpenFile(string filename)
        {
            Stream outStream = null;
            
            using (var process = CreateSUProcess())
            using (var input = new DataInputStream(process.InputStream))
            using (var output = new DataOutputStream(process.OutputStream))
            {
                await output.WriteBytesAsync($"[ -f \"{filename}\" ] && echo 1 || echo 0\n");
                await output.FlushAsync();

                var exists = await input.ReadLineAsync();
                if (exists == "1")
                {
                    var tempDirectory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Android", "data", AppInfo.PackageName, "cache");
                    var outputFile = Path.Combine(tempDirectory, Path.GetFileName(filename));

                    if (!Directory.Exists(tempDirectory))
                        Directory.CreateDirectory(tempDirectory);

                    await output.WriteBytesAsync($"cp {filename} {outputFile}\n");
                    await output.FlushAsync();

                    outStream = new FileInfo(outputFile).Open(FileMode.Open);
                    
                }

                await output.WriteBytesAsync("exit\n");
                await output.FlushAsync();

                await process.WaitForAsync();
            }

            return outStream;
        }
    }
}