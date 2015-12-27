///////////////////////////////////////////////////////////////
// Scheduler.cs - define methods to simplify display actions //
// Ver 1.1                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    MSI GE62, Core-i7, Windows 10                //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements scheduler. It will persist databse
 * on positive amount of time interval.
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, 
 *                 PersistEngine.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 13 Sep 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Timers;
using static System.Console;
namespace Project2Starter
{
    public class Scheduler
    {
        private static int _time_interval = 3000;
        // Creates time object
        public Timer schedular { get; set; } = new Timer();
        // setTimeINterval function to set the time interval to new int value (in ms)
        // by default value is 3000
        public void setTimeInterval(int newTimeinterval)
        {
            _time_interval = newTimeinterval;
        }
        
        // Scheduler consructor which takes type 1 database as an argument
        // and sets scheduler proeprties and starts until it is stopped.
        public Scheduler(DBEngine<int,DBElement<int,string>> db)
        {
            WriteLine("\n\n  Press any key to stop scheduler\n");
            schedular.Interval = _time_interval;
            schedular.AutoReset = true;
            schedular.Enabled = true;
            // Note use of timer's Elapsed delegate, binding to subscriber lambda
            // This delegate is invoked when the internal timer thread has waited
            // for the specified Interval.

            schedular.Elapsed += (object source, ElapsedEventArgs e) =>
            {
                PersistEngine p = new PersistEngine();
                p.persist_db_type1(db, p.getPDBType1FileName());
            };
            Console.ReadKey();
            stop();
        }
        // Scheduler consructor which takes type 2 database as an argument
        // and sets scheduler proeprties and starts until it is stopped.
        public Scheduler(DBEngine<string, DBElement<string, List<string>>> db)
        {
            WriteLine("\n\n  Press any key to stop scheduler\n");
            schedular.Interval = _time_interval;
            schedular.AutoReset = true;
            schedular.Enabled = true;
            // Note use of timer's Elapsed delegate, binding to subscriber lambda
            // This delegate is invoked when the internal timer thread has waited
            // for the specified Interval.

            schedular.Elapsed += (object source, ElapsedEventArgs e) =>
            {
                PersistEngine p = new PersistEngine();
                p.persist_db_type2(db, p.getPDBType2FileName());
            };
           Console.ReadKey();
            stop();
        }
        // stop function to disable the scheduler
        public void stop()
        {
            schedular.Enabled = false;
        }

    }
    class TestScheduler
    {
        static void Main(string[] args)
        {  
            DBElement<int, string> elem1 = new DBElement<int, string>("metadata1");
            elem1.timeStamp = DateTime.Parse("10/2/2013 12:32:11 AM");
            elem1.children = new List<int> { 2, 3, 4 };
            elem1.payload = "a payload of elem 1";
            DBElement<int, string> elem2 = new DBElement<int, string>("meacba2");
            elem2.timeStamp = DateTime.Parse("10/2/2014 12:32:11 AM");
            elem2.children = new List<int> { 3, 4 };
            elem2.payload = "a payload of elem 2";
            DBElement<int, string> elem3 = new DBElement<int, string>("metaabca3");
            elem3.timeStamp = DateTime.Parse("10/2/2015 12:32:11 AM");
            elem3.children = new List<int> { 5, 9 };
            elem3.payload = "a payload of elem 3";
            DBElement<int, string> elem4 = new DBElement<int, string>("abc");
            elem4.timeStamp = DateTime.Parse("10/2/2016 12:32:11 AM");
            elem4.children = new List<int> { 10, 12 };
            elem4.payload = "a payload of elem 4";
            DBElement<int, string> elem5 = new DBElement<int, string>("mabcdata5");
            elem5.timeStamp = DateTime.Parse("10/2/2017 12:32:11 AM");
            elem5.children = new List<int> { 15, 16, 18 };
            elem5.payload = "a payload of elem 5";
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            db.insert(1, elem1);
            db.insert(2, elem2);
            db.insert(3, elem3);
            db.insert(4, elem4);
            db.insert(5, elem5);
            Scheduler s = new Scheduler(db);
        

            WriteLine("\n Inserting type2 : <string , List<string>> Elements into database.");
            DBElement<string, List<string>> elem1_type2 = new DBElement<string, List<string>>("metxyzsd");
            elem1_type2.timeStamp = DateTime.Parse("10/2/2013 12:32:11 AM");
            elem1_type2.children = new List<string> { "c2", "c3", "c4" };
            elem1_type2.payload = new List<string> { "p11", "p12", "p13" };
            DBElement<string, List<string>> elem2_type2 = new DBElement<string, List<string>>("sxyasz");
            elem2_type2.timeStamp = DateTime.Parse("10/2/2014 12:32:11 AM");
            elem2_type2.children = new List<string> { "c10", "c53", "c41" };
            elem2_type2.payload = new List<string> { "p21", "p22", "p23" };
            DBElement<string, List<string>> elem3_type2 = new DBElement<string, List<string>>("xyz");
            elem3_type2.timeStamp = DateTime.Parse("10/2/2015 12:32:11 AM");
            elem3_type2.children = new List<string> { "c122", "c331", "c124" };
            elem3_type2.payload = new List<string> { "p31", "p32", "p33" };
            DBElement<string, List<string>> elem4_type2 = new DBElement<string, List<string>>("abcxyz");
            elem4_type2.timeStamp = DateTime.Parse("10/2/2016 12:32:11 AM");
            elem4_type2.children = new List<string> { "c253", "c312", "c424" };
            elem4_type2.payload = new List<string> { "p41", "p42", "p43" };
            DBEngine<string, DBElement<string, List<string>>> newdb =
              new DBEngine<string, DBElement<string, List<string>>>();
            newdb.insert("12key12", elem1_type2);
            newdb.insert("keykey", elem2_type2);
            newdb.insert("10asf", elem3_type2);
            newdb.insert("10ke2134y", elem4_type2);
            s.stop();
            s = new Scheduler(newdb);
            Read();
        }
    }
}
