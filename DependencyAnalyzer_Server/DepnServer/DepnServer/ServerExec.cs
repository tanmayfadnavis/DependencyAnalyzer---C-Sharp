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
   public class ServerExec
    {

        /*static IMessageService CreateClientChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<IMessageService> factory =
              new ChannelFactory<IMessageService>(binding, address);
            return factory.CreateChannel();
        }

        static ServiceHost CreateServiceChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri baseAddress = new Uri(url);
            Type service = typeof(MessageService);
            ServiceHost host = new ServiceHost(service, baseAddress);
            host.AddServiceEndpoint(typeof(IMessageService), binding, baseAddress);
            return host;
        } 

        private string path;
        private List<string> pattern = new List<string>();
        private List<string> options = new List<string>();
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
        public bool Recurse
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
        static public string[] getFiles(string path, List<string> patterns, bool recurse)
        {
            FileManager fm = new FileManager();
            //foreach (string p in patterns)
            //  fm.addPattern(p);

            fm.findFiles(path, patterns, recurse);
            return fm.getFiles().ToArray();

        }

        static public void start()
        {

        }*/

       /* public static void Main(string[] args)
        {
            ServerExec e = new ServerExec();


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


            e.Recurse = e.options.Contains("/S");
            e.Relationship = e.options.Contains("/R");
            e.XMLProcessing = e.options.Contains("/X");
            string[] files = Executive.getFiles(e.path, e.pattern, e.Recurse);
            Analyzer ana = new Analyzer();
            Display d = new Display();
            ana.doAnalysis(files);
            //d.display(files,e.Relationship);

            TypeBuilder tb = new TypeBuilder();
           // Dictionary<string, TypeElem> typeDict;


            //Add for sending the message to server2

            IMessageService serverproxy = CreateClientChannel("http://localhost:8081/MessageService");
            SvcMsg typeMsg = new SvcMsg();
            typeMsg.src = new Uri("http://localhost:8080/MessageService");
            typeMsg.dst = new Uri("http://localhost:8081/MessageService");
            typeMsg.cmd = SvcMsg.Command.TypeTable;
            typeMsg.body = "test";
            serverproxy.SendMessage(typeMsg);



            if (e.XMLProcessing)
            {
                XMLProcessor xp = new XMLProcessor();

                xp.process(e.Relationship);
            }

            Console.ReadLine();
        }*/
    }
}
