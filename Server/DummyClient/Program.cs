using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
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

            while (true)
            {
                // 휴대폰 연결
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 문지기한테 입장 문의
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected to {socket.RemoteEndPoint.ToString()}");

                    // 보낸다
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                        int sendBytes = socket.Send(sendBuff);
                    }
                    
                    // 받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    // 나간다
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(100);
            }
        }
    }
}
