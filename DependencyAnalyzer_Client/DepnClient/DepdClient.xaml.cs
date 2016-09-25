using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DepnServer;

namespace DepnClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {



            InitializeComponent();

            /*This is opening the client as a host to receive messages */

           // ServiceHost host = CreateServiceChannel("http://localhost:8081/MessageService");
            //host.Open();

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             
            /*this is to send the message to server */

           /* IMessageService proxy = CreateClientChannel("http://localhost:8080/MessageService");
            SvcMsg msg = new SvcMsg();
            msg.cmd = SvcMsg.Command.Projects;
            msg.src = new Uri("http://localhost:8081/MessageService");
            msg.dst = new Uri("http://localhost:8080/MessageService");
            msg.body = "body";
            proxy.SendMessage(msg); */

            /*this after receiving message from server */

            

           // text1.Text = proxy.GetMessage(msg);
           // Console.Write("\n  press key to terminate service");
            //Console.ReadKey();
            //Console.Write("\n");
        }

        //code from fawcett

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

        

        
    }
}
