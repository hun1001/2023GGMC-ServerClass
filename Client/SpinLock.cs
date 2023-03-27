class SpinLock // Lock이 풀릴 때 까지 돌아가는 존버메타
{
    volatile int _locked = 0;

    public void Acquire()
    {
        while (!(Interlocked.CompareExchange(ref _locked, 1, 0) == 0))
        {
            
        }
    }

    public void Release()
    {
        Interlocked.Exchange(ref _locked, 0);
    }
}

class Program
{
    static int number = 0;
    static SpinLock _lock = new SpinLock();

    static void Thread_1()
    {
        for (int i = 0; i < 10000; i++)
        {
            _lock.Acquire();    // 락 시작!

            number++;

            _lock.Release();    // 락 끝!
        }
    }

    static void Thread_2()
    {

        for (int i = 0; i < 10000; i++)
        {
            _lock.Acquire();

            number--;

            _lock.Release();
        }
    }

    static void Main(string[] args)
    {
        Task t1 = new Task(Thread_1);
        Task t2 = new Task(Thread_2);

        t1.Start();
        t2.Start();

        Task.WaitAll(t1, t2);

        Console.WriteLine(number);

    }
}