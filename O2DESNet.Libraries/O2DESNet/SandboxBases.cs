using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Serilog;

namespace O2DESNet
{
    public abstract class SandboxBase<TAssets> : SandboxBase where TAssets : IAssets
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SandboxBase{TAssets}"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="pointer">The pointer.</param>
        public SandboxBase(TAssets assets = default, int seed = 0, string id = null, Pointer pointer = new Pointer())
            : base(seed, id, pointer) { Assets = assets; }

        /// <summary>
        /// Gets the assets.
        /// </summary>
        public TAssets Assets { get; }
    }

    /// <summary>
    /// Sandbox
    /// </summary>
    /// <seealso cref="O2DESNet.SandboxBase" />
    public abstract class SandboxBase : ISandbox
    {
        private ILogger _logger;

        internal readonly FutureEventList FutureEventList = new FutureEventList();
        private readonly List<ISandbox> _childrenList = new List<ISandbox>();
        private readonly List<HourCounter> _hourCounterList = new List<HourCounter>();

        private DateTime _clockTime = DateTime.MinValue;
        private DateTime? _realTimeForLastRun = null;
        private bool _isCancel = false;
        private int _seed;
        private string _logFile;
        private bool _isSimulationRunning;
        private Action _onWarmedUp;
        private static int _count = 0;

        #region Consturctors

        /// <summary>
        /// Initializes a new instance of the <see cref="SandboxBase"/> class.
        /// </summary>
        public SandboxBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SandboxBase"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="pointer">The pointer.</param>
        public SandboxBase(int seed = 0, string id = null, Pointer pointer = new Pointer())
        {
            _isCancel = false;

            Seed = seed;
            Index = ++_count;
            Id = id;
            Pointer = pointer;

            _onWarmedUp += WarmedUpHandler;
        }
        #endregion


        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger => _logger;

        /// <summary>
        /// Unique index in sequence for all module instances 
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// Tag of the instance of the module
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the pointer.
        /// </summary>
        public Pointer Pointer { get; private set; }

        /// <summary>
        /// Gets the default rs.
        /// </summary>
        protected Random DefaultRS { get; private set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        public int Seed
        {
            get => _seed;
            set
            {
                _seed = value;
                DefaultRS = new Random(_seed);
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public TimeSpan Duration { get; private set; }


        #region Future Event List
        /// <summary>
        /// Schedule an event to be invoked at the specified clock-time
        /// </summary>
        protected void Schedule(Action action, DateTime clockTime, string tag = null)
        {
            FutureEventList.Add(new Event(this, action, clockTime, tag));
        }

        /// <summary>
        /// Schedule an event to be invoked after the specified time delay
        /// </summary>
        protected void Schedule(Action action, TimeSpan delay, string tag = null)
        {
            FutureEventList.Add(new Event(this, action, ClockTime + delay, tag));
        }

        /// <summary>
        /// Schedule an event at the current clock time.
        /// </summary>
        protected void Schedule(Action action, string tag = null)
        {
            FutureEventList.Add(new Event(this, action, ClockTime, tag));
        }

        #endregion

        #region Simulation Run Control
        /// <summary>
        /// Gets the head event.
        /// </summary>
        internal Event GetHeadEvent()
        {
            Event headEvent = FutureEventList.FirstOrDefault();

            foreach (SandboxBase child in _childrenList)
            {
                var childHeadEvent = child.GetHeadEvent();
                if (headEvent == null || (childHeadEvent != null &&
                    EventComparer.Instance.Compare(childHeadEvent, headEvent) < 0))
                    headEvent = childHeadEvent;
            }

            return headEvent;
        }

        /// <summary>
        /// Gets the clock time.
        /// </summary>
        public DateTime ClockTime => Parent == null ? _clockTime : Parent.ClockTime;

        /// <summary>
        /// Gets the head event time.
        /// </summary>
        public DateTime? HeadEventTime => GetHeadEvent() == null ? null : (DateTime?)GetHeadEvent().ScheduledTime;

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public bool Run()
        {
            _isSimulationRunning = true;

            if (Parent != null)
                return Parent.Run();

            var head = GetHeadEvent();

            if (head != null)
            {
                head.FutureEventList.Remove(head);
                _clockTime = head.ScheduledTime;
                head.Invoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Runs the specified duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public bool Run(TimeSpan duration)
        {
            _isSimulationRunning = true;

            Duration = duration;

            return Parent != null ? Parent.Run(duration) : Run(ClockTime.Add(duration));
        }

        /// <summary>
        /// Runs the specified terminate.
        /// </summary>
        /// <param name="terminate">The terminate.</param>
        public bool Run(DateTime terminate)
        {
            _isSimulationRunning = true;

            if (Parent != null) return Parent.Run(terminate);

            while (!_isCancel)
            {
                if (GetHeadEvent() == null || GetHeadEvent().ScheduledTime > terminate)
                {
                    _clockTime = terminate;
                    return GetHeadEvent() != null; // if the simulation can be continued
                }
                else
                {
                    Run();
                }
            }

            return !_isCancel;
        }

        /// <summary>
        /// Runs the specified event count.
        /// </summary>
        /// <param name="eventCount">The event count.</param>
        public bool Run(int eventCount)
        {
            _isSimulationRunning = true;

            if (Parent != null)
                return Parent.Run(eventCount);

            while (!_isCancel && eventCount-- > 0)
                if (!Run()) return false;

            return true;
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            _isCancel = true;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            _clockTime = DateTime.MinValue;
            _realTimeForLastRun = null;
            _seed = 0;
            _count = 0;
            _isCancel = false;
            _isSimulationRunning = false;
        }

        /// <summary>
        /// Runs the specified speed.
        /// </summary>
        /// <param name="speed">The speed.</param>
        public bool Run(double speed)
        {
            _isSimulationRunning = true;

            if (Parent != null) return Parent.Run(speed);

            bool rtn = true;

            if (_realTimeForLastRun != null)
                rtn = Run(terminate: ClockTime.AddSeconds((DateTime.Now - _realTimeForLastRun.Value).TotalSeconds * speed));

            _realTimeForLastRun = DateTime.Now;

            return rtn;
        }
        #endregion

        #region Children - Sub-modules
        /// <summary>
        /// Gets the parent.
        /// </summary>
        public ISandbox Parent { get; private set; } = null;

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IReadOnlyList<ISandbox> Children { get { return _childrenList.AsReadOnly(); } }

        protected TSandbox AddChild<TSandbox>(TSandbox child) where TSandbox : SandboxBase
        {
            _childrenList.Add(child);
            child.Parent = this;
            _onWarmedUp += child._onWarmedUp;
            return child;
        }

        protected IReadOnlyList<HourCounter> HourCounters => _hourCounterList.AsReadOnly();

        protected HourCounter AddHourCounter(bool keepHistory = false)
        {
            var hc = new HourCounter(this, keepHistory);
            _hourCounterList.Add(hc);
            _onWarmedUp += () => hc.WarmedUp();
            return hc;
        }
        #endregion

        /// <summary>
        /// Converts to string.
        /// </summary> 
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = Id;
            if (str == null || str.Length == 0) str = GetType().Name;
            str += "#" + Index.ToString();
            return str;
        }

        #region Warm-Up
        /// <summary>
        /// Warms up.
        /// </summary>
        /// <param name="period">The period.</param>
        public bool WarmUp(TimeSpan period)
        {
            if (Parent != null) return Parent.WarmUp(period);
            return WarmUp(ClockTime + period);
        }

        /// <summary>
        /// Warms up.
        /// </summary>
        /// <param name="till">The till.</param>
        public bool WarmUp(DateTime till)
        {
            if (Parent != null) return Parent.WarmUp(till);
            var result = Run(till);
            _onWarmedUp.Invoke();
            return result; // to be continued
        }



        /// <summary>
        /// Warmeds up handler.
        /// </summary>
        protected virtual void WarmedUpHandler() { }
        #endregion

        #region For Logging

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the log file.
        /// </summary>
        //public string LogFile
        //{
        //    get { return _logFile; }
        //    set
        //    {
        //        _logFile = value; if (_logFile != null) using (var sw = new StreamWriter(_logFile)) { };
        //    }
        //}

        protected void Log(params object[] args)
        {
            if (_logger is null) return;

            var timeStr = ClockTime.ToString("HH:mm:ss.fff");

            var sb = new StringBuilder();

            sb.AppendFormat("ClockTime: {0}", timeStr);
            foreach (var arg in args)
                sb.AppendFormat("\t{0}", arg);

            _logger.Information(sb.ToString());
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug mode]; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugMode { get; set; } = false;

        /// <summary>
        /// Gets a value indicating whether this simulation is on cancel request.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cancel request; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancelRequest => _isCancel;

        /// <summary>
        /// Gets a value indicating whether this simulation is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is simulation running; otherwise, <c>false</c>.
        /// </value>
        public bool IsSimulationRunning => _isSimulationRunning;
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (_childrenList != null && _childrenList.Count > 0)
                foreach (var child in _childrenList) child?.Dispose();

            if (_hourCounterList != null && _hourCounterList.Count > 0)
                foreach (var hc in _hourCounterList) hc?.Dispose();
        }

    }
}
