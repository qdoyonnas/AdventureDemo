using System;
using System.Collections.Generic;
using WaywardEngine;

namespace AdventureDemo
{
    class TimelineManager
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

        public delegate void TimelineDelegate();

        public struct TimelineEvent {
            public TimelineDelegate action { get; private set; }
            public GameObject gameObject { get; private set; }
            public Verb verb { get; private set; }
            public double timestamp { get; private set; }

            public TimelineEvent( TimelineDelegate action, GameObject gameObject, Verb verb, double timestamp )
            {
                this.action = action;
                this.gameObject = gameObject;
                this.verb = verb;
                this.timestamp = timestamp;
            }
        }

        public class ClearEventFilter {
            public GameObject gameObject;
            public double afterTime;
            public double beforeTime;

            public ClearEventFilter(GameObject gameObject = null, double afterTime = 0, double beforeTime = double.MaxValue)
            {
                this.gameObject = gameObject;
                this.afterTime = afterTime;
                this.beforeTime = beforeTime;
            }
        }

        public delegate void OnActionDelegate(GameObject gameObject, Verb verb, double timestamp);
        public event OnActionDelegate OnAction;

        #endregion

        public double now { get; private set; }
        private List<TimelineEvent> timeline = new List<TimelineEvent>();

        public bool RegisterEvent( TimelineDelegate action, GameObject gameObject, double timeOut, double fromTime = -1 )
        {
            fromTime = fromTime == -1 ? now : fromTime;
            double timestamp = fromTime + timeOut;
            if( timestamp < now ) {
                Console.WriteLine("WARNING: Timeline Event was set to trigger before now.");
            }

            TimelineEvent e = new TimelineEvent( action, gameObject, timestamp );
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
                if( filter.gameObject == null || timeline[i].gameObject == filter.gameObject ) {
                    if( timeline[i].timestamp <= filter.beforeTime && timeline[i].timestamp >= filter.afterTime ) {
                        timeline.RemoveAt(i);
                    }
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
            double endTime = now + time;

            foreach( TimelineEvent e in timeline ) {
                if( e.timestamp > endTime ) {
                    break;
                }

                now = e.timestamp;
                e.action();

            }
            ClearEvents( new ClearEventFilter(null, 0, endTime) );

            Console.WriteLine(ToString());

            now = endTime;
        }

        public override string ToString()
        {
            string s = "Current Timeline\n";
            s += "-----------------\n";
            foreach( TimelineEvent e in timeline ) {
                s += $"{e.gameObject.GetName().text} : {e.action} : {e.timestamp}\n";
            }
            s += "-----------------\n";

            return s;
        }
    }
}
