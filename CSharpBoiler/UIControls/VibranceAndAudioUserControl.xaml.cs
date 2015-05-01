using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Diagnostics;
using CSharpBoiler.AudioDeviceSwitcher;

namespace CSharpBoiler.UIControls
{
    /// <summary>
    /// Interaktionslogik für VibranceAndAudioUserControl.xaml
    /// </summary>
    public partial class VibranceAndAudioUserControl : UserControl
    {
        private VibranceProxy vibranceProxy;
        List<AudioDevice> audioDeviceList = new List<AudioDevice>();
        bool stillRunning = false;

        public VibranceAndAudioUserControl()
        {
            InitializeComponent();
        }

        private void VibranceAndAudioUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ThreadStart audioDevicesThreadStart = PopulateAudioDeviceCombobox;
                Thread audioDeviceThread = new Thread(audioDevicesThreadStart);
                audioDeviceThread.Start();
            }

            System.Runtime.InteropServices.Marshal.PrelinkAll(typeof(VibranceProxy));
        }

        public void CSGOIsRunning()
        {
            if (!stillRunning)
            {
                stillRunning = true;

                #region AquireData

                //Extract Data from the UI Thread
                bool isVibranceEnabled = false;
                bool isAudioEnabled = false;

                int refreshRate = 0;
                int inGameVibranceLevel = 0;
                int windowsVibranceLevel = 0;

                bool extractedValues = false;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    isVibranceEnabled = UseVibranceSettingsCheckBox.IsChecked == true;
                    isAudioEnabled = UseAudioSettingsCheckBox.IsChecked == true;
                    //Making sure the user won't accidentally DOS his own PC
                    refreshRate = Convert.ToInt32(RefreshRateTextBox.Text);
                    if (refreshRate < 100)
                    {
                        RefreshRateTextBox.Text = "100";
                        refreshRate = 100;
                    }

                    inGameVibranceLevel = (int) InGameVibranceLevelSlider.Value;
                    windowsVibranceLevel = (int) WindowsVibranceLevelSlider.Value;

                    extractedValues = true;
                }));

                while (!extractedValues)
                {
                    Thread.Sleep(100);
                }

                #endregion

                #region Audio

                //If Audio is enabled we want to switch the Audio Device when CSGO is running
                if (isAudioEnabled)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (InGameAudioDeviceComboBox.SelectedItem != null)
                        {
                            int audioDevice = ((AudioDevice) InGameAudioDeviceComboBox.SelectedItem).DeviceID;
                            AudioDeviceController.SetAudioDevice(audioDevice);
                        }
                    }));
                }

                #endregion

                #region Vibrance

                //If Vibrance is enabled we want to set the Vibrance when go is running
                if (isVibranceEnabled && windowsVibranceLevel != inGameVibranceLevel)
                {
                    ThreadStart vibranceThreadStart = () =>
                    {
                        //This blocks the Thread until CSGO is closed/minimised
                        //therefore we stick in in another Thread
                        vibranceProxy = new VibranceProxy();
                        if (vibranceProxy.VibranceInfo.isInitialized)
                        {
                            vibranceProxy.SetShouldRun(true);
                            vibranceProxy.SetKeepActive(false);
                            vibranceProxy.SetVibranceInGameLevel(inGameVibranceLevel);
                            vibranceProxy.SetVibranceWindowsLevel(windowsVibranceLevel);
                            vibranceProxy.SetSleepInterval(refreshRate);
                            vibranceProxy.HandleDvc();
                            vibranceProxy.UnloadLibraryEx();
                            stillRunning = false;
                        }
                    };

                    Thread vibranceThread = new Thread(vibranceThreadStart);
                    vibranceThread.Start();
                }

                #endregion
            }
        }

        public void CSGOWasRunning()
        {
            #region AquireData
            //Extract Data from the UI Thread
            bool isAudioEnabled = false;
            bool extractedValues = false;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                isAudioEnabled = UseAudioSettingsCheckBox.IsChecked == true;
                extractedValues = true;
            }));

            while (!extractedValues)
            {
                Thread.Sleep(100);
            }

            #endregion

            if (isAudioEnabled)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (WindowsAudioDeviceComboBox.SelectedItem != null)
                    {
                        int audioDevice = ((AudioDevice)WindowsAudioDeviceComboBox.SelectedItem).DeviceID;
                        AudioDeviceController.SetAudioDevice(audioDevice);
                    }
                }));
            }
        }

        

        private void PopulateAudioDeviceCombobox()
        {
            audioDeviceList = AudioDeviceController.GetAudioDevices();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                WindowsAudioDeviceComboBox.ItemsSource = audioDeviceList;
                InGameAudioDeviceComboBox.ItemsSource = audioDeviceList;
            }));
        }
    }
}
