using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AudioDeviceSwitcher;
using System.Diagnostics;

namespace CSharpBoiler.UIControls
{
    /// <summary>
    /// Interaktionslogik für VibranceAndAudioUserControl.xaml
    /// </summary>
    public partial class VibranceAndAudioUserControl : UserControl
    {
        private VibranceProxy vibranceProxy;
        List<AudioDevice> audioDeviceList = new List<AudioDevice>();

        public VibranceAndAudioUserControl()
        {
            InitializeComponent();
        }

        private void VibranceAndAudioUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            PopulateAudioDeviceCombobox();
            System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(VibranceProxy));

            ThreadStart monitorCsgotThreadStart = MonitorCSGO;
            Thread monitorCSGOThread = new Thread(monitorCsgotThreadStart);
            monitorCSGOThread.Start();
        }

        public void MonitorCSGO()
        {
            bool wasRunning = false;

            while (true)
            {
                Process[] pname = Process.GetProcessesByName("csgo");

                int refreshRate = 0;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    refreshRate = Convert.ToInt32(RefreshRateTextBox.Text);
                }));

                if (pname.Length != 0)
                {
                    int inGameVibranceLevel = 0;
                    int windowsVibranceLevel = 0;
                    wasRunning = true;
                    bool extractedValues = false;

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        inGameVibranceLevel = (int)InGameVibranceLevelSlider.Value;
                        windowsVibranceLevel = (int) WindowsVibranceLevelSlider.Value;

                        extractedValues = true;

                        if (InGameAudioDeviceComboBox.SelectedItem != null)
                        {
                            int audioDevice = ((AudioDevice) InGameAudioDeviceComboBox.SelectedItem).DeviceID;
                            AudioDeviceController.SetAudioDevice(audioDevice);
                        }
                    }));

                    while (!extractedValues)
                    {
                        Thread.Sleep(100);
                    }

                    if (windowsVibranceLevel == inGameVibranceLevel)
                        continue;

                    vibranceProxy = new VibranceProxy();
                    if (vibranceProxy.VibranceInfo.isInitialized)
                    {
                        vibranceProxy.SetShouldRun(true);
                        vibranceProxy.SetKeepActive(false);
                        vibranceProxy.SetVibranceInGameLevel(inGameVibranceLevel);
                        vibranceProxy.SetVibranceWindowsLevel(windowsVibranceLevel);
                        vibranceProxy.SetSleepInterval(refreshRate);
                        vibranceProxy.HandleDvc();
                        bool unload = vibranceProxy.UnloadLibraryEx();
                    }
                }

                if (pname.Length == 0 && wasRunning)
                {
                    wasRunning = false;

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (WindowsAudioDeviceComboBox.SelectedItem != null)
                        {
                            int audioDevice = ((AudioDevice)WindowsAudioDeviceComboBox.SelectedItem).DeviceID;
                            AudioDeviceController.SetAudioDevice(audioDevice);
                        }
                    }));
                }

                Thread.Sleep(refreshRate != 0 ? refreshRate : 5000);
            }
        }

        private void PopulateAudioDeviceCombobox()
        {
            List<AudioDevice> audioDeviceList = AudioDeviceController.GetAudioDevices();

            WindowsAudioDeviceComboBox.ItemsSource = audioDeviceList;
            InGameAudioDeviceComboBox.ItemsSource = audioDeviceList;
        }
    }
}
