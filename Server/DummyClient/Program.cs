using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Connected To {endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World !");
            Send(sendBuff);

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"DisConnected {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> Buffer)
        {
            string recvData = Encoding.UTF8.GetString(Buffer.Array, Buffer.Offset, Buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");

            return Buffer.Count;
        }

        public override void OnSend(int numOfbytes)
        {
            Console.WriteLine($"Sended {numOfbytes} byte");
        }
    }

    internal class Program
    {
        static Connector _connector = new Connector();

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry IPhost = Dns.GetHostEntry(host);
            IPAddress IPAddr = IPhost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

            _connector.Init(endPoint, () => { return new GameSession(); });

            while (true)
            {
                ;
            }
        }

    }
}
