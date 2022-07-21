using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disConnect = 0;
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> _sendList = new List<ArraySegment<byte>>();
        object _lock = new object();

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public void Start(Socket socket)
        {
            _socket = socket;

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);

                if (_sendList.Count == 0)
                    RegisterSend();
            }
        }

        void RegisterSend()
        {
            while (_sendQueue.Count > 0)
            {
                byte[] Buffer = _sendQueue.Dequeue();
                _sendList.Add(Buffer);
            }
            _sendArgs.BufferList = _sendList;

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _sendList.Clear();

                        OnSend(args.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    DisConnect();
                }
            }
        }

        void RegisterRecv()
        {
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                    {
                        if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                        {
                            DisConnect();
                            return;
                        }
                        int processLen = OnRecv(args.Buffer);

                        if (_recvBuffer.OnRead(args.BytesTransferred) == false)
                        {
                            DisConnect();
                            return;
                        }

                        RegisterRecv();
                    }
                    else
                    {
                        Console.WriteLine(args.BytesTransferred);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                DisConnect();
            }
        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref _disConnect, 1) == 1)
                return;

            OnDisConnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> Buffer);
        public abstract void OnSend(int numOfbytes);
        public abstract void OnDisConnected(EndPoint endPoint);
    }
}
