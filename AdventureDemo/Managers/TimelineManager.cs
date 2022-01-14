using System;
using System.Collections.Generic;
using WaywardEngine;

namespace AdventureCore
{
    public class TimelineManager
    {
        #region Singleton

        private static TimelineManager _instance;
        public static TimelineManager instance {
            get {
                if( _instance == null ) {
                    _instance = new TimelineManager();
                }
                return _instance;
            }
        }

        private TimelineManager() { }

        #endregion

        #region Structs

        public delegate void TimelineDelegate(Dictionary<string, object> data);

        public struct TimelineEvent {
            public TimelineDelegate action { get; private set; }
            public double timestamp { get; private set; }
            public Dictionary<string, object> data { get; private set; }

            public TimelineEvent( TimelineDelegate action, Dictionary<string, object> data, double timestamp )
            {
                this.action = action;
                this.timestamp = timestamp;
                this.data = data;
            }
        }

        public class ClearEventFilter {
            public Dictionary<string, object> data;
            public double afterTime;
            public double beforeTime;

            public ClearEventFilter(Dictionary<string, object> data = null, double afterTime = 0, double beforeTime = double.MaxValue)
            {
                this.data = data;
                this.afterTime = afterTime;
                this.beforeTime = beforeTime;
            }
        }

        #endregion

        public double now { get; private set; }
        private List<TimelineEvent> timeline = new List<TimelineEvent>();

		#region Events

		public delegate void TimeAdvancedAction(double deltaTime);
        public event TimeAdvancedAction onTimeAdvanced;

        public event Action onTimeAdvanceStart;
        public event Action onTimeAdvanceEnd;

        public delegate void OnActionDelegate( Dictionary<string, object> data );
        public event OnActionDelegate OnActionEvent;
        public void OnAction(Dictionary<string, object> data)
        {
            OnActionEvent?.Invoke(data);
        }

        #endregion

        public bool RegisterEvent( TimelineDelegate action, Dictionary<string, object> data, double timeOut, double fromTime = -1 )
        {
            fromTime = fromTime == -1 ? now : fromTime;
            double timestamp = fromTime + timeOut;
            if( timestamp < now ) {
                Console.WriteLine("WARNING: Timeline Event was set to trigger before now.");
            }

            TimelineEvent e = new TimelineEvent( action, data, timestamp );
            bool done = false;
            for( int i = 0; i < timeline.Count; i++ ) {
                if( timestamp < timeline[i].timestamp ) {
                    timeline.Insert(i, e);
                    done = true;
                    break;
                }
            }

            if( !done ) {
                timeline.Add(e);
                done = true;
            }

            Console.WriteLine(ToString());

            return done;
        }
        
        public void ClearEvents(ClearEventFilter filter = null)
        {
            if( filter == null ) {
                timeline.Clear();
                return;
            }

            for( int i = timeline.Count - 1; i >= 0; i-- ) {
                bool match = true;

                if( filter.data != null ) {
                    foreach(KeyValuePair<string, object> criteria in filter.data) {
                        if( !timeline[i].data.ContainsKey(criteria.Key) || ( criteria.Value != null && timeline[i].data[criteria.Key] == criteria.Value) ) {
                            match = false;
                        }
                    }
                }

                if( timeline[i].timestamp > filter.beforeTime || timeline[i].timestamp < filter.afterTime ) {
                    match = false;
                }

                if( match ) {
                    timeline.RemoveAt(i);
                }
            }
        }

        public TimelineEvent GetNextEvent()
        {
            return timeline[0];
        }
        public TimelineEvent GetEventAt( int index )
        {
            return timeline[index];
        }
        public TimelineEvent GetLastEvent()
        {
            return timeline[timeline.Count - 1];
        }

        public void AdvanceTimeline( double time )
        {
            onTimeAdvanceStart?.Invoke();

            double startTime = now;
            double endTime = now + time;

            for( int i = 0; i < timeline.Count; i++ ) {
                TimelineEvent e  = timeline[i];

                if( e.timestamp > endTime ) {
                    break;
                }

                now = e.timestamp;
                onTimeAdvanced?.Invoke(now - startTime);

                e.action(e.data);
            }
            ClearEvents( new ClearEventFilter(null, 0, endTime) );

            Console.WriteLine(ToString());

            now = endTime;

            onTimeAdvanceEnd?.Invoke();
        }
    }
}
