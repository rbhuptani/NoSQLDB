///////////////////////////////////////////////////////////////
// XMLFactory.cs - define methods to simplify XML operations //
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
 * This package is updated to support readclients and writeclients 
 * to  create random XML messages which will be sent to server
 * This package implements XMLFactory. It will help PersistEngine
 * to persist database into xml file.
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 2.0 : 11/12/2015
 * - added functionlaity to add random read messages
 * - added functionality to add random write messages
 * ver 1.0 : 30 Sep 15
 * - first release
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using static System.Console;
namespace Project2Starter
{
    public class XMLFactory
    {
        // create function will create new xml file and set comments.
        public static Random rnd = new Random();
        public static List<string> keys = new List<string>();
        public static int create(string fileName,string comments = "type of this file was not mentioned in crreate function.")
        {
            fileName = correct_path(fileName);
            if (!File.Exists(fileName))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(@fileName, settings);
                if (writer != null)
                {
                    writer.WriteStartDocument();
                    writer.WriteComment(comments);
                    writer.WriteStartElement("nosqldb");
                    writer.WriteEndElement();
                    writer.WriteEndDocumentAsync();
                    writer.Flush();
                    writer.Close();
                    return 1; //new file created
                }
                else
                    return -1;  // -1 is for file does not exist and is not created.
            }
            else
                return 0; // file exists.  
        }
        //this function will reform the filepath
        public static string correct_path(string fileName)
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
            if (fileName.EndsWith(".xml"))
                fileName = temp + "\\" + fileName;
            else
                fileName = temp + "\\" + fileName + ".xml";
            return fileName;
        }
        //this function will convert the xml file into string
        public bool xmlTostring(string fileName, out string reader)
        {   
            string temp = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            if (fileName.EndsWith(".xml"))
                fileName = temp + "/" + fileName;
            else
                fileName = temp + "/" + fileName + ".xml";
            if (File.Exists(fileName))
            {
                reader = System.IO.File.ReadAllText(fileName) + "\n";
                return true;
            }
            reader = "";
            return false;
        }
        // this function will generate XML declaration and other needed details
        public static string XMLGenerator(string client_type,string client_address)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlNode rootNode = doc.CreateElement("client_message");
            doc.AppendChild(rootNode);
            XmlNode xclient_type = doc.CreateElement("client_type");
            xclient_type.InnerText = client_type;
            XmlNode xclient_address = doc.CreateElement("client_address");
            xclient_address.InnerText = client_address;
            XmlNode num_of_messages = doc.CreateElement("num_of_messages");
            num_of_messages.InnerText = "0";
            rootNode.AppendChild(xclient_type);
            rootNode.AppendChild(xclient_address);
            rootNode.AppendChild(num_of_messages);
            return doc.OuterXml;
        }
        // this function will fetch num_of_msg tag and increment it
        public static void incr_num_of_messages(ref XmlDocument xmldoc)
        {
            XmlNode num_of_messages = xmldoc.GetElementsByTagName("num_of_messages").Item(0);
            int size = Int32.Parse(num_of_messages.InnerText) + 1;
            num_of_messages.InnerText = size.ToString();
        }
        // this function will create a message element based on commandname
        public static void insertMessage(ref XmlDocument doc, int id,string commandName,string dbtype)
        {
            XmlNode root = doc.DocumentElement;
            Parameters parameters = createElement(commandName,rnd.Next(0, 10), rnd.Next(0, 10));
            XmlElement message = doc.CreateElement("message");
            message.SetAttribute("id", id.ToString());
            XmlNode command = doc.CreateElement("command");
            command.InnerText = commandName.ToLower();
            message.AppendChild(command);
            XmlNode xdbtype = doc.CreateElement("dbtype");
            xdbtype.InnerText = dbtype.ToLower();
            message.AppendChild(xdbtype);
            XmlNode xparameters = doc.CreateElement("parameters");
            int i = 1;
            foreach (KeyValuePair<string, string> param in parameters)
            {
                XmlElement xparam = doc.CreateElement("param");
                xparam.SetAttribute("id", i.ToString());
                xparam.SetAttribute("name", param.Key);
                xparam.InnerText = param.Value;
                if (dbtype == "string" && param.Key.ToLower() == "key")
                    xparam.InnerText = param.Value.Replace("key_", ""); ;
                xparameters.AppendChild(xparam);
                i++;
            }
            message.AppendChild(xparameters);
            root.AppendChild(message);
            incr_num_of_messages(ref doc);
        }
        //this function will create parameters to insert into message
        public static Parameters createElement(string commandName, int num_of_children,int num_of_payload)
        {
            Parameters parameters = new Parameters();
            string prefix = "";
            if(commandName.ToLower() == "add")
            {
                string key = "key_" +  rnd.Next(1, 65535).ToString();
                keys.Add(key);
                parameters.Add("key", key);
            }
            if (commandName.ToLower() == "edit" || commandName.ToLower() == "delete")
            {
                string key = "";
                if (keys.Count > 0)
                    key = keys[rnd.Next(0, keys.Count - 1)];
                else key = "key_" + rnd.Next(1, 65535).ToString();
                parameters.Add("key", key);
                prefix = "edited_";
                if (commandName.ToLower() == "delete")
                    return parameters;
            }
            parameters.Add("name", prefix + "name_" + rnd.Next(1, 65535));
            parameters.Add("description",prefix + "description_"+ rnd.Next(1, 65535));
            int i = num_of_children;
            while (i > 0)
            {
                parameters.Add("children", prefix + "child_" + rnd.Next(1, 65535));
                i--;
            }
            i = num_of_payload;
            while (i > 0)
            {
                parameters.Add("payload",prefix +  "payload_" + rnd.Next(1, 65535));
                i--;
            }
            return parameters;
        }
        //this function will create write rmessages based on given command, num_of_messages and dbtype
        public static void insertMessages(ref XmlDocument xmldoc,string command,int num_of_messages,string dbtype)
        {
            int i = 1;
            while (i <= num_of_messages)
            {
                XMLFactory.insertMessage(ref xmldoc, rnd.Next(0,65535), command,dbtype);
                i++;
            }
        }
        //this function will create read rmessages based on given query type, num_of_messages and dbtype
        public static void insertReadMessages(ref XmlDocument xmldoc,string type,string dbtype,int num_of_messages,string param1,string param2="")
        {
            int i = 1;
            while (i <= num_of_messages)
            {
                XMLFactory.insertReadMessage(ref xmldoc, rnd.Next(0, 65535), type,dbtype,param1,param2);
                i++;
            }
        }
        // this function will create a read message element based on query type and unput parameters
        private static void insertReadMessage(ref XmlDocument doc, int id, string type,string dbtype,string param1,string param2="")
        {
            XmlNode root = doc.DocumentElement;
            XmlElement message = doc.CreateElement("message");
            message.SetAttribute("id", id.ToString());
            XmlNode command = doc.CreateElement("command");
            command.InnerText = "query";
            message.AppendChild(command);
            XmlNode xdbtype = doc.CreateElement("dbtype");
            xdbtype.InnerText = dbtype;
            message.AppendChild(xdbtype);
            XmlNode xparameters = doc.CreateElement("parameters");
            XmlElement xparam = doc.CreateElement("param");
            xparam.SetAttribute("name", "type"); xparam.InnerText = type; xparameters.AppendChild(xparam);
            XmlElement xparam1 = doc.CreateElement("param");
            xparam1.SetAttribute("name", "input1"); xparam1.InnerText = param1; xparameters.AppendChild(xparam1);
            if(param2 != "")
            {
                XmlElement xparam2 = doc.CreateElement("param");
                xparam2.SetAttribute("name", "input2"); xparam2.InnerText = param2; xparameters.AppendChild(xparam2);
            }
                
            message.AppendChild(xparameters);
            root.AppendChild(message);
            incr_num_of_messages(ref doc);
        }
    }
    public class Parameters : List<KeyValuePair<string, string>>
    {
        public void Add(string key, string value)
        {
            var element = new KeyValuePair<string, string>(key, value);
            this.Add(element);
        }
    }

    class TestXMLFactory
    {
        static void Main(string[] args)
        {
            "Testing XMLFactory".title();
            /*Console.WriteLine("\n Showing Package structure of current solution.\n");
            string reader;
            XMLFactory x = new XMLFactory(); 
            x.xmlTostring("ProjectStructure", out reader);
            Console.WriteLine(reader);
            
            Console.WriteLine(XMLFactory.correct_path("abc"));*/
            Console.WriteLine();

            /*XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(XMLFactory.XMLGenerator("read", "address"));
            XMLFactory.insertReadMessages(ref xmldoc, "type 1", 3, "key");
            XMLFactory.insertReadMessages(ref xmldoc, "type 2", 3, "type2");
            XMLFactory.insertReadMessages(ref xmldoc, "type 3", 3, "type3");
            XMLFactory.insertReadMessages(ref xmldoc, "type 4", 3, "type4");
            XMLFactory.insertReadMessages(ref xmldoc, "type 5", 3, "type5");
            XMLFactory.insertReadMessages(ref xmldoc, "type 5", 3, "type6", "type7");
            xmldoc.Save("readclient.xml");

            XMLFactory.insertMessages(ref xmldoc, "add", 50);
            Console.WriteLine(xmldoc.OuterXml);
            */
        }
    }
}
