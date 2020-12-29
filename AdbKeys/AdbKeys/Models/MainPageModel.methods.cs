using AdbKeys.Dependencies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AdbKeys.Models
{
    public partial class MainPageModel
    {
        const string targetFilePath = "/data/misc/adb/adb_keys";
        private string pubFilePath = "";

        public AsyncCommand<EventArgs> InitCommand { get; set; }
        public AsyncCommand<EventArgs> OpenFileCommand { get; set; }
        public AsyncCommand<EventArgs> CopyToFolderCommand { get; set; }
        public AsyncCommand<EventArgs> CalculateActualMD5Command { get; set; }
        public AsyncCommand<EventArgs> CalculateOpenedMD5Command { get; set; }
        public RelayCommand<EventArgs> CompareHashesCommand { get; set; }

        protected void InitializeMethods()
        {
            InitCommand = new AsyncCommand<EventArgs>(Initialize);
            OpenFileCommand = new AsyncCommand<EventArgs>(OpenFile);
            CopyToFolderCommand = new AsyncCommand<EventArgs>(CopyToFolder, _ => CertOpened);
            CalculateActualMD5Command = new AsyncCommand<EventArgs>(CalculateActualMD5);
            CalculateOpenedMD5Command = new AsyncCommand<EventArgs>(CalculateOpenedMD5, _ => CertOpened);
            CompareHashesCommand = new RelayCommand<EventArgs>(CompareHashes, _ => ActualHash != "Waiting.." && OpenedHash != "Waiting..");
        }

        private async Task Initialize(EventArgs _)
        {
            Status = "Waiting..";
            CertOpened = false;
            Loaded = true;
            AboutApp = "About App";
            ActualHash = "Waiting..";
            OpenedHash = "Waiting..";

            await Task.Delay(1500);

            while (await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();

                if (status == PermissionStatus.Denied)
                {
                    ISnackBar.Dependency.ShowSnackbar("Please accept the permission request!");
                }
            }

            while (await Permissions.CheckStatusAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                if (status == PermissionStatus.Denied)
                {
                    ISnackBar.Dependency.ShowSnackbar("Please accept the permission request!");
                }
            }

            RootStatus = await IShell.Dependency.CheckDeviceRooted() ? RootStatus.Granted : RootStatus.Denied;

            /*if (RootStatus == RootStatus.Granted)
            {
                await IShell.Dependency.ExecuteRootCommand("mount -o rw, remount, rw /");
                await IShell.Dependency.ExecuteRootCommand("mount -o rw, remount, rw /data");
                await IShell.Dependency.ExecuteRootCommand("mount -o rw, remount, rw /system");
            }*/
        }

        private async Task OpenFile(EventArgs _)
        {
            var res = await FilePicker.PickAsync();

            if (!string.IsNullOrEmpty(res.FullPath))
            {
                pubFilePath = res.FullPath;
                CertOpened = true;
            }
            else
            {
                pubFilePath = string.Empty;
                CertOpened = false;
            }

            OpenedHash = "Waiting..";
            ChecksumState = ChecksumState.Waiting;
        }

        private async Task CopyToFolder(EventArgs _)
        {
            Executing = true;
            await CalculateActualMD5(EventArgs.Empty);
            await CalculateOpenedMD5(EventArgs.Empty);

            if (OpenedHash == ActualHash)
            {
                Status = "Status: Opened and Actual keys are equals. No change needed!";
            }
            else
            {
                if (await IShell.Dependency.FileExists(targetFilePath))
                    await IShell.Dependency.ExecuteRootCommand($"rm {targetFilePath}");
                await IShell.Dependency.ExecuteRootCommand($"cp {pubFilePath} {targetFilePath}");

                await CalculateActualMD5(EventArgs.Empty);

                if (ChecksumState == ChecksumState.Equals)
                    Status = "Status: Keys changed successfully! Try to connect to ADB";
                else if (ChecksumState == ChecksumState.NotEquals)
                    Status = "Status: Failed to change adb_keys";
            }

            CompareHashes(EventArgs.Empty);

            Executing = false;
        }

        private async Task CalculateActualMD5(EventArgs arg)
        {
            using (var stream = await IShell.Dependency.OpenFile(targetFilePath))
            {
                if (stream == null)
                    ActualHash = "Not exists";
                else
                {
                    using (var md5 = MD5.Create())
                    {
                        var buffer = new byte[stream.Length];
                        var readed = await stream.ReadAsync(buffer, 0, buffer.Length);

                        ActualHash = BitConverter.ToString(md5.ComputeHash(buffer, 0, readed)).Replace("-", "");
                    }
                }
            }
        }

        private async Task CalculateOpenedMD5(EventArgs arg)
        {
            using (var stream = File.OpenRead(pubFilePath))
            {
                if (stream == null)
                    OpenedHash = "Not exists";
                else
                {
                    using (var md5 = MD5.Create())
                    {
                        var buffer = new byte[stream.Length];
                        var readed = await stream.ReadAsync(buffer, 0, buffer.Length);

                        OpenedHash = BitConverter.ToString(md5.ComputeHash(buffer, 0, readed)).Replace("-", "");
                    }
                }
            }
        }

        private void CompareHashes(EventArgs _)
        {
            if (CompareHashesCommand.CanExecute(null))
                ChecksumState = OpenedHash == ActualHash ? ChecksumState.Equals : ChecksumState.NotEquals;
            else
                ChecksumState = ChecksumState.Waiting;
        }
    }
}