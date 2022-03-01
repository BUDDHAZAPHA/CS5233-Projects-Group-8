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

            for(int i=0;i<1000;i++)
                sim.Run(TimeSpan.FromDays(100));
            Console.WriteLine($"Average cycle Time : {sim.AverageCycleTime} days");
            Console.WriteLine($"Max Berth Time : {sim.MaxBerthTime} days");
            Console.WriteLine($"Min Berth Time : {sim.MinBerthTime} days");
            Console.WriteLine($"Average waiting Time : {sim.AverageWaitingTime}  days");
            //rcq.ResourceHourCounterActive[res].AverageCount / rcq.ResourceHourCounterDynamicCapacity[res].AverageCount
           
            Console.WriteLine($"Average Utility of Crane1 : {sim.HC_InServer1.TotalIncrement/(100*1000)} ");
            Console.WriteLine($"Average Utility of Crane2 : {sim.HC_InServer2.TotalIncrement / (100 * 1000)} ");
            //Console.WriteLine($"Average cycle Time : {sim.ProcessedList.sel } days");

            Console.WriteLine($"Average time the load was inside the system : {sim.HC_InSystem.AverageDuration} ");
            Console.WriteLine($"Percentage of ships that stay in the harbour for more than 2 days : {(sim.ProcessCounts *100)/sim.HC_InQueue.TotalIncrement} ");
        }
    }
}
