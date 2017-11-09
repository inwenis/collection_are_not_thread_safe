using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// this program will throw an exception
// it just shows that collections are not thread safe
// the exception will be 
// System.NullReferenceException: Object reference not set to an instance of an object.
//    at System.Linq.Buffer`1.ToArray()
//    at System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)
//    at Program.<>c__DisplayClass0_1.<Main>b__1()
// if multiple thread will access your collections use Concurent Collections
class Program
{
    static void Main()
    {
        var set = new HashSet<object>();
        var tasks = new List<Task>();
        ThreadPool.SetMinThreads(100, 100);
        Enumerable.Range(1, 1000000).Select(y => set.Add(new ClassConsumingSomeMemory())).ToArray();
        for (int k = 0; k < 1000; k++)
        {
            int local_k = k;
            var task = Task.Run(() =>
            {
                try
                {
                    var objects = set.ToArray();
                    foreach (var @object in objects)
                    {
                        set.Remove(@object);
                    }
                    Console.WriteLine($"My {nameof(local_k)} is {local_k}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }
}

public class ClassConsumingSomeMemory
{
    public int[] eatSomeBytes;
    public ClassConsumingSomeMemory()
    {
        eatSomeBytes = new int[400];
    }
}
