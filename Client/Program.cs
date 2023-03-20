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
        Thread t = new Thread(MainThread);
        // 쓰레드의 이름 지정
        t.Name = "TestThread";
        // 쓰레드 뒤에서만 실행하냐 앞에서도 하냐 차이 보여주는 거라고 하심
        t.IsBackground = true;
        
        t.Start();
        
        Console.WriteLine("Hello world!");
        t.Join();
    }
}