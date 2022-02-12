using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock
    {
        volatile static int _lock = 0;

        public void Acquire()
        {
            while (true)
            {
                int origin = Interlocked.CompareExchange(ref _lock, 1, 0);
                if (origin == 0)
                    break;

                Thread.Yield();
            }
        }

        public void Release()
        {
            _lock = 0;
        }
    }

    class Program
    {
        static int num = 0;

        static Mutex _lock = new Mutex();

        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.WaitOne();
                num++;
                _lock.ReleaseMutex();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.WaitOne();
                num--;
                _lock.ReleaseMutex();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(num);
        }
    }
}
