using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace OverwatchPriorityService
{
    public partial class OverwatchPriorityService : ServiceBase
    {
        // TODO: does this need to be here? or just a var in onstart?
        private ProcessInfo _overwatchProcessInfo;

        public OverwatchPriorityService()
        {
            InitializeComponent();
            _eventLog = new EventLog();
            if (!EventLog.SourceExists("OverwatchPriorityServiceSource"))
            {
                EventLog.CreateEventSource(
                    "OverwatchPriorityServiceSource", "OverwatchPriorityServiceLog");
            }
            _eventLog.Source = "OverwatchPriorityServiceSource";
            _eventLog.Log = "OverwatchPriorityServiceLog";
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Hook overwatch events.
            _overwatchProcessInfo = new ProcessInfo("Overwatch.exe");
            _overwatchProcessInfo.Started +=
                new ProcessInfo.StartedEventHandler(this.OverwatchStarted);
            _overwatchProcessInfo.Terminated +=
                new ProcessInfo.TerminatedEventHandler(this.OverwatchTerminated);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            _eventLog.WriteEntry("Service started.");
        }

        protected override void OnStop()
        {
            // Update the service state to Stop Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            _eventLog.WriteEntry("Service stopped.");
        }

        private void OverwatchStarted(object sender, EventArgs e)
        {
            _eventLog.WriteEntry("Overwatch started.");

            // Set priority to High.
            Process[] processes = Process.GetProcessesByName("Overwatch");
            if (processes.Length != 0)
            {
                foreach (Process proc in processes)
                {
                    proc.PriorityClass = ProcessPriorityClass.High;
                    if (proc.PriorityClass != ProcessPriorityClass.High)
                    {
                        _eventLog.WriteEntry("Unable to set priority on process " + 
                            proc.ProcessName + ", ID=" + 
                            proc.Id.ToString() + "!");
                    }
                    else
                    {
                        _eventLog.WriteEntry("Overwatch priority set to High.");
                    }
                }
            }
            else
            {
                _eventLog.WriteEntry("Unable to find Overwatch process!");
            }
            
        }

        private void OverwatchTerminated(object sender, EventArgs e)
        {
            _eventLog.WriteEntry("Overwatch stopped.");
        }

        #region Windows Functions

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public uint dwServiceType;
            public ServiceState dwCurrentState;
            public uint dwControlsAccepted;
            public uint dwWin32ExitCode;
            public uint dwServiceSpecificExitCode;
            public uint dwCheckPoint;
            public uint dwWaitHint;
        };

        #endregion Windows Functions End

    }
}
