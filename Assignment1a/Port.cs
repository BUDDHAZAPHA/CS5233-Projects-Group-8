using O2DESNet;
using MathNet;
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

        public int NoInServer1 { get; set; }
        public int NoInServer2 { get; set; }
        public int NoInQueue { get; set; }
        public List<Ship> PendingList = new();
        public List<Ship> ProcessedList = new();
        public double AverageCycleTime
        {
            get
            {
                return ProcessedList.Average(ship => (ship.TimeStamp_End - ship.TimeStamp_Arrive).TotalDays);
            }
        }

        public double MaxBerthTime
        {
            get
            {
                return ProcessedList.Max(ship => (ship.TimeStamp_End - ship.TimeStamp_Arrive).TotalDays);
            }
        }
        public double MinBerthTime
        {
            get
            {
                return ProcessedList.Min(ship => (ship.TimeStamp_End - ship.TimeStamp_Arrive).TotalDays);
            }
        }

        public double AverageWaitingTime
        {
            get
            {
                return ProcessedList.Average(ship => (ship.TimeStamp_End - ship.TimeStamp_Start).TotalDays);
            }
        }
        public HourCounter HC_InQueue { get; set; }
        public HourCounter HC_InServer1 { get; set; }
        public HourCounter HC_InServer2 { get; set; }
        public HourCounter HC_InSystem { get; set; }
        public PortStats Config { get; set; }

        
        public Port(PortStats config,int seed) : base(seed)
        {
            Config = config;
            HC_InQueue = AddHourCounter();
            HC_InServer1 = AddHourCounter();
            HC_InServer2 = AddHourCounter();
            HC_InSystem = AddHourCounter();
            Schedule(()=>Arrive(new Ship { Index=0}));
           
        }
        void Arrive(Ship ship)
        {
            NoInQueue++;
            HC_InQueue.ObserveChange(1);
            HC_InSystem.ObserveChange(1);
          //  HC_InQueue.ObserveCount(NoInQueue);
            if (NoInServer1 < Config.ServerCapacity)  Schedule(()=> StartServer1(ship));
           else if(NoInServer2 < Config.ServerCapacity) Schedule(() => StartServer2(ship));
            else
                PendingList.Add(ship);

            Schedule(() => Arrive(new Ship { Index = ship.Index + 1 }), TimeSpan.FromDays(MathNet.Numerics.Distributions.Exponential.Sample(DefaultRS, Config.ExpectedArrivalTime)));
            ship.TimeStamp_Arrive = ClockTime;
            
            Console.WriteLine($"{ClockTime} \tArrive#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");
        }
        void StartServer1(Ship ship)
        {
            NoInServer1++;
            NoInQueue--;
         
            HC_InQueue.ObserveChange(-1);
            HC_InServer1.ObserveChange(1);
            //HC_InQueue.ObserveCount(NoInQueue);
            ship.TimeStamp_Start = ClockTime;
           
            Schedule(() => End1(ship), TimeSpan.FromDays(MathNet.Numerics.Distributions.ContinuousUniform.Sample(DefaultRS, Config.ExpectedServiceStartTime, Config.ExpectedServiceEndTime)));

            Console.WriteLine($"{ClockTime} \tStart1#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");
        }
        void StartServer2(Ship ship)
        {
            NoInServer2++;
            NoInQueue--;
            ship.TimeStamp_Start = ClockTime;
            HC_InServer2.ObserveCount(NoInServer2);
            HC_InQueue.ObserveChange(-1);
            HC_InServer2.ObserveChange(1);
          
            Schedule(() => End2(ship), TimeSpan.FromDays(MathNet.Numerics.Distributions.ContinuousUniform.Sample(DefaultRS, Config.ExpectedServiceStartTime, Config.ExpectedServiceEndTime)));
            Console.WriteLine($"{ClockTime} \tStart2#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");

        }
        void End1(Ship ship)
        {
            NoInServer1--;
            HC_InSystem.ObserveChange(-1);
            HC_InServer1.ObserveChange(-1);
           // HC_InSystem.ObserveCount(NoInServer2);
            if (NoInQueue > 0)
            {
                //because fifo
                var next = PendingList.First();
                PendingList.RemoveAt(0);
                Schedule(() => StartServer1(next));
                  }

            ship.TimeStamp_End = ClockTime;
            ProcessedList.Add(ship);
            Console.WriteLine($"{ClockTime} \tEnd1#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");

        }
        void End2(Ship ship)
        {
            NoInServer2--;
            HC_InServer2.ObserveChange(-1);
            HC_InSystem.ObserveChange(-1);
            if (NoInQueue > 0)
            {
                //because fifo
                var next = PendingList.First();
                PendingList.RemoveAt(0);
                Schedule(() => StartServer2(next));
            }

            ship.TimeStamp_End = ClockTime;
            ProcessedList.Add(ship);
            Console.WriteLine($"{ClockTime} \tEnd2#{ship.Index}\tQ={NoInQueue}\tS1={NoInServer1}\tS2={NoInServer2}");

        }
    }
}
