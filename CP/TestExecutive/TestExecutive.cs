///////////////////////////////////////////////////////////////
// TestExecutive.cs - To test all the requirments            //
// Ver 1.0                                                   //
// Application: Demonstration for CSE681-SMA, Project#4      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Source Name: Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////

/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *         DBEngine,DBElement,HiResTimer,QueryEngine,PersistEngine
 *
/*
 * Maintenance History:
 * --------------------
 * ver 1.0 : 18 Nov 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Console;
using Project2Starter;
using System.IO;
using System.Diagnostics;
using Project4Starter;
using System.Threading;

namespace Project4Starter
{
    class TestExecutive
    {
        static XmlDocument xdoc;
        static string address;
        static int server_port;
        static int wpfclient_port;
        static int read_start_port;
        static int write_start_port;
        static int num_of_read_clients;
        static int num_of_write_clients;
        static string read_dbtype;
        static string write_dbtype;
        static int num_of_read_msgs;
        static int num_of_add_msgs;
        static int num_of_edit_msgs;
        static int num_of_delete_msgs;
        static string read_log;
        static string write_log;
        public TestExecutive()
        {
            string fileName = XMLFactory.correct_path("config.xml");
            if (File.Exists(fileName))
            {
                try
                {
                    xdoc = new XmlDocument();
                    xdoc.Load(fileName); }
                catch (Exception e)
                { WriteLine("\n Wrong Input file. Error Message : {0}\n", e.StackTrace); }
            }
            else
                WriteLine("\n  File \" {0} \" does not exist.", fileName);
            
        }

        public bool setValues(XmlDocument xdoc)
        {

            try
            {
                XmlNodeList serverInfo = xdoc.GetElementsByTagName("server").Item(0).ChildNodes;
                address = serverInfo.Item(0).InnerText;
                server_port = Int32.Parse(serverInfo.Item(1).InnerText);
                XmlNodeList wpfclientInfo = xdoc.GetElementsByTagName("wpfclient").Item(0).ChildNodes;
                wpfclient_port = Int32.Parse(wpfclientInfo.Item(0).InnerText);
                XmlNodeList readclientInfo = xdoc.GetElementsByTagName("readclients").Item(0).ChildNodes;
                num_of_read_clients = Int32.Parse(readclientInfo.Item(0).InnerText);
                read_start_port = Int32.Parse(readclientInfo.Item(1).InnerText);
                read_log = readclientInfo.Item(2).InnerText.ToLower();
                read_dbtype = readclientInfo.Item(3).InnerText.ToLower();
                num_of_read_msgs = Int32.Parse(readclientInfo.Item(4).InnerText);
                XmlNodeList writeclientInfo = xdoc.GetElementsByTagName("writeclients").Item(0).ChildNodes;
                num_of_write_clients = Int32.Parse(writeclientInfo.Item(0).InnerText);
                write_start_port = Int32.Parse(writeclientInfo.Item(1).InnerText);
                write_log = writeclientInfo.Item(2).InnerText.ToLower();
                write_dbtype = writeclientInfo.Item(3).InnerText.ToLower();
                num_of_add_msgs = Int32.Parse(writeclientInfo.Item(4).InnerText);
                num_of_edit_msgs = Int32.Parse(writeclientInfo.Item(5).InnerText);
                num_of_delete_msgs = Int32.Parse(writeclientInfo.Item(6).InnerText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            return true; ; 
        }
        public string correct_path(string packagename)
        {
            string temp = AppDomain.CurrentDomain.BaseDirectory;
            int j = 0;
            while (j != 4)
            {
                int i = temp.LastIndexOf("\\");
                temp = temp.Substring(0, i);
                j++;
            }
            //temp = temp + "\\";
            packagename = temp + "\\" + packagename + "\\bin\\debug\\" + packagename + ".exe";
            return packagename;
        }
        public void TestR2()
        {
            "Demonstrating Requirement #2".title();
            Console.WriteLine("\nYou can look through Solution Explorer that I have used all the packages that i had created in Project 2.");
        }
        public void TestR3()
        {
            "Demonstrating Requirement #3".title();
            Console.WriteLine("\nYou can look in ICommService, CommService, Reciever.cs, Sender.cs, Server.cs, ReadClient.cs, WriteClient.cs, Client_WPF.cs that it uses WCF to communicate between Clients and Server.");
        }
        public void TestR4()
        {
            "Demonstrating Requirement #4".title();
            WriteLine("\n\t 1) You can look at outputs of WriteClients on WriteClient as well as on WriteClient tab on WPFClient that add,edit,delete messages are sent to server and results are sent back from server.");
            WriteLine("\t 2) You can look at outputs of ReadClient as well as on ReadClient tab on WPFClient that all types of queries are sent to server and results are sent back from server.");
            WriteLine("\t 3) You can look at outputs of DB Operations tab on WPFClient that Persist/Restore messages are sent to server and results are sent back from server.");
        }
        public void TestR5()
        {
            "Demonstrating Requirement #5".title();
            Console.WriteLine("\nWriteClients can be run through Console as well as WPF. Console clients can set number of messages from WriteClient.cs and WPF-WriteClient can change number of messages using add,edit,delete textboxes.");
            Console.WriteLine("Both Clients will measure time elapsed to execute N messages on server. Server will send this performance measure to WPF client which you can analyze in Performance tab. ");
            Console.WriteLine("XMLFactory will create random messages through insertMessages function for writeclient.");
        }
        public void TestR6()
        {
            "Demonstrating Requirement #6".title();
            Console.WriteLine("\nBoth read and write clients can determine if they want to log messages or not by option switch in config.xml file. It can be checked on Line number 108 in ReadClient.cs and WriteClient.cs. ");
            
        }
        public void TestR7()
        {
            "Demonstrating Requirement #7".title();
            Console.WriteLine("\nReadClients can be run through Console as well as WPF. Console clients can set number of messages from ReadClient.cs and WPF-ReadClient can change number of messages using query type textboxes.");
            Console.WriteLine("XMLFactory will create random messages through insertReadMessages function for readclient.");
        }
        public void TestR8()
        {
            "Demonstrating Requirement #8".title();
            Console.WriteLine("\nRead Clients can measure time elapsed to execute N messages on server. Server will send this performance measure to WPF client which you can analyze in Performance tab. ");
        }
        public void TestR10()
        {
            "Demonstrating Requirement #10".title();
            Console.WriteLine("\nNumber of Writers and Readers can be set from config.xml file.");
        }

        static void Main(string[] args)
        {
            TestExecutive starter = new TestExecutive();
            if (TestExecutive.xdoc == null ) { WriteLine("\n Invalid configuration file.\n");return; }
            if (!starter.setValues(TestExecutive.xdoc)) { WriteLine("\n Invalid configuration file.\n"); return; }
            string arg = TestExecutive.server_port + " " + TestExecutive.address + " " + TestExecutive.wpfclient_port;
            Process.Start(starter.correct_path("Server"),arg);
            arg = TestExecutive.wpfclient_port + " " + TestExecutive.server_port;
            Process.Start(starter.correct_path("Client_WPF"), arg);
            Thread.Sleep(100);
            int i = 0;
            while (i < num_of_read_clients)
            {
                arg = "/R http://localhost:"+ TestExecutive.server_port + "/CommService /L http://localhost:"+ (read_start_port + i) +"/CommService " + "/log " + read_log + " /dbt " + read_dbtype;
                arg += " /readmsgs " + num_of_read_msgs;
                Process.Start(starter.correct_path("ReadClient"), arg);
                i++;
            }
            i = 0;
            while (i < num_of_write_clients)
            {
                arg = "/R http://localhost:" + TestExecutive.server_port + "/CommService /L http://localhost:" + (write_start_port + i) + "/CommService " + "/log " + write_log;
                arg += " /dbt " + write_dbtype + " /addmsgs " + num_of_add_msgs + " /editmsgs " + num_of_edit_msgs + " /deletemsgs " + num_of_delete_msgs; 
                Process.Start(starter.correct_path("WriteClient"), arg);
                i++;
            }
            starter.TestR2();
            starter.TestR3();
            starter.TestR4();
            starter.TestR5();
            starter.TestR6();
            starter.TestR7();
            starter.TestR8();
        }
    }
}
