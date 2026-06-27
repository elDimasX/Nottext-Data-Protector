using System;
using System.IO;

namespace Nottext_Data_Protector.Services
{
    public static class OnboardingService
    {
        private const string FileName = "pp.welcome";

        private static string PrimaryMarkerPath => Path.Combine(Global.pasta, FileName);

        private static string FallbackMarkerPath
        {
            get
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Nottext Data Protector");

                return Path.Combine(dir, FileName);
            }
        }

        public static bool HasCompletedOnboarding()
        {
            try
            {
                if (File.Exists(PrimaryMarkerPath))
                    return true;
            }
            catch { }

            try
            {
                if (File.Exists(FallbackMarkerPath))
                    return true;
            }
            catch { }

            return false;
        }

        public static void MarkCompleted()
        {
            string content = "completed=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                Directory.CreateDirectory(Global.pasta);
                File.WriteAllText(PrimaryMarkerPath, content);
                return;
            }
            catch { }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FallbackMarkerPath));
                File.WriteAllText(FallbackMarkerPath, content);
            }
            catch { }
        }
    }
}
