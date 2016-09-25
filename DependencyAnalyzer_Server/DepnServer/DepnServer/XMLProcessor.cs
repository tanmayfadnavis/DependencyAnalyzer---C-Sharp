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
 * The XML processor package will process the output
 * and make an XML file and show the output in 
 * an XML format. Similarly to the display package,
 * if the option to show relationships are provided,
 * it will generate another xml file showing relationships.
 * 
 * Public Interface
 * ================
 *
 * public void process(bool relationship)
 * 
/*
 * Build Process
 * =============
 * Required Files:
 *  XMLProcessor.cs Analyzer.cs
 *   
 * Build command:
 *   csc /D:TEST_XML XMLProcessor.cs Analyzer.cs
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
using System.Xml;


namespace DepnServer
{
   public class XMLProcessor
    {

       public void process(bool relationship)
       {

           XmlDocument doc1 = new XmlDocument();
           doc1.LoadXml(" <codeAnalysis>   </codeAnalysis> ");

           XmlNode root = doc1.DocumentElement;

           Repository rep = Repository.getInstance();
           List<Elem> table = rep.locations;
           List<Elem> temp = rep.locations;

           //FOR DISPLAYING NAMESCAPE,CLASSES AND FUNCTIONS

           foreach(Elem e in table)
           {

               if(e.type.Equals("namespace"))
               {
                   //Create a new node.
                   XmlElement nameNode = doc1.CreateElement("namespace");
                   nameNode.SetAttribute("name", e.name);
                   root.AppendChild(nameNode);
               }

               if (e.type.Equals("inheritance"))
               {

                   XmlElement inhertNode = doc1.CreateElement("inheritance");
                   inhertNode.SetAttribute("name", e.name);
                   root.AppendChild(inhertNode);
               }

               if (e.type.Equals("class"))
               {

                   XmlElement classNode = doc1.CreateElement("class");
                    classNode.SetAttribute("name", e.name);
                    root.AppendChild(classNode);
               }

               if (e.type.Equals("function"))
               {
                   int size = e.end - e.begin;
                   int complexity = 0;
                   foreach (Elem t in temp)
                   {
                       if (t.type.Equals("control") && (t.begin > e.begin) && (t.end < e.end))
                           complexity++;
                       if (t.type.Equals("braceless") && (t.begin > e.begin) && (t.end < e.end))
                           complexity++;

                   }

                   XmlElement funtNode = doc1.CreateElement("function");
                   funtNode.SetAttribute("name", e.name);
                   funtNode.SetAttribute("size", size.ToString());
                   funtNode.SetAttribute("complexity", complexity.ToString());
                   root.AppendChild(funtNode);

               }

           }
           doc1.Save("Functions.xml");


           //FOR DISPLAYING RELATIONSHIPS
           if (relationship)
           {
               XmlDocument doc2 = new XmlDocument();
               doc2.LoadXml(" <codeAnalysis>   </codeAnalysis> ");

               XmlNode root2 = doc2.DocumentElement;

               List<RelElem> table2 = rep.inheritance;

               List<RelElem> temp2 = rep.inheritance;

               foreach (Elem ee in table)
               {
                   if (ee.type.Equals("class"))
                   {
                       //CREATE NODE

                       XmlElement classNode = doc2.CreateElement("class");
                       classNode.SetAttribute("name", ee.name);
                       root2.AppendChild(classNode);
                       
                       

                       foreach (RelElem rr in table2)
                       {
                           if ((rr.beginRel >= ee.begin) && (rr.endRel <= ee.end) && (rr.type != "UsingTemp") && (rr.type != "UsingStr"))
                           {
                               XmlElement relNode = doc2.CreateElement("relationship");
                               relNode.SetAttribute("type", rr.type);
                               relNode.SetAttribute("with", rr.withName);
                               relNode.SetAttribute("atline",rr.beginRel.ToString());
                               classNode.AppendChild(relNode);


                           }
                       }
                   }
               }

               doc2.Save("Relationships.xml");
           }
         

       }
 
#if(TEST_XML)
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
            XMLProcessor x=new XMLProcessor();
            x.process(relationships);
            Console.ReadLine(); 
        }
#endif
    }
}
