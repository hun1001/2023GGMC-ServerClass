using System;
using System.Threading;

internal class Program
{
    static void MainThread()
    {
        for (int i = 0; i < 10; ++i)
        {
            Console.WriteLine("Hello thread!");
        }
    }
    
    private static void Main(string[] args)
    {
        // 알아서 쓰레드 만들어서 실행시켜주는 거
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(5, 5);

        for (int i = 0; i < 5; ++i)
        {
            Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
            t.Start();
        }

        ThreadPool.QueueUserWorkItem((o) => MainThread());

        while (true)
        {
            
        }
    }
}