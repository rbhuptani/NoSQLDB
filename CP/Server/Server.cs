///////////////////////////////////////////////////////////////
// Server.cs - CommService server                            //
// Ver 2.4                                                   //
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
 * Note:
 * - This server now receives, process and then sends back result messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.4 : 15 Nov 2015
 * - server will process messages which will be in xml format
 * - based on type of command, server will execute according database operation
 * - all the functionality of Project 2 can now be done remotely on console as well as wpf
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using Project2Starter;
using System;
using System.Collections.Generic;
using System.Xml;
using HRTimer;
using System.Threading;
namespace Project4Starter
{
    using Util = Utilities;
    using NoSQLDB = Project2Starter;
    class Server
    {
        string address { get; set; } = "localhost";
        string port { get; set; } = "8080";
        string wpf_port = "8089";
        static ulong total_commnds = 0;
        static ulong total_time =0;
        static ulong avg_time = 0;
        static HiResTimer timer;
        public NoSQLDB.DBEngine<string, NoSQLDB.DBElement<string, List<string>>> database =  new NoSQLDB.DBEngine<string, NoSQLDB.DBElement<string, List<string>>>();
        public NoSQLDB.DBEngine<int,NoSQLDB.DBElement<int,string>> database_type1 = new NoSQLDB.DBEngine<int, NoSQLDB.DBElement<int, string>>();
            //----< quick way to grab ports and addresses from commandline >-----

        public void ProcessCommandLine(string[] args)
        {
            
            if (args.Length > 0)
                port = args[0];
            if (args.Length > 1)
                address = args[1];
            if (args.Length > 2)
                wpf_port = args[2];
        }
        // ----< to get the avg time to exeucte any command based on current total time and total commands executed
        public ulong getAvg(ulong time,ulong commands)
        {
            if(commands != 0)
                return time / commands;
            return 0;
        }
        // ----< this function will process the message, fetch command type and execute database operation accordingly

        public string processMsg(string message)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(message);
            XmlNode Message = doc.DocumentElement;
            string dbtype = "",mId = "";
            try{
                mId = Message.Attributes.GetNamedItem("id").Value;
                dbtype = doc.GetElementsByTagName("dbtype").Item(0).InnerText;
            }
            catch(Exception ex){
                Console.WriteLine("Message id and dbtype are not present in this message.Exception message: {0}",ex.Message);
            }
            XmlNodeList commands = doc.GetElementsByTagName("command");
            string command = "";
            if (commands.Count > 0)
                command = commands.Item(0).InnerText;
            switch (command.ToLower()){
                case "add":
                    if (dbtype == "string")
                        return "write" + "Message #"+mId + " : " + addType1(doc);
                    return "write" + "Message #" + mId + " : " + add(doc);
                case "delete":
                    return "write" + "Message #" + mId + " : " + delete(doc);
                case "edit":
                    return "write" + "Message #" + mId + " : " +  edit(doc);
                case "query":
                    return "read"+ "Message #" + mId + " : " +  query(doc);
                case "persist":
                    return "dbop" + "Message #" + mId + " : " +  persist(doc);
                case "restore":
                    return "dbop" + "Message #" + mId + " : " + restore(doc);
                default:
                    return "Message #" + mId + " : " + "Invalid Command.";
            }
        }
        // ----< this function will add average time in performance tag which is sent to wpf client.
        public string addAvgTime(string content,ulong time)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(content);
            XmlNode root = xd.FirstChild;
            XmlNode avgTime = xd.CreateElement("avgtime");
            avgTime.InnerText = time.ToString();
            root.AppendChild(avgTime);
            return xd.OuterXml;
        }
        // ----< this function will convert string to int.
        public int stringtoint(string input)
        {
            int result = 0;
            try
            {
                result = Int32.Parse(input);
            }
            catch
            {
                result = -1;
            }
            return result;
        }
        //----<this function will insert eleement in type 1 database.
        public string addType1(XmlDocument doc)
        {
            NoSQLDB.DBElement<int, string> newElem = new NoSQLDB.DBElement<int, string>();
            int key = -1;
            XmlNodeList parameters = doc.GetElementsByTagName("parameters").Item(0).ChildNodes;
            List<int> echildren = new List<int>();
            List<string> ePayload = new List<string>();
            foreach (XmlNode xparam in parameters)
            {
                string type = xparam.Attributes.GetNamedItem("name").InnerText.ToLower();
                switch (type)
                {
                    case "key":
                        key = stringtoint(xparam.InnerText);
                        break;
                    case "name":
                        newElem.name = xparam.InnerText;
                        break;
                    case "description":
                        newElem.descr = xparam.InnerText;
                        break;
                    case "children":
                        echildren.Add(stringtoint(xparam.InnerText));
                        break;
                    case "payload":
                        newElem.payload =  xparam.InnerText;
                        break;
                }
            }
            newElem.children = echildren;
            if (key == -1)
                return "Key not Found in Add command";
            if (!database_type1.insert(key, newElem))
                return "Key = " + key + " already exists in database.";
            return " New Element inserted with Key = " + key;
        }
        // ----< this function will add element in type 2 database
        public string add(XmlDocument doc)
        {
            NoSQLDB.DBElement<string, List<string>> newElem = new NoSQLDB.DBElement<string, List<string>>();
            newElem.payload = new List<string>();
            string key="";
            XmlNodeList parameters = doc.GetElementsByTagName("parameters").Item(0).ChildNodes;
            List<string> echildren = new List<string>();
            List<string> ePayload = new List<string>();
            foreach (XmlNode xparam in parameters)
            {
                string type = xparam.Attributes.GetNamedItem("name").InnerText.ToLower();
                switch (type)
                {
                    case "key":
                        key = xparam.InnerText;
                        break;
                    case "name":
                        newElem.name = xparam.InnerText;
                        break;
                    case "description":
                        newElem.descr = xparam.InnerText;
                        break;
                    case "children":
                        echildren.Add(xparam.InnerText);
                        break;
                    case "payload":
                        ePayload.Add(xparam.InnerText);
                        break;
                }
            }
            newElem.children = echildren;
            newElem.payload = ePayload;
            if (key == "")
                return "Key not Found in Add command";
            if (!database.insert(key, newElem))
                return "Key = " + key + " already exists in database.";
            return " New Element inserted with Key = " + key;
        }
        // ----< this function will perform edit in type 2 database
        public string edit(XmlDocument doc)
        {
            string key="";
            XmlNodeList parameters = doc.GetElementsByTagName("parameters").Item(0).ChildNodes;
            string name = "default_editied_name", descr = "default_edited_description";
            List<string> echildren = new List<string>();
            List<string> ePayload = new List<string>();
            foreach (XmlNode xparam in parameters)
            {
                string type = xparam.Attributes.GetNamedItem("name").InnerText.ToLower();
                switch (type)
                {
                    case "key":
                        key = xparam.InnerText;
                        break;
                    case "name":
                        name = xparam.InnerText;
                        break;
                    case "description":
                        descr = xparam.InnerText;
                        break;
                    case "children":
                        echildren.Add(xparam.InnerText);
                        break;
                    case "payload":
                        ePayload.Add(xparam.InnerText);
                        break;
                }
            }
            if (key == "")
                return " Key not Found in Edit command.";
            NoSQLDB.DBElement<string, List<string>> oldElem = new NoSQLDB.DBElement<string, List<string>>();
            if (!database.getValue(key, out oldElem))
                return " Key = " + key + " does no exist in database.";
            database.editMetadata<List<string>>(key, name);
            database.editDescription<List<string>>(key, descr);
            database.editChildren<List<string>>(key, echildren);
            database.editPayload(key, ePayload);
            return " Element edited with Key = " + key;
        }
        // ----< this function will perform delete in type 1  and type 2 database
        public string delete(XmlDocument doc)
        {          
            string key="";
            string dbtype = doc.GetElementsByTagName("dbtype").Item(0).InnerText;
            XmlNodeList parameters = doc.GetElementsByTagName("parameters").Item(0).ChildNodes;
            foreach (XmlNode xparam in parameters)
            {
                string type = xparam.Attributes.GetNamedItem("name").InnerText.ToLower();
                if(type == "key")
                {
                    key = xparam.InnerText;
                    break;
                }
            }
            
            if (key == "")
                return "Key not Found in Delete command";
            if (dbtype == "string")
            {
                if (!database_type1.delete(stringtoint(key)))
                    return "Key = " + key + " does not exist in database.-string";
            }
            else
            {
                if (!database.delete(key))
                    return "Key = " + key + " does not exist in database.";
            }
            return " Element deleted with Key = " + key;
        }
        // ----< this function will perform different tpes of query on type 2 database.
        public string query(XmlDocument doc)
        {
            XmlNodeList parameters = doc.GetElementsByTagName("parameters").Item(0).ChildNodes;
            NoSQLDB.QueryEngine<string, NoSQLDB.DBElement<string, List<string>>, List<string>, string> qeType2 = new NoSQLDB.QueryEngine<string, NoSQLDB.DBElement<string, List<string>>, List<string>, string>(database);
            NoSQLDB.DBFactory<string, NoSQLDB.DBElement<string, List<string>>> dbf = new NoSQLDB.DBFactory<string, NoSQLDB.DBElement<string, List<string>>>();
            string type = parameters.Item(0).InnerText.ToLower();
            if (parameters.Count <= 1)
                return " Wrong imput.";
            string result="",queryDesc = "";
            switch (type)
            {
                case "type 1":
                    if (qeType2.Query1(parameters.Item(1).InnerText.ToLower(), ref result))
                        return result;
                    else
                        return " Key not Found.";
                case "type 2":
                    if (qeType2.Query2(parameters.Item(1).InnerText.ToLower(), out result))
                        return result;
                    else
                        return " Key not Found.";
                case "type 3":
                    Func<string, bool> queryType2_3 = qeType2.Query3(parameters.Item(1).InnerText.ToLower());
                    queryDesc = "Key with reguler expression " + parameters.Item(1).InnerText.ToLower();
                    return qeType2.QueryResult(qeType2.processQuery(queryType2_3, out dbf), dbf, queryDesc);
                case "type 4":
                    Func<string, bool> queryType2_4 = qeType2.Query4(parameters.Item(1).InnerText.ToLower());
                    queryDesc = "Metadata with Substring : " + parameters.Item(1).InnerText.ToLower();
                    return qeType2.QueryResult(qeType2.processQuery(queryType2_4, out dbf), dbf, queryDesc);
                case "type 5":
                    Func<string, bool> queryType2_5;
                    DateTime startTime = DateTime.Parse(parameters.Item(1).InnerText.ToLower());
                    DateTime endTime;
                    if (parameters.Count == 3)
                    {
                        endTime = DateTime.Parse(parameters.Item(2).InnerText.ToLower());
                        queryType2_5 = qeType2.Query5(startTime,endTime);
                    }
                    else
                        queryType2_5 = qeType2.Query5(startTime);
                    queryDesc = "Data with timestamp between " + startTime.ToString() + " and " + DateTime.Now.ToString();
                    return qeType2.QueryResult(qeType2.processQuery(queryType2_5, out dbf), dbf, queryDesc);
                default:
                    return " Invalide Query Type.";
            }
        }
        // ----< this function will restore both types of database based on the user input
        public string restore(XmlDocument doc)
        {
            NoSQLDB.PersistEngine p = new NoSQLDB.PersistEngine();
            string s = "";
            try
            {
                s = doc.GetElementsByTagName("dbtype").Item(0).InnerText;
            }
            catch { }
            if(s == "listofstring")
            {
                    if(p.restore_db_type2(ref database, p.getPDBType2FileName()))
                        return " Database of type < string, List<string>>  Restored.";
            }
            if(s == "string")
            {
                    if(p.restore_db_type1(ref database_type1, p.getPDBType1FileName()))
                        return " Database of type < int, string>  Restored.";
            }     
            return " Database is not persisted.";
        }
        // ----< this function will persist both types of database based on the user input
        public string persist(XmlDocument doc)
        {
            NoSQLDB.PersistEngine p = new NoSQLDB.PersistEngine();
            string s = "";
                try
                {
                    s = doc.GetElementsByTagName("dbtype").Item(0).InnerText;
                }
                catch { }
            if(s == "listofstring")
            {
                    if(p.persist_db_type2(database, p.getPDBType2FileName()))
                        return " Database of type < string, List < string > >  Persisted.";
            }
            if(s == "string")
            {
                    if(p.persist_db_type1(database_type1, p.getPDBType1FileName()))
                        return " Database of type < int, string >  Persisted.";
            }     
            return " Error : Database is not persisted.";
        }
        static void Main(string[] args)
        {
            Util.verbose = false;
            Server srvr = new Server();
            srvr.ProcessCommandLine(args);
            Console.Title = "Server";
            Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
            Console.Write("\n ====================================================\n");
            Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
            Receiver rcvr = new Receiver(srvr.port, srvr.address);
            PersistEngine p = new PersistEngine();
            string queryInput2 = "QueryInputType2.xml";
            p.restore_db_type2(ref srvr.database, queryInput2);
            Message msg_wpf = new Message();
            msg_wpf.fromUrl = Util.makeUrl(srvr.address, srvr.port);
            msg_wpf.toUrl = Util.makeUrl("localhost", srvr.wpf_port);
            Action serviceAction = () =>
            {
                Message msg = null;
                while (true)
                {
                    msg = rcvr.getMessage();   // note use of non-service method to deQ messages
                    Console.Write("\n  Received message:");
                    Console.Write("\n  sender is {0}", msg.fromUrl);
                    if (msg.content == "connection start message")
                        continue;
                    if (msg.content == "done")
                    {
                        Console.Write("\n  client has finished\n");
                        continue;
                    }
                    if (msg.content == "closeServer")
                    {
                        Console.Write("received closeServer");
                        break;
                    }
                    if(msg.content.ToLower().StartsWith("<message"))
                    {
                        Server.total_commnds++;
                        timer = new HiResTimer();
                        timer.Start();
                        msg.content = srvr.processMsg(msg.content);
                        timer.Stop();
                        ulong execTime = timer.ElapsedMicroseconds;
                        Server.total_time += execTime;
                        Server.avg_time = srvr.getAvg(Server.total_time,Server.total_commnds);
                        Util.swapUrls(ref msg);
                        sndr.sendMessage(msg);

                    }
                    else if (msg.content.ToLower().StartsWith("<performance"))
                    {

                        msg_wpf.content = srvr.addAvgTime(msg.content,Server.avg_time);
                        sndr.sendMessage(msg_wpf);
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Util.swapUrls(ref msg);
                        sndr.sendMessage(msg);
                    }
                }
            };
            if (rcvr.StartService())
                rcvr.doService(serviceAction); // This serviceAction asynchronous, so the call doesn't block. 
            Util.waitForUser();
        }
    }
}
