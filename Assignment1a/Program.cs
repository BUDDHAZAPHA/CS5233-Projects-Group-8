using System;

namespace Assignment1a
{
    public class Program
    {
       
        static void Main(string[] args)
        {
           
            Console.WriteLine("Hello World!");
            var config = new PortStats
            {
                ServerCapacity = 1,
                ExpectedArrivalTime = 1.25,
                ExpectedServiceStartTime = 0.5,
                ExpectedServiceEndTime = 1.5

            };
            var sim = new Port(config,seed: 0);
            // sim.Run(TimeSpan.FromDays(100));

            for(int i=0;i<100;i++)
                sim.Run(TimeSpan.FromDays(100));
            Console.WriteLine($"Average cycle Time : {sim.AverageCycleTime} mins");
            Console.WriteLine($"Average waiting Time : {sim.AverageWaitingTime}  days");
        }
    }
}
