using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project4Starter;
using System.Threading;

namespace Client_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool firstConnect = true;
        static Receiver rcvr = null;
        static wpfSender sndr = null;
        string localAddress = "localhost";
        string localPort = "8089";
        string remoteAddress = "localhost";
        string remotePort = "8080";
        static Clients c = new Clients();
        public MainWindow()
        {
            InitializeComponent();
        }

        
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
                dispatcher_.Invoke(act,System.Windows.Threading.DispatcherPriority.Render);

            }
            public override void sendExceptionNotify(Exception ex, string msg = "")
            {
                Action act = () => { txt_status.Text = ex.Message; };
                dispatcher_.Invoke(act, System.Windows.Threading.DispatcherPriority.Render);
            }
            public override void sendAttemptNotify(int attemptNumber)
            {
                Action act = null;
                act = () => { txt_status.Text = String.Format("attempt to send #{0}", attemptNumber); };
                dispatcher_.Invoke(act, System.Windows.Threading.DispatcherPriority.Render);
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
            c.lst_read_response.Items.Insert(0, content);
        }
        //----< used by main thread >----------------------------------------

        public void postSndMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            c.lst_read_send.Items.Insert(0, content);
        }
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
                        Dispatcher.Invoke(act,System.Windows.Threading.DispatcherPriority.Send);
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
            
            sndr = new wpfSender(txt_status, this.Dispatcher);
        }
        //----< set up channel after entering ports and addresses >----------

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            localPort = "8089";
            localAddress = "localhost";
            remoteAddress = txt_radd.Text;
            remotePort = txt_rport.Text;

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

            Action act = () => { c.Show(); };
            Dispatcher.Invoke(act);
        }
    }
}
