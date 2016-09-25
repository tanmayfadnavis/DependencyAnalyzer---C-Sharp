///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2014, .Net Framework 4.5                       //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Original Author:      Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com               //
//Author: Tanmay Fadnavis, Syracuse University                      //
//          tfadnavi@syr.edu                                        //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.3: Oct 2014
 * Added the following rules
 * -DetectEnum Rule
 * -DetectInheritance Rule
 * -DetectComposition Rule
 * -DetectAggregation Rule
 * -DetectUsing Rule
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DepnServer
{

    public class TypeElem //Holds the type table
    {
        public string namespaceName { get; set; }
        public string typeType { get; set; }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", typeType)).Append(" : ");
            temp.Append(String.Format("{0,-10}", namespaceName)).Append(" : ");
            temp.Append("}");
            return temp.ToString();
        }

    }
    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }

    public class RelElem  //holds relationship info
    {
        public string type { get; set; }
        public string name { get; set; }
        public string withName { get; set; }
        public int beginRel { get; set; }
        public int endRel { get; set; }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-10}", withName)).Append(" : ");
            temp.Append(String.Format("{0,-5}", beginRel.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", endRel.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }

    }

    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>();
        ScopeStack<RelElem> stack2_ = new ScopeStack<RelElem>();
        List<RelElem> inheritance_ = new List<RelElem>();
        Dictionary<string, TypeElem> typeTable_ = new Dictionary<string, TypeElem>();
        static Repository instance;

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }


        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }


        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }

        public ScopeStack<RelElem> stack2
        {
            get { return stack2_; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations_; }
        }

        public List<RelElem> inheritance
        {
            get { return inheritance_; }
        }

        public Dictionary<string,TypeElem> typeTable
        {
            get { return typeTable_; }
        }

    }
    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;

            if (elem.type.Equals("braceless"))
            {
                elem.end = repo_.semi.lineCount - 1;
            }
            else
            {
                elem.end = 0;
                repo_.stack.push(elem);
            }
            //if (elem.type == "control" || elem.name == "anonymous")
            //  return;
            repo_.locations.Add(elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }

    //pushing the relationships information onto the stack

    public class PushStack2 : AAction
    {
        Repository repo_;

        public PushStack2(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            RelElem Relem = new RelElem();
            Relem.type = semi[0];  // expects relationship
            Relem.name = semi[1];  //expects name
            Relem.withName = semi[2];// expects withname
            Relem.beginRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;

            if (semi[0].Equals("Aggregation"))
            {
                Relem.endRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;
            }
            else if (semi[0].Equals("Composition"))
            {
                Relem.endRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;
            }
            else if (semi[0].Equals("Using"))
            {
                Relem.endRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;
            }
            else if (semi[0].Equals("UsingTemp"))
            {
                Relem.endRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;
            }
            else if (semi[0].Equals("UsingStr"))
            {
                Relem.endRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;
            }
            else if (semi[0].Equals("Inheritance"))
            {
                Relem.endRel = repo_.semi.lineCount - 1 - repo_.prevLineCount;
            }
            else
            {
                Relem.endRel = 0;
                repo_.stack2.push(Relem);
            }

            if (Relem.type == "control" || Relem.name == "anonymous" || Relem.withName == "anonymous2")
                return;

            repo_.inheritance.Add(Relem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }


    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem;
            try
            {
                elem = repo_.stack.pop();

                //reverse for loop for anonymous scope

                if (elem.type.Equals("control"))
                {
                    for (int i = repo_.locations.Count - 1; i >= 0; --i)
                    {
                        Elem temp = repo_.locations[i];
                        if (elem.type == temp.type)
                        {
                            if (elem.name == temp.name)
                            {
                                if ((repo_.locations[i]).end == 0)
                                {
                                    (repo_.locations[i]).end = repo_.semi.lineCount;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < repo_.locations.Count; ++i)
                    {
                        Elem temp = repo_.locations[i];
                        if (elem.type == temp.type)
                        {
                            if (elem.name == temp.name)
                            {
                                if ((repo_.locations[i]).end == 0)
                                {
                                    (repo_.locations[i]).end = repo_.semi.lineCount;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            //if(local[0] == "control")
            // return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }


    //POP STACK 2 FOR POPPING OUT ELEMENTS

    public class PopStack2 : AAction
    {
        Repository repo_;

        public PopStack2(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            RelElem Relem;
            try
            {
                Relem = repo_.stack2.pop();
                for (int i = 0; i < repo_.inheritance.Count; ++i)
                {
                    RelElem temp = repo_.inheritance[i];
                    if (Relem.type == temp.type)
                    {
                        if (Relem.name == temp.name)
                        {
                            if (Relem.withName == temp.withName)
                            {
                                if ((repo_.inheritance[i]).endRel == 0)
                                {
                                    (repo_.inheritance[i]).endRel = repo_.semi.lineCount - repo_.prevLineCount;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(Relem.type).Add(Relem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }

    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    //rules to detect enums

    public class DetectEnum : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("enum");

            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local);
                return true;
            }
            return false;
        }
    }

    //detect braceless scope

    public class DetectBracelessScope : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != ";")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("braceless").Add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }


    //rule to detect Inheritance

    public class DetectInheritance : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");

            if (indexCL != -1)
            {
                if (semi.Contains(":") != -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("Inheritance").Add(semi[indexCL + 1]).Add(semi[indexCL + 3]);
                    doActions(local);
                    return true;
                }
            }
            return false;
        }

    }

    //Detect Aggregation Relationship
    public class DetectAggregation : ARule
    {

        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = -1;
            int braIndex = -1;
            int newIndex = -1;
            int strIndex = -1;
            int eqlIndex = -1;
            int otherIndex = -1;
            int otherIndex2 = -1;
            Repository rep = Repository.getInstance();
            List<Elem> temp = rep.locations;
            foreach (Elem e in temp)
            {
                if (e.type.Equals("class"))
                {
                    index = semi.Contains(e.name);
                    braIndex = semi.Contains("{");
                    newIndex = semi.Contains("new");
                }

                if (index != -1 && newIndex != -1 && braIndex == -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("Aggregation").Add(semi[index + 1]).Add(semi[index]);
                    doActions(local);
                    return true;
                }
            }

            strIndex = semi.FindFirst("string");
            eqlIndex = semi.Contains("=");
            otherIndex = semi.Contains("<");
            otherIndex2 = semi.Contains("[");

            if (strIndex != -1 && eqlIndex != -1 && otherIndex2 == -1 && otherIndex == -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("Aggregation").Add(semi[strIndex]).Add(semi[strIndex + 1]);
                doActions(local);
                return true;
            }

            return false;
        }

    }

    //detect Using rule

    public class DetectUsing : ARule
    {

        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = -1;
            int braIndex = -1;
            int realUsing = -1;
            int dotIndex = -1;

            //Adding using rule for detecting strings

            int strIndex = -1;
            int strOpen = -1;
            int strClose = -1;
            int realStr = -1;

            Repository rep = Repository.getInstance();
            List<Elem> temp = rep.locations;
            List<RelElem> tempRel = rep.inheritance;
            List<RelElem> tempUse = rep.inheritance;


            //adding using rule for string

            strIndex = semi.Contains("string");
            strOpen = semi.Contains("(");
            strClose = semi.Contains(")");

            if ((strIndex != -1) && (strOpen != -1) && (strClose != -1) && (semi[strClose - 1] !="(") && (semi[strClose -1 ] != ")") && (semi.Contains("<") ==-1) && (semi.Contains("[") ==-1))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("UsingStr").Add(semi[strIndex]).Add(semi[strClose - 1]);
                doActions(local);
                return true;
            }

            foreach (Elem e in temp)
            {
                if (e.type.Equals("class"))
                {
                    index = semi.Contains(e.name);
                    braIndex = semi.Contains("(");
                    dotIndex = semi.Contains(".");
                }

                if (index != -1 && braIndex != -1 && dotIndex==-1 && semi[index + 1] !="(" && semi[index +1] !=")")
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("UsingTemp").Add(semi[index]).Add(semi[index + 1]);
                    doActions(local);
                    return true;
                }
            }
            foreach (RelElem rel in tempRel)
            {
                realUsing = semi.Contains(rel.withName);
                if (realUsing != -1 && rel.type.Equals("UsingTemp"))
                {
                   // foreach(RelElem rr in tempUse)
                    //{
                        
                         CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                        // local semiExp with tokens for type and name
                         local.displayNewLines = false;
                         local.Add("Using").Add(semi[realUsing]).Add(rel.name);
                           doActions(local);
                        return true;
                    //}

                }
            }

            //adding using for string
            foreach (RelElem rel in tempRel)
            {
                realStr = semi.Contains(rel.withName);
                if (realStr != -1 && rel.type.Equals("UsingStr"))
                {
                    // foreach(RelElem rr in tempUse)
                    //{

                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("Using").Add(rel.withName).Add(rel.withName);
                    doActions(local);
                    return true;
                    //}

                }
            }

            return false;
        }

    }

    //DETECT COMPOSITION

    public class DetectComposition : ARule
    {

        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = -1;
            int braIndex = -1;

            Repository rep = Repository.getInstance();
            List<Elem> temp = rep.locations;
            foreach (Elem e in temp)
            {
                if (e.type.Equals("struct"))
                {
                    index = semi.Contains(e.name);
                    braIndex = semi.Contains("{");

                }
                if (e.type.Equals("enum"))
                {
                    index = semi.Contains(e.name);
                    braIndex = semi.Contains("{");
                }

                if (index != -1 && braIndex == -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    if (e.type.Equals("struct"))
                        local.Add("Composition").Add("struct").Add(semi[index]);
                    else
                        local.Add("Composition").Add("enum").Add(semi[index]);

                    doActions(local);
                    return true;
                }
            }
            return false;
        }

    }

    //detect anonymous scope for relationships as the rel elem has 3 fileds
    public class DetectAnonymousScope2 : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous").Add("anonymous2"); ;
                doActions(local);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi);
                return true;
            }
            return false;
        }
    }
    public class BuildCodeAnalyzer
    {
        Repository repo = new Repository();

        public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            //capture enum info
            DetectEnum detectEn = new DetectEnum();
            detectEn.add(push);
            parser.add(detectEn);

            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            //hendle braceless scope
            DetectBracelessScope brace = new DetectBracelessScope();
            brace.add(push);
            parser.add(brace);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            // parser configured
            return parser;
        }

        public virtual Parser build2(Repository repo, CSsemi.CSemiExp semi2)
        {
            //repo.semi.lineCount = 0;
            repo.semi = semi2;
            repo.prevLineCount = semi2.lineCount;
            //Console.WriteLine("semi2 line count is {0}", semi2.lineCount);
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack2 push2 = new PushStack2(repo);


            //detect inheritance
            DetectInheritance detectI = new DetectInheritance();
            detectI.add(push2);
            parser.add(detectI);

            //Console.WriteLine("{0}",repo.locations);

            //detect aggregation
            DetectAggregation detectAGR = new DetectAggregation();
            detectAGR.add(push2);
            parser.add(detectAGR);

            // detect using
            DetectUsing detectUSG = new DetectUsing();
            detectUSG.add(push2);
            parser.add(detectUSG); 

            //detect composition
            DetectComposition detectComp = new DetectComposition();
            detectComp.add(push2);
            parser.add(detectComp);

            //detect anonymous scope
            /*  DetectAnonymousScope2 anon = new DetectAnonymousScope2();
              anon.add(push2);
              parser.add(anon); */

            // handle leaving scopes
            /* DetectLeavingScope leave = new DetectLeavingScope();
             PopStack2 pop2 = new PopStack2(repo);
             leave.add(pop2);
             parser.add(leave); */

            // parser configured
            return parser;

        }
    }
}

