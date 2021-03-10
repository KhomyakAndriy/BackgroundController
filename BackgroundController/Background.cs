using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundController
{
    public class Background
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
            UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        public void SetWallpaper(String path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public void SetNext()
        {
            var path = SettingsManager.ReadSetting<string>("BackgroundsFolder");
            var index = SettingsManager.ReadSetting<int>("LastIndex");
            var files = Directory.GetFiles(path);
            if (files.Length == 0)
            {
                return;
            }

            if (index >= files.Length)
            {
                index = 0;
            }

            Task.Factory.StartNew(() =>
            {
                SetWallpaper(files[index]);
                index++;
                SettingsManager.AddUpdateAppSettings("LastIndex", index);
            });
        }

        public void RestoreOriginal()
        {
            Task.Factory.StartNew(() =>
            {
                SetWallpaper(SettingsManager.ReadSetting<string>("OriginalBackground"));
            });
        }
    }
}
