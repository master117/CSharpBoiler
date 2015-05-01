namespace CSharpBoiler.AudioDeviceSwitcher
{
    public class AudioDevice
    {
        public int DeviceID { get; set; }
        public string DeviceName { get; set; }

        public AudioDevice(int tempDeviceID, string tempDeviceName)
        {
            DeviceID = tempDeviceID;
            DeviceName = tempDeviceName;
        }

        public override string ToString()
        {
            return DeviceName;
        }
    }
}
