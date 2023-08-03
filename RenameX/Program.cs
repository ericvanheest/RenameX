using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace RenameX
{
    class Program
    {
        static int StringReverse(string x, string y)
        {
            return String.Compare(y, x);
        }
    
        static void Main(string[] args)
        {
            CLParser parser = new CLParser();
            parser.Parse(args);

            if (!parser.Valid)
                return;

            string[] strOrigFiles;
            if (parser.Directories)
            {
                List<string> dirs = new List<string>(Directory.GetDirectories(".", parser.Wildcard, parser.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                dirs.Sort(StringReverse);
                strOrigFiles = dirs.ToArray();
            }
            else
                strOrigFiles = Directory.GetFiles(".", parser.Wildcard, parser.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            string[] strOrigFilesNoPath = new string[strOrigFiles.Length];
            for (int i = 0; i < strOrigFiles.Length; i++)
            {
                strOrigFilesNoPath[i] = Path.GetFileName(strOrigFiles[i]);
            }
            File.WriteAllLines(parser.TempFile, strOrigFilesNoPath, Encoding.Unicode);
            Process procEditor = Process.Start(parser.Editor, parser.TempFile);
            procEditor.WaitForExit();
            string[] strNewFiles = File.ReadAllLines(parser.TempFile, Encoding.Unicode);
            //bool bIgnoreLast = false;
            if (strOrigFilesNoPath.Length != strNewFiles.Length)
            {
                if ((strNewFiles.Length == strOrigFilesNoPath.Length + 1) && strNewFiles[strNewFiles.Length - 1] == "")
                {
                    // bIgnoreLast = true;
                }
                else
                {
                    Console.WriteLine("Error: file list lengths differ (original: {0}, new: {1})", strOrigFilesNoPath.Length, strNewFiles.Length);
                    return;
                }
            }

            for (int i = 0; i < strOrigFiles.Length; i++)
            {
                string strSource = strOrigFiles[i];
                string strDest = Path.Combine(Path.GetDirectoryName(strSource), strNewFiles[i]);
                try
                {
                    if (parser.Directories)
                    {
                        if (Directory.Exists(strDest))
                        {
                            if (!parser.Quiet)
                                Console.WriteLine("Already exists: {0}", strNewFiles[i]);
                        }
                        else if (!Directory.Exists(strSource))
                        {
                            Console.WriteLine("Error: Could not find source directory: {0}", strOrigFiles[i]);
                        }
                        else
                        {
                            Directory.Move(strSource, strDest);
                            if (!parser.Quiet)
                                Console.WriteLine("{0} => {1}", strOrigFilesNoPath[i], strNewFiles[i]);
                        }
                    }
                    else
                    {
                        if (File.Exists(strDest))
                        {
                            if (!parser.Quiet)
                                Console.WriteLine("Already exists: {0}", strNewFiles[i]);
                        }
                        else if (!File.Exists(strSource))
                        {
                            Console.WriteLine("Error: Could not find source file: {0}", strOrigFiles[i]);
                        }
                        else
                        {
                            File.Move(strSource, strDest);
                            if (!parser.Quiet)
                                Console.WriteLine("{0} => {1}", strOrigFilesNoPath[i], strNewFiles[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error moving {0} => {1} : {2}", strOrigFilesNoPath[i], strNewFiles[i], ex.Message);
                }
            }

            File.Delete(parser.TempFile);
        }
    }
}
