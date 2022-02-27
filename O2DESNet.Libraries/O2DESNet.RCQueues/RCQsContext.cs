using System;
using System.Collections.Generic;
using System.Linq;

namespace O2DESNet.RCQueues
{
    public class RCQsContext : Sandbox
    {
        #region Statics
        public class Statics
        {
            public List<IResource> Resources { get; set; }
            public Dictionary<IResource, (TimeSpan TimeInterval, List<double> Capacities)> DynamicCapacities { get; set; }
        }
        
        private Statics Config { get; }

        private int _count;

        public int Count => _count;
        //public DateTime ClockTime { get; }
        #endregion

        #region Dynamics
        public readonly RCQsModel _rcqModel;
        private readonly List<IActivityHandler> _activityHandlers = new List<IActivityHandler>();

        /// <summary>
        /// Contains all loads in the system (entered but not exited), mapping to the activity handler they are currently in
        /// </summary>
        private readonly Dictionary<ILoad, IActivityHandler> _occupying = new Dictionary<ILoad, IActivityHandler>();
        /// <summary>
        /// Contains all loads requesting to start an activity, including requesting to enter for the 1st activity 
        /// </summary>
        private readonly Dictionary<ILoad, IActivityHandler> _requesting = new Dictionary<ILoad, IActivityHandler>();
        /// <summary>
        /// Map load to its batch
        /// </summary>
        private readonly Dictionary<ILoad, IBatch> _loadToBatch = new Dictionary<ILoad, IBatch>();

        public Dictionary<IResource, double> ResourceOccupied
        {
            get
            {
                return Config.Resources.ToDictionary(res => res,
                    res => _rcqModel.ResourceHC_Occupied[res].LastCount);
            }
        }

        public Dictionary<IResource, double> ResourceHourlyUtilization
        {
            get
            {
                return Config.Resources.ToDictionary(res => res,
                    res => (_rcqModel.ResourceHC_Occupied[res].AverageCount - _rcqModel.ResourceHC_PendingLock_Passive[res].AverageCount - _rcqModel.ResourceHC_PendingLock_Active[res].AverageCount) /
                    _rcqModel.ResourceHC_DynamicCapacity[res].AverageCount);
            }
        }
        public Dictionary<IResource, double> ResourceUtilization
        {
            get
            {
                return Config.Resources.ToDictionary(res => res, 
                    res => _rcqModel.ResourceHC_Occupied[res].AverageCount / 
                    _rcqModel.ResourceHC_Available[res].AverageCount);
            }
        }
        public Dictionary<IResource, double> ResourceAvailability
        {
            get
            {
                return Config.Resources.ToDictionary(res => res,
                    res => _rcqModel.ResourceHC_Available[res].AverageCount);
            }
        }

        protected IActivityHandler AddChild(IActivityHandler activityHandler)
        {
            AddChild(activityHandler as Sandbox);
            _activityHandlers.Add(activityHandler);
            activityHandler.OnRequestToStart += load => RequestToStart(load, activityHandler);
            return activityHandler;
        }
        #endregion

        #region Events
        private void Start(IBatch batch)
        {          
            var load = batch.First();

            // for debug
            //if (load.Index == 11 && _requesting[load].Activity.Name == "PreparingToDismount") ;
            //if (load.Index == 11 && _requesting[load].Activity.Name == "Dismounting") ;

            if (!_occupying.ContainsKey(load))
            {
                /// new to the system   
                _occupying.Add(load, _requesting[load]);
                _loadToBatch.Add(load, batch);
            }
            else
            {
                /// entered in the system                
                _occupying[load].Depart(load);
                _occupying[load] = _requesting[load];
                _loadToBatch[load] = batch;
            }            
            _occupying[load].Start(load);
            _requesting.Remove(load);
        }
        void AdjustResourceCapacity(IResource resource, int index)
        {
            //if (resource.Name == "Equipment Forklift at Receipt station Fleet")
           // {
                //Console.WriteLine(ClockTime);
           // }
            /// call lock/unlock input events of the RCQModel                            

            double LatestCapacityValue = Config.DynamicCapacities[resource].Capacities[index];  //Capacity value that to be updated
            var PreviousCapacityValue = 0.0;
            if (index == 0)
            {
                PreviousCapacityValue = _rcqModel.ResourceHC_DynamicCapacity[resource].LastCount;
            }
            else
            {
                PreviousCapacityValue = Config.DynamicCapacities[resource].Capacities[index-1];
            }

            if (LatestCapacityValue < PreviousCapacityValue)
            {
                _rcqModel.RequestToLock(resource, PreviousCapacityValue - LatestCapacityValue);
            }
            else if (LatestCapacityValue > PreviousCapacityValue)
            {

                _rcqModel.RequestToUnlock(resource, LatestCapacityValue - PreviousCapacityValue);
            }
            else
            {
                //  Console.WriteLine("\t No change in {0} capacity", resource.Id);
            }

            index++;

            if (index == Config.DynamicCapacities[resource].Capacities.Count) index = 0;
            Schedule(() => AdjustResourceCapacity(resource, index), Config.DynamicCapacities[resource].TimeInterval);
            
            _count++;
        }

        private void RequestToStart(ILoad load, IActivityHandler activityHandler)
        {
            // for debug
            //if (load.Index == 11 && activityHandler.Activity.Name == "PreparingToDismount") ;
            //if (load.Index == 11 && activityHandler.Activity.Name == "Dismounting") ;

            _requesting.Add(load, activityHandler);
            var activity = activityHandler.Activity;
            if (!_occupying.ContainsKey(load))
            {
                /// new to the system   
                _rcqModel.RequestToEnter(load, activity);
            }
            else
            {
                /// entered in the system
                _rcqModel.Finish(_loadToBatch[load], new Dictionary<ILoad, IActivity> { { load, activity } });
            }
        }

        protected void Exit(ILoad load)
        {
            if (!_occupying.ContainsKey(load)) throw new Exception("The load does not exist.");
            _rcqModel.Finish(_loadToBatch[load], null);
            _occupying[load].Depart(load);
            _occupying.Remove(load);
            _loadToBatch.Remove(load);
        }
        #endregion

        public RCQsContext(Statics config, int seed = 0) : base(seed)
        {
            Config = config;

            _rcqModel = AddChild(new RCQsModel(new RCQsModel.Statics(Config.Resources), DefaultRS.Next()));

            foreach (var res in Config.Resources)
            {
                if (Config.DynamicCapacities.ContainsKey(res))
                {
                    Schedule(() => AdjustResourceCapacity(res, 0));
                }
            }
            //ClockTime = Convert.ToDateTime("2/2/2021");
            _rcqModel.OnStarted += Start;
            _rcqModel.OnReadyToExit += _rcqModel.Exit;
        }
    }
}
