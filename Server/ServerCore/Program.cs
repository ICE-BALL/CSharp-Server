using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAccepthandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();
                session.Start(clientSocket);

                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG Server !");
                session.Send(sendBuff);

                Thread.Sleep(1000);

                session.Disconnect();
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            string host = Dns.GetHostName();
            // 주소 가져오기
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            // 첫 번째로 알려준 주소
            IPAddress ipAddr = ipHost.AddressList[0];
            // 최종 주소  7777 => 문 느낌 클라이언트랑 맞춰 줘야함.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            try
            {
                _listener.Init(endPoint, OnAccepthandler);
                Console.WriteLine("Listening...");

                while (true)
                {
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }
    }
}
