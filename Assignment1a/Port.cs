using O2DESNet;
using O2DESNet.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1a
{
   public class PortStats
    {
        public double ExpectedArrivalTime { get; set; }
        public double ExpectedServiceStartTime { get; set; }
        public double ExpectedServiceEndTime { get; set; }
        public int ServerCapacity { get; set; }

    }
    public class Port : Sandbox
    {

             
        public int Count { get; set; }
        public int NoInServer1 { get; set; }
        public int NoInServer2 { get; set; }
        public int NoInQueue { get; set; }
        public List<Ship> PendingList = new();
        public List<Ship> ProcessedList = new();
        public double AverageCycleTime
        {
            get
            {
                return ProcessedList.Average(ship => (ship.TimeStamp_End - ship.TimeStamp_Arrive).TotalMinutes);
            }
        }

        public double AverageWaitingTime
        {
            get
            {
                return ProcessedList.Average(ship => (ship.TimeStamp_End - ship.TimeStamp_Start).TotalDays);
            }
        }
        public PortStats Config { get; set; }

        
        public Port(PortStats config,int seed) : base(seed)
        {
            Config = config;
           
            Schedule(()=>Arrive(new Ship { Index=0}));
           
        }
        void Arrive(Ship ship)
        {
            NoInQueue++;
            if (NoInServer1 < Config.ServerCapacity)  Schedule(()=> StartServer1(ship));
            else if(NoInServer2 < Config.ServerCapacity) Schedule(() => StartServer2(ship));
            else
                PendingList.Add(ship);

            Schedule(() => Arrive(new Ship { Index = ship.Index + 1 }), TimeSpan.FromDays(Exponential.Sample(DefaultRS, Config.ExpectedArrivalTime)));
            ship.TimeStamp_Arrive = ClockTime;
            
            Console.WriteLine($"{ClockTime} \tArrive#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");
        }
        void StartServer1(Ship ship)
        {
            NoInServer1++;
            NoInQueue--;
            ship.TimeStamp_Start = ClockTime;
            Schedule(() => End1(ship), TimeSpan.FromDays(Uniform.Sample(DefaultRS, Config.ExpectedServiceStartTime, Config.ExpectedServiceEndTime)));

            Console.WriteLine($"{ClockTime} \tArrive#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");
        }
        void StartServer2(Ship ship)
        {
            NoInServer2++;
            NoInQueue--;
            ship.TimeStamp_Start = ClockTime;
            Schedule(() => End2(ship), TimeSpan.FromDays(Uniform.Sample(DefaultRS, Config.ExpectedServiceStartTime, Config.ExpectedServiceEndTime)));
            Console.WriteLine($"{ClockTime} \tArrive#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");

        }
        void End1(Ship ship)
        {
            NoInServer1--;
            if (NoInQueue > 0)
            {
                //because fifo
                var next = PendingList.First();
                PendingList.RemoveAt(0);
                Schedule(() => StartServer1(next));
                  }

            ship.TimeStamp_End = ClockTime;
            ProcessedList.Add(ship);
            Console.WriteLine($"{ClockTime} \tArrive#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");

        }
        void End2(Ship ship)
        {
            NoInServer2--;
            if (NoInQueue > 0)
            {
                //because fifo
                var next = PendingList.First();
                PendingList.RemoveAt(0);
                Schedule(() => StartServer2(next));
            }

            ship.TimeStamp_End = ClockTime;
            ProcessedList.Add(ship);
            Console.WriteLine($"{ClockTime} \tArrive#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");

        }
    }
}
