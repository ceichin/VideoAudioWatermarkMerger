using System;
using System.IO;
using Foundation;

namespace WatermarkToVideoCreator.Helpers
{
    public static class FileSystemHelper
    {
        public static string RootDir
        {
            get
            {
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
                documentsPath = documentsPath.Replace("Documents/", "");
                return documentsPath.Replace("Documents", "");
            }
        }

        public static string DocumentsDir
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
				string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				#endif
                #endif
				return documentsPath;
			}
		}

        public static string TemporaryDir
        {
            get
            {
                return Path.GetTempPath();
            }
        }

        public static string LibraryDir
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

        public static string BundleDir
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

