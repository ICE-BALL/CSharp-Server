using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Connected To {endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Server !");
            Send(sendBuff);

            Thread.Sleep(1000);

            DisConnect();

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"DisConnected {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> Buffer)
        {
            string recvData = Encoding.UTF8.GetString(Buffer.Array, Buffer.Offset, Buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");

            return Buffer.Count;
        }

        public override void OnSend(int numOfbytes)
        {
            Console.WriteLine($"Sended {numOfbytes} byte");
        }
    }

    internal class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry IPhos = Dns.GetHostEntry(host);
            IPAddress IPAddr = IPhos.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });

            while (true)
            {
                ;
            }
        }
    }
}
