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
 * The analyzer package takes the input from the eecutive package.
 * The input are the files to be processed and it sends the files
 * to the parser for analysis.
 * 
 * Public Interface
 * ================
 *
 * public void doAnalysis(string[] files)
 * 
/*
 * Build Process
 * =============
 * Required Files:
 *   Analyzer.cs
 *   
 * Build command:
 *   csc /D:TEST_ANA Analyzer.cs FileMgr.cs
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
   public class Analyzer
    {

       public void doAnalysis(string[] files)
       {

           // Parser p = new Parser();
           //p.parseFiles(files);

           //NEW DESIGN

           CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
           CSsemi.CSemiExp semi2 = new CSsemi.CSemiExp();
           BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
           Parser parser = builder.build();

           foreach (object file in files)
           {

               semi.displayNewLines = false;
               if (!semi.open(file as string))
               {
                   Console.Write("\n  Can't open {0}\n\n", file);
                   return;
               }

               // Console.Write("\n  Type and Function Analysis");
               // Console.Write("\n ----------------------------\n");

               //  Console.WriteLine("processing file{0}", file as string);




               try
               {
                   if (semi.getSemi())
                   {
                       semi2 = semi;
                       parser.parse(semi);
                   }
                   while (semi.getSemi())
                       parser.parse(semi);
                   //Console.Write("\n\n  locations table contains:");
               }
               catch (Exception ex)
               {
                   Console.Write("\n\n  {0}\n", ex.Message);
               }

               semi.close();

           }

           Repository rep = Repository.getInstance();
           List<Elem> table = rep.locations;


           Parser parser2 = builder.build2(rep, semi2);
           foreach (object file in files)
           {

               // CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
               semi2.displayNewLines = false;

               if (!semi2.open(file as string))
               {
                   Console.Write("\n  Can't open {0}\n\n", file);
                   return;
               }

               try
               {
                   do
                   {
                       parser2.parse(semi2);
                   } while (semi2.getSemi());

               }
               catch (Exception ex)
               {
                   Console.Write("\n\n  {0}\n", ex.Message);
               }
               //Repository rep = Repository.getInstance();

               semi2.close();

           }


           
       }

#if(TEST_ANA)
       static void Main(string[] args)
        {
          Console.Write("Testing FileMgr Class");
            Console.Write("\n======================\n");

            FileManager fm = new FileManager();
            fm.addPattern("*.cs");
            fm.findFiles("../../");
            List<string> files = fm.getFiles();
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Analyzer a=new Analyzer()
            doAnalysis(files);
            Console.ReadLine(); 

        }
#endif
    }
}
