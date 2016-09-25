/////////////////////////////////////////////////////////////////////////
// CmdLine.cs  -  Package to parse command line arguments                  //
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
 * The display package displays the output after the code analysis.
 * If there is an option to display the relationships,it will display 
 * the relationships too,else it just displays function sizes and complexities
 * and files which are being parsed.
 * 
 * Public Interface
 * ================
 *
 * public void display(string[] files,bool relationship)
 * 
/*
 * Build Process
 * =============
 * Required Files:
 *  Display.cs Analyzer.cs
 *   
 * Build command:
 *   csc /D:TEST_DISPLAY Display.cs Analyzer.cs
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

namespace DepnServer
{
   public class Display
    {

        public void display(string[] files,bool relationship)
        {

            Repository rep = Repository.getInstance();
            List<Elem> table = rep.locations;
                
                
            //DISPLAY FILES IN THE FILE SET
            Console.WriteLine("Files in fileset are \n");
            foreach(object file in files)
            {
                Console.WriteLine(file as string);
            }

             

                // COUNT FOR COMPLEXITY AND FUNCTION SIZES

                List<Elem> temp = rep.locations;

                foreach(Elem e in table)
                {
                    if(e.type.Equals("namespace"))
                    {
                        Console.WriteLine("\n{0,9},{1,10}, starts at line{2,5},end at line{3,5}", e.type, e.name, e.begin, e.end);
                    }

                    if (e.type.Equals("inheritance"))
                    {
                        Console.WriteLine("\n{0,10},{1,10},starts at line{2,5},ends at line{3,5}", e.type, e.name, e.begin, e.end);
                    }

                    //search for the namespace for that particular class

                    if(e.type.Equals("class"))
                    {
                        foreach(Elem tt in temp)
                        {
                            if(tt.type.Equals("namespace"))
                            {
                                if((tt.begin < e.begin) && (tt.end > e.end))
                                {
                                    Console.WriteLine("\n{0,5},{1,10}.{2,5},starts at line{3,5},ends at line{4,5}", e.type, tt.name, e.name, e.begin, e.end);
                                }
                            }
                        }


                    }
                    if(e.type.Equals("function"))
                    {
                        int size = e.end - e.begin;
                        int complexity=0;
                        foreach(Elem t in temp)
                        {
                            if(t.type.Equals("control") && (t.begin > e.begin) && (t.end < e.end))
                                complexity++;
                            if (t.type.Equals("braceless") && (t.begin > e.begin) && (t.end < e.end))
                                complexity++;

                        }

                        Console.WriteLine("\nFunction {0,10}, size={1,3} complexity={2,3}", e.name, size, complexity);

                    } 

                } 

                //DISPLAY RELATIONSHIPS

                
               //Console.Write("\n\n  relationship table contains:");
              /*  List<RelElem> table2 = rep.inheritance;
                
               foreach (RelElem e in table2)
                {
                    Console.Write("\n  {0,10}, {1,25},{2,20}, {3,5}, {4,5}", e.type, e.name, e.withName, e.beginRel, e.endRel);
                }
                Console.WriteLine();
                Console.Write("\n\n  That's all folks!\n\n");
                Console.ReadLine(); */
                
                //DISPLAY FINAL RELATIONSHIPS
                if (relationship)
                {
                    List<RelElem> table2 = rep.inheritance;

                    List<RelElem> temp2 = rep.inheritance;

                    foreach (Elem ee in table)
                    {
                        if (ee.type.Equals("class"))
                        {
                            foreach (RelElem rr in table2)
                            {
                                if ((rr.beginRel >= ee.begin) && (rr.endRel <= ee.end) && (rr.type != "UsingTemp") && (rr.type !="UsingStr"))
                                {
                                    Console.WriteLine("\nclass {0} has {1} relationship with {2} at line {3}", ee.name, rr.type, rr.withName,rr.beginRel);
                                }
                            }
                        }
                    }
                } 
        }

#if(TEST_DISPLAY)
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
            Analyzer a=new Analyzer()
            doAnalysis(files);
            Display d=new Display();
            d.display(files,relationships);
            Console.ReadLine(); 
        }
#endif
    }

}