using System;
using System.Threading;

internal class Program
{
    // volatile 키워드는 컴퓨터한테 건드리지 말라는 키워드
    volatile static bool _stop = false;

    static void MainThread()
    {
        Console.WriteLine("쓰레드 시작!");

        while(!_stop)
        {

        }
    }
    
    private static void Main(string[] args)
    {
        Task t = new Task(MainThread);
        t.Start();

        Thread.Sleep(1000);

        _stop = true;

        Console.WriteLine("stop 호출"); 
        Console.WriteLine("종료 대기중");

        t.Wait();

        Console.WriteLine("종료 성공");
    }
}