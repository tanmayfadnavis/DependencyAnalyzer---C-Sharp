using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DepnServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
   
    [DataContract(Namespace="DepnServer")]

    public class SvcMsg
    {
        public enum Command { Projects, ProjectList, Dependencies, TypeTable};
        [DataMember]
        public Command cmd;
        [DataMember]
        public Uri src;
        [DataMember]
        public Uri dst;
        [DataMember]
        public string body;

        public void ShowMessage()
        {
            Console.Write("\n  Received Message:");
            Console.Write("\n    src = {0}\n    dst = {1}", src.ToString(), dst.ToString());
            Console.Write("\n    cmd = {0}", cmd.ToString());
        }


        public string toString()
        {
            throw new NotImplementedException();
           // return "command=" + cmd + "source" + src + "destination=" + dst + "body=" + body;
        }
    }

    [ServiceContract(Namespace="DepnServer")]
    public interface IMessageService
    {
        [OperationContract]
        void SendMessage(SvcMsg msg);

        [OperationContract]
        string GetMessage();
    }

    [ServiceBehavior(Namespace="DepnServer")]

    public class MessageService : IMessageService
    {
        public void SendMessage(SvcMsg msg)
        {
            msg.ShowMessage();
           
        }

        public string GetMessage()
        {
            return "temp";
        }
    }
}