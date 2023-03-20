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
        t.Name = "TestThread";
        t.IsBackground = true;
        
        t.Start();
        
        Console.WriteLine("Hello world!");
        t.Join();
    }
}