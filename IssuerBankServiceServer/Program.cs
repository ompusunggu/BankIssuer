using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using IssuerBankServiceLibrary;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
namespace IssuerBankServiceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpServerChannel channel = new TcpServerChannel(2345);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServiceServer), "IIssuerService", WellKnownObjectMode.SingleCall);
                Console.WriteLine("Server is running");
                Console.ReadLine();
            }
            catch
            {
                Console.WriteLine("Can't start server");
            }
        }
    }
}
