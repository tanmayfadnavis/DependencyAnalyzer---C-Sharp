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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class DepServer
    {
        static IMessageService CreateClientChannel(string url)
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

        public static string ConvertToXml(object toSerialize)
        {
            string temp;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, toSerialize, ns);
                temp = writer.ToString();
            }
            return temp;
        }
        static void Main(string[] args)
        {
            /*This is opening the server as a host to receive messages */

            ServiceHost host = CreateServiceChannel("http://localhost:8080/MessageService");
            host.Open();

           /* IMessageService proxy = CreateClientChannel("http://localhost:8081/MessageService");

            List<string> projects = new List<string>();
            projects.Add("Project #1");
            projects.Add("Project #2");


            SvcMsg msg = new SvcMsg();
            msg.src = new Uri("http://localhost:8080/MessageService");
            msg.dst = new Uri("http://localhost:8081/MessageService");
            msg.cmd = SvcMsg.Command.ProjectList;
            msg.body = ConvertToXml(projects);
            proxy.SendMessage(msg); */

            /*SEND A MESSAGE TO SERVER-2 REQUESTING ITS TYPE TABLE AND SENDING SERVER-1'S TYPE TABLE*/

            



            IMessageService serverproxy = CreateClientChannel("http://localhost:8081/MessageService");
            SvcMsg typeMsg = new SvcMsg();
            typeMsg.src = new Uri("http://localhost:8080/MessageService");
            typeMsg.dst = new Uri("http://localhost:8081/MessageService");
            typeMsg.cmd = SvcMsg.Command.TypeTable;
            //typeMsg.body = typeDict.ToString() ;
            serverproxy.SendMessage(typeMsg);

            Console.Write("\n  press key to terminate service\n");
            Console.ReadKey();
            Console.Write("\n");
            host.Close();
        }
    }
}
