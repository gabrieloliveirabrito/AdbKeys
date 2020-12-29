using System;
using System.Collections.Generic;
using System.Text;

namespace AdbKeys.Models
{
    public enum RootStatus
    {
        NotLoaded,
        Granted,
        Denied
    }

    public enum ChecksumState
    {
        Waiting,
        Equals,
        NotEquals
    }

    public partial class MainPageModel : BaseModel
    {
        private bool loaded = false;
        public bool Loaded
        {
            get => loaded;
            set => Set(ref loaded, value);
        }

        private string status = "Waiting..";
        public string Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private bool certOpened = false;
        public bool CertOpened
        {
            get => certOpened;
            set => Set(ref certOpened, value);
        }

        public string aboutApp = null;
        public string AboutApp
        {
            get => aboutApp;
            set => Set(ref aboutApp, value);
        }

        private bool executing;
        public bool Executing
        {
            get => executing;
            set => Set(ref executing, value);
        }

        private RootStatus rootStatus = RootStatus.NotLoaded;
        public RootStatus RootStatus
        {
            get => rootStatus;
            set => Set(ref rootStatus, value);
        }

        private string actualHash = "";
        public string ActualHash
        {
            get => actualHash;
            set => Set(ref actualHash, value);
        }

        private string openedHash = "";
        public string OpenedHash
        {
            get => openedHash;
            set => Set(ref openedHash, value);
        }

        private ChecksumState checksumState = ChecksumState.Waiting;
        public ChecksumState ChecksumState
        {
            get => checksumState;
            set => Set(ref checksumState, value);
        }

        public MainPageModel()
        {
            InitializeMethods();
        }
    }
}
