using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
//using System.Timers;

namespace TaskScheduler_example
{
    class Program
    {
        static void Main( string[] args )
        {
            //TaskScheduler ts = new TaskScheduler();
            //ts.ScheduleTask();
            //Console.ReadLine();
            //ts.ScheduleTask( new TimeSpan( 11, 15, 00 ) );

            TaskScheduler.Instance.ScheduleTask( 14, 17, 0.0027777777777778,
            () =>
            {
                TaskScheduler.TaskToStart();
            } );

            Console.ReadLine();
        }
    }

    public class TaskScheduler
    {
        private static TaskScheduler _instance;
        //private List<Timer> timers = new List<Timer>();

        private TaskScheduler() { }

        public static TaskScheduler Instance => _instance ?? (_instance = new TaskScheduler());

        public void ScheduleTask( int hour, int min, double intervalInHour, Action task )
        {
            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime( now.Year, now.Month, now.Day, hour, min, 0, 0 );
            //double checkPeriodInHour = 0.001;
            if( now > firstRun )
            {
                firstRun = firstRun.AddDays( 1 );
            }

            TimeSpan timeToGo = firstRun - now;
            if( timeToGo <= TimeSpan.Zero )
            {
                timeToGo = TimeSpan.Zero;
            }

            var timer = new Timer( x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours( intervalInHour ) );

            //timers.Add( timer );
        }

        public static void TaskToStart()
        {
            Console.WriteLine( "Task started at: " + DateTime.Now );
            Console.ReadKey();
        }
    }

    //public class TaskScheduler
    //{
    //    //private System.Threading.Timer timer;
    //    //const double interval60Minutes = 1000;
    //    const double interval = 0.001;
    //    //TimeSpan TimeToStart = new TimeSpan( 13, 45, 00 );

    //    public void ScheduleTask()
    //    {
    //        Timer checkForTime = new Timer( interval );
    //        checkForTime.Elapsed += new ElapsedEventHandler( CheckForTime_Elapsed );
    //        checkForTime.Enabled = true;
    //    }

    //    void CheckForTime_Elapsed( object sender, ElapsedEventArgs e )
    //    {
    //        if( TimeIsReady() )
    //        {
    //            TaskToStart();
    //        }
    //    }

    //    private bool TimeIsReady()
    //    {
    //        DateTime current = DateTime.Now;
    //        TimeSpan TimeToStart = new TimeSpan( 12, 48, 00 );
    //        TimeSpan timeToGo = TimeToStart - current.TimeOfDay;

    //        if( timeToGo < TimeSpan.Zero )
    //        {
    //            //Console.WriteLine( "Check time " + DateTime.Now );
    //            return false; //time already passed
    //        }
    //        else
    //        {
    //            return true;
    //        }
    //    }

    //    public static void TaskToStart()
    //    {
    //        Console.WriteLine( "Task started at: " + DateTime.Now );
    //        Console.ReadKey();
    //    }
    //}


    //public void ScheduleTask( TimeSpan alertTime )
    //{
    //    DateTime current = DateTime.Now;
    //    TimeSpan timeToGo = alertTime - current.TimeOfDay;
    //    if( timeToGo < TimeSpan.Zero )
    //    {
    //        return; //time already passed
    //    }
    //    this.timer = new System.Threading.Timer( x =>
    //    {
    //        this.ShowMessageToUser();
    //    }, null, timeToGo, Timeout.InfiniteTimeSpan );
    //}

    //private void ShowMessageToUser()
    //{
    //    Console.WriteLine( "Task started at: " + DateTime.Now );
    //    Console.ReadKey();
    //}
    //}


}
