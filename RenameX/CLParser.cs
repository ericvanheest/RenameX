using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenameX
{
    public class CLParser
    {
        public string TempFile { get; set; }
        public string Editor { get; set; }
        public bool Recursive { get; set; }
        public bool Directories { get; set; }
        public bool ShowHelp { get; set; }
        public string Wildcard { get; set; }
        public bool Valid { get; set; }
        public bool Quiet { get; set; }

        public CLParser()
        {
            TempFile = "_renx.files";
            Editor = "notepad.exe";
            Recursive = false;
            Directories = false;
            ShowHelp = false;
            Wildcard = "*";
            Valid = false;
            Quiet = false;
        }

        enum NextOption { None, Editor, TempFile };
        enum NextArgs { None, Wildcard };

        public string Version()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductVersion;
        }

        public void Usage()
        {
            Console.WriteLine(String.Format(
"RenameX, version {0}\n" +
"\n" +
"Usage:    RenameX [options] [wildcard]\n" +
"\n" +
"Options:  -d         Rename directories instead of files\n" +
"          -e prog    Use 'prog' to edit files (default: {1})\n" + 
"          -s         Recurse directories\n" +
"          -t file    Use 'file' as the temporary file (default: {2})\n" +
"          -q         Quiet mode (do not print the list of renamed files)\n" +
"          -?         Show help text\n" +
"", Version(), Editor, TempFile));
        }

        public void Parse(string[] args)
        {
            NextOption nextOption = NextOption.None;
            NextArgs nextArg = NextArgs.Wildcard;

            Valid = true;

            foreach (string s in args)
            {
                switch (nextOption)
                {
                    case NextOption.None:
                        if (s[0] == '-' || s[0] == '/')
                        {
                            for (int i = 1; i < s.Length; i++)
                            {
                                switch (s[i])
                                {
                                    case 'e':
                                    case 'E':
                                        nextOption = NextOption.Editor;
                                        break;
                                    case 't':
                                    case 'T':
                                        nextOption = NextOption.TempFile;
                                        break;
                                    case 's':
                                    case 'S':
                                        Recursive = true;
                                        break;
                                    case 'd':
                                    case 'D':
                                        Directories = true;
                                        break;
                                    case 'q':
                                    case 'Q':
                                        Quiet = true;
                                        break;
                                    case 'h':
                                    case 'H':
                                    case '?':
                                        ShowHelp = true;
                                        break;
                                }
                            }
                        }
                        else switch (nextArg)
                            {
                                case NextArgs.Wildcard:
                                    Wildcard = s;
                                    nextArg = NextArgs.None;
                                    break;
                                default:
                                    Console.WriteLine("Warning: Ignoring extra command line parameters");
                                    break;
                            }
                        break;
                    case NextOption.Editor:
                        Editor = s;
                        nextOption = NextOption.None;
                        break;
                    case NextOption.TempFile:
                        TempFile = s;
                        nextOption = NextOption.None;
                        break;
                }
            }

            if (ShowHelp)
            {
                Usage();
                Valid = false;
                return;
            }
        }
    }
}
