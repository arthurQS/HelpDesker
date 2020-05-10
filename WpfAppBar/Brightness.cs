using System;
using System.Linq;
using System.Management;
using System.Windows;

namespace WpfAppBar
{
    class Brightness : Window
    {
        int iCount = 0; //global counter for hiding/closing form after certian period of inactivity
        byte[] bLevels; //array of valid level values
        string[] arguments;
        //get the actual percentage of brightness
        static int GetBrightness()
        {
            //define scope (namespace)
            ManagementScope s = new ManagementScope("root\\WMI");

            //define query
            SelectQuery q = new SelectQuery("WmiMonitorBrightness");

            //output current brightness
            ManagementObjectSearcher mos = new ManagementObjectSearcher(s, q);

            ManagementObjectCollection moc = mos.Get();

            //store result
            byte curBrightness = 0;
            foreach (ManagementObject o in moc)
            {
                curBrightness = (byte)o.GetPropertyValue("CurrentBrightness");
                break; //only work on the first object
            }

            moc.Dispose();
            mos.Dispose();

            return (int)curBrightness;
        }

        //array of valid brightness values in percent
        static byte[] GetBrightnessLevels()
        {
            //define scope (namespace)
            ManagementScope s = new ManagementScope("root\\WMI");

            //define query
            SelectQuery q = new SelectQuery("WmiMonitorBrightness");

            //output current brightness
            ManagementObjectSearcher mos = new ManagementObjectSearcher(s, q);
            byte[] BrightnessLevels = new byte[0];

            try
            {
                ManagementObjectCollection moc = mos.Get();

                //store result


                foreach (ManagementObject o in moc)
                {
                    BrightnessLevels = (byte[])o.GetPropertyValue("Level");
                    break; //only work on the first object
                }

                moc.Dispose();
                mos.Dispose();

            }
            catch (Exception)
            {
                MessageBox.Show("Sorry, Your System does not support this brightness control...");

            }

            return BrightnessLevels;
        }

        static void SetBrightness(byte targetBrightness)
        {
            //define scope (namespace)
            ManagementScope s = new ManagementScope("root\\WMI");

            //define query
            SelectQuery q = new SelectQuery("WmiMonitorBrightnessMethods");

            //output current brightness
            ManagementObjectSearcher mos = new ManagementObjectSearcher(s, q);

            ManagementObjectCollection moc = mos.Get();

            foreach (ManagementObject o in moc)
            {
                o.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness }); //note the reversed order - won't work otherwise!
                break; //only work on the first object
            }

            moc.Dispose();
            mos.Dispose();
        }

        public void startup_brightness()
        {
            string sPercent = arguments[Array.FindIndex(arguments, item => item.Contains("%"))];
            if (sPercent.Length > 1)
            {
                int iPercent = Convert.ToInt16(sPercent.Split('%').ElementAt(0));
                if (iPercent >= 0 && iPercent <= bLevels[bLevels.Count() - 1])
                {
                    byte level = 100;
                    foreach (byte item in bLevels)
                    {
                        if (item >= iPercent)
                        {
                            level = item;
                            break;
                        }
                    }
                    SetBrightness(level);
                }

            }
        }
    }
}
