///////////////////////////////////////////////////////////////
// Wpfclient :  WPF client to log perfromance                //
// Ver 2.2                                                   //
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
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *                    HiResTimer, XmlFactory
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
 * ver 1.0 : 11/12/2015
 * - Added tabs for readclient, writeclient, dboperations, and perfromance
 * - Added functionlaity to X number of send read messages by putting values in textboxes
 * - Added functionlaity to X number of send write messages by putting values in textboxes
 * - Server's average time to execute command is updated after each client done
 * - Performance data will be logged on perfromance tab 
 */

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Project4Starter;
using System.Threading;
using Project2Starter;
using System.Xml;
using HRTimer;

namespace Client_WPF
{
    public class DataObject
    {
        public string key { get; set; }
        public string desc { get; set; }
        public string name { get; set; }
        public string timestamp { get; set; }
        public string children { get; set; }
        public string payload { get; set; }
    }
    public partial class MainWindow : Window
    {
        static bool firstConnect = true;
        static Receiver rcvr = null;
        //static wpfSender sndr = null;
        static Sender sndr = null;

        string localAddress = "localhost";
        string localPort;
        string remotePort;
        public MainWindow()
        {
            InitializeComponent();
            lbl_Exec.Content = "Server's current average time to execute any command is -- microseconds.";
            string[] args = Environment.GetCommandLineArgs();
            if(args.Length == 3)
            {
                localPort = args[1];
                remotePort = args[2];
            }
            else
            {
                localPort = "8089";
                remotePort = "8080";
            }
            connect_Click(btn_connect, new RoutedEventArgs(Button.ClickEvent));
        }
        // ----< nested sender class   
        public class wpfSender : Sender
        {
            TextBox txt_status = null;  // reference to UIs local status textbox
            System.Windows.Threading.Dispatcher dispatcher_ = null;

            public wpfSender(TextBox txt_status, System.Windows.Threading.Dispatcher dispatcher)
            {
                dispatcher_ = dispatcher;  // use to send results action to main UI thread
                this.txt_status = txt_status;
            }
            public override void sendMsgNotify(string msg)
            {
                Action act = () => { txt_status.Text = msg; };
                dispatcher_.Invoke(act);

            }
            public override void sendExceptionNotify(Exception ex, string msg = "")
            {
                Action act = () => { txt_status.Text = ex.Message; };
                dispatcher_.Invoke(act);
            }
            public override void sendAttemptNotify(int attemptNumber)
            {
                Action act = null;
                act = () => { txt_status.Text = String.Format("attempt to send #{0}", attemptNumber); };
                dispatcher_.Invoke(act);
            }
        }
        string trim(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            for (int i = 0; i < sb.Length; ++i)
                if (sb[i] == '\n')
                    sb.Remove(i, 1);
            return sb.ToString().Trim();
        }
        //----< indirectly used by child receive thread to post results >----
        public void postRcvMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            if (content.ToLower().StartsWith("write"))
                lst_write_response.Items.Insert(0, content.Remove(0,5));
            else if((content.ToLower().StartsWith("read")))
                lst_read_response.Items.Insert(0, content.Remove(0, 4));
            else if ((content.ToLower().StartsWith("dbop")))
                lst_db_response.Items.Insert(0, content.Remove(0, 4));
            else if ((content.ToLower().StartsWith("<performance")))
            {
                StringBuilder sb = new StringBuilder();
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(content);
                sb.Append(String.Format("{0,-40} || ",xdoc.GetElementsByTagName("client_url").Item(0).InnerText));
                sb.Append(String.Format("{0,-25} || ", xdoc.GetElementsByTagName("client_type").Item(0).InnerText));
                sb.Append(String.Format("{0,-50} || ", xdoc.GetElementsByTagName("num_of_msgs").Item(0).InnerText));
                sb.Append(String.Format("{0,-50}", xdoc.GetElementsByTagName("time").Item(0).InnerText));
                lst_performance.Items.Insert(0, sb.ToString());
                lbl_Exec.Content = "Server's current average time to execute any command is " + xdoc.GetElementsByTagName("avgtime").Item(0).InnerText + " microseconds.";
            }
        }
        //----< used by main thread >----------------------------------------
        public void postSndMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            lst_write_send.Items.Insert(0, content);
        }
        //----< set up channel after entering ports and addresses >----------
        void setupChannel()
        {
            rcvr = new Receiver(localPort, localAddress);
            Action serviceAction = () =>
            {
                try
                {
                    Message rmsg = null;
                    while (true)
                    {
                        rmsg = rcvr.getMessage();
                        Action act = () => { postRcvMsg(rmsg.content); };
                        Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
                catch (Exception ex)
                {
                    Action act = () => { txt_status.Text = ex.Message; };
                    Dispatcher.Invoke(act);
                }
            };
            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction);
            }

            //sndr = new wpfSender(txt_status, this.Dispatcher);
            sndr = new Sender("http://localhost:" + localPort + "/CommService") ;
        }
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            localAddress = "localhost";
            
            if (firstConnect)
            {
                firstConnect = false;
                if (rcvr != null)
                    rcvr.shutDown();
                setupChannel();
            }
            txt_status.Text = "connect setup";
            btn_connect.IsEnabled = false;
            txt_radd.IsEnabled = false;
            txt_rport.IsEnabled = false;
            
        }
        // ----< this function will send read messages to server

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            Message msg = new Message();
            Action act;
            msg.fromUrl = "http://localhost:"+ localPort +"/CommService";
            msg.toUrl = "http://localhost:"+ remotePort +"/CommService";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(XMLFactory.XMLGenerator("read", msg.fromUrl));
            string dbtype = "listofstring";
            if (rb_int.IsChecked == true)
                dbtype = "string";
            XMLFactory.insertReadMessages(ref xmldoc, "type 1", dbtype, Int32.Parse(txt_qt1.Text), "keykey");
            XMLFactory.insertReadMessages(ref xmldoc, "type 2", dbtype, Int32.Parse(txt_qt2.Text), "12key12");
            XMLFactory.insertReadMessages(ref xmldoc, "type 3", dbtype, Int32.Parse(txt_qt3.Text), ".*ke.*y.*");
            XMLFactory.insertReadMessages(ref xmldoc, "type 4", dbtype, Int32.Parse(txt_qt4.Text), "xyz");
            int t5 = Int32.Parse(txt_qt5.Text);
            XMLFactory.insertReadMessages(ref xmldoc, "type 5", dbtype, t5 / 2, "9/2/2014 12:32:11 AM");
            t5 = t5 % 2 == 1 ? t5 / 2 + 1 : t5 / 2;
            XMLFactory.insertReadMessages(ref xmldoc, "type 5", dbtype, t5, "9/2/2014 12:32:11 AM", "11/2/2017 12:32:11 AM");
            XmlNodeList num_of_messages = xmldoc.GetElementsByTagName("num_of_messages");
            int numMsgs = 0;
            if (num_of_messages.Count > 0)
                numMsgs = Int32.Parse(num_of_messages.Item(0).InnerText);
            XmlNodeList Messages = xmldoc.GetElementsByTagName("message");
            int counter = 0;
            HiResTimer timer = new HiResTimer();
            timer.Start();
            while (counter < numMsgs)
            {
                msg.content = Messages.Item(counter).OuterXml;
                if (sndr.sendMessage(msg))
                {
                    act = () => {lst_read_send.Items.Insert(0, msg.toUrl + " " + msg.fromUrl);};
                    Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                    lst_read_send.Items.Insert(0, "Connection Failed.");
               Thread.Sleep(10);
                ++counter;
            }
            timer.Stop();
            ulong execTime = timer.ElapsedMicroseconds;
            msg.content = sendPerfromance(msg.fromUrl, numMsgs, execTime, "Read");
            sndr.sendMessage(msg);
            Thread.Sleep(10);
        }
        // ----< this function will disconnect from server and closes the wpf client
        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            sndr.shutdown();
            rcvr.shutDown();
            Application.Current.Shutdown();
        }
        // ----< create a xml message which will be sent to WPF client for logging performance of clients
        public string sendPerfromance(string fromUrl, int numMsgs, ulong execTime,string client_type)
        {
            string result = "<performance><client_url>";
            result += fromUrl + "</client_url><client_type>"+ client_type +"</client_type><num_of_msgs>";
            result += numMsgs + "</num_of_msgs><time>" + execTime + "</time></performance>";
            return result;
        }
        // ----< this function will send write messages to server
        private void btn_write_send_Click(object sender, RoutedEventArgs e)
        {
            Action act1;
            Message msg = new Message();
            msg.fromUrl = "http://localhost:" + localPort + "/CommService";
            msg.toUrl = "http://localhost:" + remotePort + "/CommService";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(XMLFactory.XMLGenerator("write", msg.fromUrl));
            string dbtype="listofstring";
            if (rb_int.IsChecked == true)
                dbtype = "string";
            XMLFactory.insertMessages(ref xmldoc, "add", Int32.Parse(txt_add_messages.Text), dbtype);
            XMLFactory.insertMessages(ref xmldoc, "edit", Int32.Parse(txt_edit_messages.Text), dbtype);
            XMLFactory.insertMessages(ref xmldoc, "delete", Int32.Parse(txt_delete_messages.Text), dbtype);
            XmlNodeList num_of_messages = xmldoc.GetElementsByTagName("num_of_messages");
            int numMsgs = 0;
            if (num_of_messages.Count > 0)
                numMsgs = Int32.Parse(num_of_messages.Item(0).InnerText);
            XmlNodeList Messages = xmldoc.GetElementsByTagName("message");
            int counter = 0;
            HiResTimer timer = new HiResTimer();
            timer.Start();
            try
            {
                while (counter < numMsgs)
                {
                    msg.content = Messages.Item(counter).OuterXml;
                    if (sndr.sendMessage(msg))
                        act1 = () => {lst_write_send.Items.Insert(0, msg.content);};
                    else
                        act1 = () => {lst_write_send.Items.Insert(0, "Connection Failed.");};
                    Dispatcher.Invoke(act1, System.Windows.Threading.DispatcherPriority.Background);
                    Thread.Sleep(10);
                    ++counter;
                }
                timer.Stop();
                ulong execTime = timer.ElapsedMicroseconds;
                msg.content = sendPerfromance(msg.fromUrl, numMsgs, execTime,"Write");
                sndr.sendMessage(msg);
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                act1 = () => {lst_write_send.Items.Insert(0, ex.ToString());};
                Dispatcher.Invoke(act1, System.Windows.Threading.DispatcherPriority.Background);
            }
        }
        // ----< this function will send persist message which will persist databse on server
        private void btn_persist_Click(object sender, RoutedEventArgs e)
        {
            string db_type = "";
            Message msg = new Message();
            msg.fromUrl = "http://localhost:" + localPort + "/CommService";
            msg.toUrl = "http://localhost:" + remotePort + "/CommService";
            if (rb_db_int.IsChecked == true)
                db_type = "string";
            else
                db_type = "listofstring";
            msg.content = "<message id=\"3241\"><command>persist</command><dbtype>"+ db_type + "</dbtype></message>";
            sndr.sendMessage(msg);
            Action act1 = () => {
                lst_db_send.Items.Insert(0, msg.content);
            };
            Dispatcher.Invoke(act1, System.Windows.Threading.DispatcherPriority.Background);
        }
        // ----< this function will send restore message which will restore databse on server
        private void btn_restore_Click(object sender, RoutedEventArgs e)
        {
            string db_type = "";
            Message msg = new Message();
            msg.fromUrl = "http://localhost:" + localPort + "/CommService";
            msg.toUrl = "http://localhost:" + remotePort + "/CommService";
            if (rb_db_int.IsChecked == true)
                db_type = "string";
            else
                db_type = "listofstring";
            msg.content = "<message id=\"3243\"><command>restore</command><dbtype>" + db_type + "</dbtype></message>";
            sndr.sendMessage(msg);
            Action act1 = () => {
                lst_db_send.Items.Insert(0, msg.content);
            };
            Dispatcher.Invoke(act1, System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
