/////////////////////////////////////////////////////////////////////////
// FileMgr.cs  -  Package to retrieve the files based on the path       //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Lenovo V570, Win 7, SP 1                              //
// Application: Pr#2  CSE681, Fall 2014                               //
// Author:     Tanmay Fadnavis, Syracuse University,                  //
//                 tfadnavi@syr.edu                                  //
/////////////////////////////////////////////////////////////////////////
/*
 * Package Operations
 * ==================
 * The File Manager takes input as path,pattern and
 * a recurse option. It retrieves the files from the specified path.
 * If no path is specified,default is used. It retrieves all the files which 
 * matches the file pattern. If no pattern is specified, default is used.
 * Recurse value if it is true,it searches for files recursively
 * in the current directory.
 * 
 * Public Interface
 * ================
 *
 * public void findFiles(path,pattern,recurse)
 * public void addPattern(string pattern)
 * public string[] getFiles()
/*
 * Build Process
 * =============
 * Required Files:
 *   FileMgr.cs
 *   
 * Build command:
 *   csc /D:TEST_FILEMGR FileMgr.cs 
 *
 * 
 * Maintenance History
 * ===================
 * ver 1.0 : Oct 2014
 *   - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DepnServer
{
    public class FileManager
    {

        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        //private bool recurse = true;

        public void findFiles(string path,List<string> patterns,bool recurse)
        {
            if (patterns.Count == 0)
                addPattern("*.*");
            foreach (string pattern in patterns)
            {
                
                string[] newFiles = Directory.GetFiles(path, pattern);
                for (int i = 0; i < newFiles.Length; i++)
                    newFiles[i] = Path.GetFullPath(newFiles[i]);
                files.AddRange(newFiles);
            } 
            if (recurse)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                    findFiles(dir,patterns,recurse);
            }

        }

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        } 

        public List<string> getFiles()
        {
            return files;
        } 
 
#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("Testing FileMgr Class");
            Console.Write("\n======================\n");

            FileManager fm = new FileManager();
            fm.addPattern("*.*");
            fm.findFiles("../../");
            List<string> files = fm.getFiles();
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Console.ReadLine(); 
        }
#endif
    }
}
