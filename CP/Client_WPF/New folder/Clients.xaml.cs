using Project2Starter;
using Project4Starter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Client_WPF
{
    /// <summary>
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        public Clients()
        {
            InitializeComponent();
        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            Message msg = new Message();
            msg.fromUrl = "http://localhost:8089/CommService";
            msg.toUrl = "http://localhost:8080/CommService";
            XmlDocument xmldoc = new XmlDocument();
            //xmldoc.LoadXml(XMLFactory.XMLGenerator("read", "address"));
            Receiver rcvr = new Receiver("8089", "localhost");
            if (rcvr.StartService())
            {
                rcvr.doService(rcvr.defaultServiceAction());
            }

            Sender sndr = new Sender(msg.fromUrl);
            string fileName = XMLFactory.correct_path("qi.xml");
            if (File.Exists(fileName))
            {
                try
                { xmldoc.Load(@fileName); }
                catch (Exception ex)
                { Console.WriteLine("\n Wrong Input file. Error Message : {0}\n", ex.Message); }
            }
            XMLFactory.insertMessages(ref xmldoc, "add", Int32.Parse(txt_add_messages.Text));
            XMLFactory.insertMessages(ref xmldoc, "edit", Int32.Parse(txt_edit_messages.Text));
            XMLFactory.insertMessages(ref xmldoc, "delete", Int32.Parse(txt_delete_messages.Text));
            XmlNodeList num_of_messages = xmldoc.GetElementsByTagName("num_of_messages");
            int numMsgs = 0;
            if (num_of_messages.Count > 0)
                numMsgs = Int32.Parse(num_of_messages.Item(0).InnerText);
            XmlNodeList Messages = xmldoc.GetElementsByTagName("message");

            int counter = 0;
            while (counter <= numMsgs)
            {
                //msg.content = "Message #" + (++counter).ToString();
                msg.content = Messages.Item(counter).OuterXml;
                lst_read_send.Items.Insert(0, msg.content);
                
                //Console.Write("\n  sending {0}", msg.content);
                if (!sndr.sendMessage(msg))
                {
                    Thread.Sleep(10);
                    return;
                }
                    

                Thread.Sleep(10);
                ++counter;
            }
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
