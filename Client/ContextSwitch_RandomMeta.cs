﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace SServer
{
    class SpinLock // Lock이 풀릴 때 까지 돌아가는 존버메타

    {
        volatile int _locked = 0;  // _locked 가시성 확보

        public void Acquire()
        {
            while (true)
            {
                int expected = 0; // 예상하는 값
                int desired = 1;  // 원하는 값
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                // 쉬다 올께 --> 셋 중 하나 선택 
                //Thread.Sleep(1); // 무조건 휴식 -> 1ms 정도 쉬고 싶은데 정확한 시간은 운영체제가 결정

                //Thread.Sleep(0); // 조건부 양보 -> 나보다 우선순위가 낮으면 양보 안함 
                // 우선순위가 높거나 동일한 쓰레드가 없으면 다시 본인한테 

                Thread.Yield(); // 관대한 양보 -> 관대하게 양보할테니, 지금 실행가능한 쓰레드가 있으면 양보할께요.
                                // 없으면 마지막에 쓰레드 이용.
            }
        }

        public void Release()
        {
            // 락 해제, 문을 열고 나옴.
            _locked = 0;
        }
    }

    class Program
    {


        static int number = 0;
        // 스핀락 인스턴스 생성
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.Acquire();    // 진입 후 락!

                number++;

                _lock.Release();    // 나간 뒤 락 해제!
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
}