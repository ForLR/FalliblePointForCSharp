using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalliblePointForCSharp
{
    /// <summary>
    /// 使用背景事例
    /// 多个用户编号从1到10，我希望编号相同的用户在执行代码时可以线程同步，但是其他编号的可以插入进来，不需要同步
    /// 尝试了lock锁，但是lock会把所有线程同步处理，同一时间只能处理一个请求，结束以后才处理第二个请求，我的需求是同一用户线程同步，不同用户可以异步处理
    /// </summary>
    public class 实现同一用户的线程锁
    {
        private static readonly object _lock = new object();
        static  void Main(string[] args)
        {
            Person person1 = new Person() { Id=1,Name="张三"};
            Person person2 = new Person() { Id = 2, Name = "李四" };
            Person person3 = new Person() { Id = 3, Name = "王五" };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
    
            for (int i = 0; i < 100; i++)
            {
                Task.Run(() => { MutexGetCoupon(person1); });
               Task.Run(() => { MutexGetCoupon(person2); });
               Task.Run(() => { MutexGetCoupon(person3); });
            }
             Task.Delay(1);
            stopwatch.Stop();
            Console.WriteLine("Mutex总耗时:"+stopwatch.ElapsedMilliseconds+"ms");


            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (int i = 0; i < 100; i++)
            {
                 Task.Run(() => { LockGetCoupon(person1); });
                 Task.Run(() => { LockGetCoupon(person2); });
                 Task.Run(() => { LockGetCoupon(person3); });
            }
            stopwatch1.Stop();
            Console.WriteLine("Lock总耗时:" + stopwatch1.ElapsedMilliseconds + "ms");
            Console.ReadKey();

        }
        /// <summary>
        /// 使用lock锁的话 每一次都需要排队进入
        /// </summary>
        /// <param name="person"></param>
        public static void LockGetCoupon(Person person)
        {
            Console.WriteLine(person.Name+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"Lock进入");

            lock (_lock)
            {
                if (person.IsGetCoupon)
                {
                    Console.WriteLine(person.Name + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "已经领取");
                }
                else
                {
                    person.IsGetCoupon = true;
                    Console.WriteLine(person.Name + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "领取成功");
                }

            }
        }

        public static void MutexGetCoupon(Person person)
        {
            Console.WriteLine(person.Name + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Mutex进入");

            using (var mutex=new Mutex(false,person.Id.ToString()))
            {
                try
                {
                    if (mutex.WaitOne(-1,false))
                    {

                        if (person.IsGetCoupon)
                        {
                            Console.WriteLine(person.Name + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "已经领取");
                        }
                        else
                        {
                            person.IsGetCoupon = true;
                            Console.WriteLine(person.Name + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "领取成功");
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGetCoupon { get; set; }
    }
}
