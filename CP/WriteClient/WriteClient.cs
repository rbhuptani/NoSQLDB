///////////////////////////////////////////////////////////////
// WriteClient.cs - Console writeclient                      //
// Ver 2.2                                                   //
// Application: Demonstration for CSE681-SMA, Project#4      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Source Name: Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 * -                    HiResTimer,XMLFactory
 * 
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.2 : 11/12/2015
 * - will now create add,edit,delete XML messages runtime using XMLFactory
 * - will have option to switch whether to log messages on console or not
 * ver 2.1 : 29 Oct 2015
 * - fixed bug in processCommandLine(...)
 * - added rcvr.shutdown() and sndr.shutDown() 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Threading;
using HRTimer;
using Project2Starter;
using System.Xml;
namespace Project4Starter
{
    using Util = Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted

    class WriteClient
    {
        string localUrl { get; set; } = "http://localhost:8081/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";

        int addMsgs=5;
        int editMsgs = 5;
        int deleteMsgs=5;
        string dbtype = "string";
        bool no_log=false;
        //----< default constructor
        public WriteClient() { }
        // ----< declares constructor using which we can predefine how many add,edit,delete messages will be created
        public WriteClient(int am,int em,int dm,string dbt)
        {
            addMsgs=am;
            editMsgs = em;
            deleteMsgs=dm;
            dbtype = dbt;
        }
        //----< retrieve urls from the CommandLine if there are any >--------
        public void processCommandLine(string[] args)
        {
          if (args.Length == 0)
            return;
          localUrl = Util.processCommandLineForLocal(args, localUrl);
          remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
          if (Util.processCommandLineForLog(args, "").ToLower() == "true")
            no_log = true;
        }
        // ----< create a xml message which will be sent to WPF client for logging performance of clients
        public string sendPerfromance(string fromUrl, int numMsgs, ulong execTime)
        {
            string result = "<performance><client_url>";
            result += fromUrl + "</client_url><client_type>Write</client_type><num_of_msgs>";
            result += numMsgs + "</num_of_msgs><time>" + execTime + "</time></performance>";
            return result;
        }
        
        static void Main(string[] args)
        {
          Console.Write("\n  starting CommService client");
          Console.Write("\n =============================\n");
          Console.Title = "Write Client";
          WriteClient clnt = new WriteClient();
          clnt.processCommandLine(args);
          clnt.dbtype = Util.getDBType(args, "/dbt");
          clnt.addMsgs = Util.getmsgsCount(args, "/addmsgs");
          clnt.editMsgs = Util.getmsgsCount(args, "/editmsgs");
          clnt.deleteMsgs = Util.getmsgsCount(args, "/deletemsgs");
          if (clnt.dbtype != "string" || clnt.dbtype != "listofstring")
            clnt.dbtype = "listofstring";
          string localPort = Util.urlPort(clnt.localUrl);
          string localAddr = Util.urlAddress(clnt.localUrl);
          Receiver rcvr = new Receiver(localPort, localAddr);
          if (rcvr.StartService())
          {
                if(!clnt.no_log)
                    rcvr.doService(rcvr.defaultServiceAction());
                else
                {
                    Console.WriteLine("Client is recieving messages but are not logged to console as no_log flag is set.");
                    rcvr.doService(rcvr.sa_nolog());
                }
          }
          Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message
          Message msg = new Message();
          msg.fromUrl = clnt.localUrl;
          msg.toUrl = clnt.remoteUrl;
          Console.Write("\n  sender's url is {0}", msg.fromUrl);
          Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);
          if (!sndr.Connect(msg.toUrl))
          {
            Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
            sndr.shutdown();
            rcvr.shutDown();
            return;
          }
          // will create messages based on number of add,edit,delete messages defined
          XmlDocument xmldoc = new XmlDocument();
          xmldoc.LoadXml(XMLFactory.XMLGenerator("write", "address"));
          XMLFactory.insertMessages(ref xmldoc, "add",clnt.addMsgs,clnt.dbtype);
          XMLFactory.insertMessages(ref xmldoc, "edit", clnt.editMsgs, clnt.dbtype);
          XMLFactory.insertMessages(ref xmldoc, "delete", clnt.deleteMsgs, clnt.dbtype);
          XmlNodeList num_of_messages = xmldoc.GetElementsByTagName("num_of_messages");
          int numMsgs = 0;
          if(num_of_messages.Count>0)
            numMsgs = Int32.Parse(num_of_messages.Item(0).InnerText);
          XmlNodeList Messages = xmldoc.GetElementsByTagName("message");  
          int counter = 0;
          HiResTimer timer = new HiResTimer ();
          timer.Start();
          while (counter < numMsgs)
          {
            msg.content = Messages.Item(counter).OuterXml;
            if (!sndr.sendMessage(msg))
              return;
            Thread.Sleep(150);
            ++counter;
          }
          string mid="";
          try
          {
                mid = Messages.Item(counter-1).Attributes.GetNamedItem("id").Value;
                rcvr.setlastMID(mid);
                while (true)
                {
                    if (rcvr.getBool())
                    {
                        timer.Stop();
                        rcvr.setBool(false);
                        break;
                    }
                }
          }
          catch
          {
                    Console.WriteLine("errorr");
          }
          ulong execTime = timer.ElapsedMicroseconds;
          Console.WriteLine("Time taken to execute {0} commands : {1} microseconds.\n",numMsgs,execTime);
          msg.content = clnt.sendPerfromance(msg.fromUrl,numMsgs,execTime);
          sndr.sendMessage(msg);
          Thread.Sleep(500);
          msg.content = "done";
          sndr.sendMessage(msg);
          Util.waitForUser();
          rcvr.shutDown();
          sndr.shutdown();
          Console.Write("\n\n");
        }
  }
}
