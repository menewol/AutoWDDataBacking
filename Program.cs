using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace AutoWDDataBacking
{
    class Program
    {

        static void Main(string[] args)
        {
            DriveInfo sourceDrive = null, targetDrive;
            string deviceLabel = "";
            //read command args
            try
            {
                deviceLabel = args[0];
            }
            catch (Exception)
            {
                printDeviceNotFound();
            }
            //find source device to copy from
            sourceDrive = findDevice(deviceLabel);
            if (sourceDrive != null)
            {
                //check if target-hdd is plugged in
                targetDrive = findDevice("BackupFor_" + deviceLabel);
                if (targetDrive != null)
                {
                    //start copying/indexing files whatever.
                    Console.WriteLine("Indexing source drive...");
                    FileIndexingPrep(sourceDrive);
                    Console.WriteLine("Indexing target drive...");
                    FileIndexingPrep(targetDrive);
                }
                else
                {
                    printDeviceNotFound();
                }
            }
            else
            {
                printDeviceNotFound();
            }


            Console.Read();
        }

        private static DriveInfo findDevice(string backupDevice)
        {
            string label = "";
            DriveInfo[] drvs = System.IO.DriveInfo.GetDrives();
            foreach (DriveInfo item in drvs)
            {
                try
                {
                    label = item.VolumeLabel;
                    if (label == backupDevice)
                    {
                        Console.WriteLine("Found HDD: " + label);
                        return item;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return null;

        }

        private static void printDeviceNotFound()
        {
            Console.WriteLine("Specify a Device via commandline arguments or check your device label writing.");
            Console.WriteLine("Usage: AutoWDDataBacking.exe <VOLUME_LABEL>");
        }

        public static void ProcessDirectory(string targetDirectory, string systemLetterDrive)
        {
            try
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                    ProcessFile(fileName);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    if (subdirectory != (systemLetterDrive + "System Volume Information"))
                        ProcessDirectory(subdirectory, systemLetterDrive);
                    else
                        continue;
                }

            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ProcessFile(string path)
        {
            string retVal = "Processed file " + path;
            Console.WriteLine(retVal);
        }

        public static void FileIndexingPrep(DriveInfo targetDrive)
        {
            string path = targetDrive.Name;
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory(path, targetDrive.Name);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }
        }
    }
}
