/////////////////////////////////////////////////////////////////////////
// Executive.cs  -  Entry package to Code Analyzer                    //
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
 * The Executive package via the class Executive, provides the entry point
 * for the code analyzer. It communicates with CommandLineParser package,
 * FileManager package,Analyzer Package,Display Package and XML processor
 * Package. It communicates the output of one package to the other.
 * As it is the first package to be executed, it won't have a test stub
 * 
 *
/*
 * Build Process
 * =============
 * Required Files:
 *   Executive.cs, CmdLine.cs, FileMgr.cs,Analyzer.cs,Display.cd,XMLProcessor.cs
 * 
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel.Channels;


namespace DepnServer
{
    class Executive
    {


        

        private string path;
        private List<string> pattern=new List<string>();
        private List<string> options=new List<string>();
        private bool recurse;
        private bool relationship;
        private bool xmlProcessing;

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public bool Relationship
        {
            get
            {
                return relationship;
            }
            set
            {
                relationship = value;
            }
        }

        public bool XMLProcessing
        {
            get
            {
                return xmlProcessing;
            }
            set
            {
                xmlProcessing = value;
            }
        }
        public  bool Recurse
        {
            get
            {
                return recurse;
            }

            set
            {
                recurse = value;
            }
        }
        static public string[] getFiles(string path, List<string> patterns,bool recurse)
        {
            FileManager fm = new FileManager();
            //foreach (string p in patterns)
              //  fm.addPattern(p);

            fm.findFiles(path,patterns,recurse);
            return fm.getFiles().ToArray();

        }

            

        static void Main(string[] args)
        {
            Executive e = new Executive();
            
            
                try
                {

                    CommandLineParser argument = new CommandLineParser();

                  //  Console.WriteLine("testing ");
                   // Console.WriteLine("Command Line Argument is = \n" + args);
                    argument.splitArgument(args, out e.path, out e.pattern, out e.options);

                   // Console.WriteLine("path is {0}", e.path);
                    if (e.path.Length == 0)
                        e.path = "../../";
                    if (e.pattern.Count == 0)
                        e.pattern.Add("*.cs");

                    

                   // Console.WriteLine("File Path is {0}\nFile Pattern is {1}\nOptions are{2}", e.Path, e.pattern, e.options);

                }

                catch (Exception exp)
                {
                    Console.WriteLine("There is an error in command line {0} \n", args);
                    Console.WriteLine("\n Error Message {0} \n \n", exp.Message);
                }
            
            
            e.Recurse=e.options.Contains("/S");
            e.Relationship = e.options.Contains("/R");
            e.XMLProcessing = e.options.Contains("/X");
                

            
            string[] files = Executive.getFiles(e.path,e.pattern,e.Recurse);

          
           

            Analyzer ana = new Analyzer();

            Display d = new Display();
                
           
                 ana.doAnalysis(files);
                 //d.display(files,e.Relationship);

                 TypeBuilder tb = new TypeBuilder();
                 tb.addTypes();

                 Repository r = Repository.getInstance();
                 

                 foreach (KeyValuePair<string,TypeElem> k in r.typeTable)
                 {
                     Console.WriteLine(" {0} and {1}", k.Key, k.Value);
                 }
                     

             if (e.XMLProcessing)
             {
                 XMLProcessor xp = new XMLProcessor();

                 xp.process(e.Relationship);
             }

            Console.ReadLine();
        }

        
    }
}
