using System;
using System.IO;
using Foundation;

namespace Core.Helpers
{
    public static class FSHelper
    {
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public static string ROOT_DIR
        {
            get
            {
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
                // Both cases just in case, to prevent chaos
                documentsPath = documentsPath.Replace("Documents/", "");
                return documentsPath.Replace("Documents", "");
            }
        }

        public static string TEMP_DIR
        {
            get
            {
                return System.IO.Path.GetTempPath();
            }
        }

        public static string LIBRARY_DIR
        {
            get
            {
                #if SILVERLIGHT
                #else
                #if __ANDROID__
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                #else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
                string libraryPath = Path.Combine (documentsPath, "../Library/");
                #endif
                #endif
                return libraryPath;
            }
        }

        public static string DOCS_DIR
        {
            get
            {
                #if SILVERLIGHT
                #else
                #if __ANDROID__
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                #else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
                #endif
                #endif
                return documentsPath;
            }
        }

        public static string BUNDLE_DIR
        {
            get
            {
                return NSBundle.MainBundle.BundlePath;
            }
        }

        public static void CleanDir(string dirUrl)
        {
            DirectoryInfo dir = new DirectoryInfo(dirUrl);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete(); 
            }

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                dir.Delete(true); 
            }
        }

    }
}

