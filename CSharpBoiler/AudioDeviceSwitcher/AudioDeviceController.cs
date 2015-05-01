using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpBoiler.AudioDeviceSwitcher
{
     public static class AudioDeviceController
    {
        //Returns a List of all Available Audio Devices and their IDs
        public static List<AudioDevice> GetAudioDevices()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;

            //Finding all Audio Devices
            Process audioDeviceFinderProcess = Process.Start(new ProcessStartInfo()
            {
                FileName = currentPath + @"\" + "EndPointController.exe",
                Arguments = "",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                WorkingDirectory = ""
            });
            //Reading the Programm output to a string
            string outputString = audioDeviceFinderProcess.StandardOutput.ReadToEnd();

            //creating a List
            List<AudioDevice> audioDeviceList = new List<AudioDevice>();

            string[] devices = Regex.Split(outputString, "\r\n");

            //Using a Counter to get the Device ID;
            int i = 0;
            //Transforming the String into an AudioDevice and adding it to the AudioDeviceList
            foreach (var device in devices)
            {
                if (device == "")
                    continue;

                AudioDevice ad = new AudioDevice(i, device);
                audioDeviceList.Add(ad);
                i++;
            }

            return audioDeviceList;
        }

        //Setting the Default Audio Device to the given ID
        public static void SetAudioDevice(int newDeviceID)
        {

            Process.Start(new ProcessStartInfo()
            {
                FileName = "EndPointController.exe",
                Arguments = newDeviceID.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                WorkingDirectory = ""
            });
        }
    }
}
