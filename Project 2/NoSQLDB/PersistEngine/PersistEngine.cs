///////////////////////////////////////////////////////////////
// PersistEngine.cs - Test DBEngine and DBExtensions         //
// Ver 1.1                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    MSI GE62, Core-i7, Windows 10                //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////
/*
* Following Project requirements are met here :
*   - Requirement 5:  1) persist database contents to an XML file
*                     2) Restore databse from an XML file
*                     3) Augment datase from an XML file
*/

/*
 * Package Operations: 
 * -------------------
 * This package fulfills requirement 5 of thi project.
 * Using this package we can perform persisting to an xml file,
 * restoring and augmenting database from an xml file
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBElement.cs, DBEngine.cs,  
 *   DBExtensions.cs, UtilityExtensions.cs
 *   XMLFactory.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 1 Oct 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Xml;
namespace Project2Starter
{
    public class PersistEngine
    {
        private string _PERSISTED_XML_TYPE1;// = "NoSQLDBType1";
        private string _PERSISTED_XML_TYPE2;// = "NoSQLDBType2";
        
        public PersistEngine()
        {
            _PERSISTED_XML_TYPE1 = "NoSQLDBType1";
            _PERSISTED_XML_TYPE2 = "NoSQLDBType2";
        }
        //returns filepath of <int,string> type of db
        public string getPDBType1FileName()
        {
            return _PERSISTED_XML_TYPE1;
        }
        //returns adfilepathdress of <string,List<string>> type of db
        public string getPDBType2FileName()
        {
            return _PERSISTED_XML_TYPE2;
        }
        // set the default database type 1 file name 
        public void setPDBType1FileName(string fileName)
        {
            _PERSISTED_XML_TYPE1 = fileName;
        }
        // set the default database type 2 file name 
        public void setPDBType2FileName(string fileName)
        {
            _PERSISTED_XML_TYPE2 = fileName;
        }
        // persist metadata of type1 and type 2
        public void persist_metadata<Key,Value>(DBElement<Key,Value> elem,Key key, ref XmlDocument xdoc, ref XmlNode root, ref XmlElement item,ref XmlElement element)
        {
            XmlElement name = xdoc.CreateElement("name");
            name.InnerText = elem.name;
            XmlElement desc = xdoc.CreateElement("description");
            desc.InnerText = elem.descr;
            XmlElement timestamp = xdoc.CreateElement("timestamp");
            timestamp.InnerText = elem.timeStamp.ToString();
            XmlElement children = xdoc.CreateElement("children");
            XmlElement categories = xdoc.CreateElement("categories");
            if (elem.children.Count() > 0)
            {
                foreach (Key c in elem.children)
                {
                    XmlElement child = xdoc.CreateElement("key");
                    child.InnerText = c.ToString();
                    children.AppendChild(child);
                }
            }
            if (elem.category.Count() > 0)
            {
                foreach (string c in elem.category)
                {
                    XmlElement child = xdoc.CreateElement("category");
                    child.InnerText = c.ToString();
                    categories.AppendChild(child);
                }
            }
            element.AppendChild(name);
            element.AppendChild(desc);
            element.AppendChild(timestamp);
            element.AppendChild(children);
            element.AppendChild(categories);
        }
        // persist payload of element of type 1
        public bool persist_element_type1<Key, Value , Data>(DBEngine<Key,Value> db,Key key,ref XmlDocument xdoc,ref XmlNode root,string filename)
        {
            Value value;
            if (db.getValue(key,out value)) {
                if (check_key<Key>(key, filename))  // add to database only if the key does not exist
                {
                    XmlElement item = xdoc.CreateElement("item");
                    item.SetAttribute("key", key.ToString());
                    XmlElement keyid = xdoc.CreateElement("key"); //Create key node
                    keyid.InnerText = key.ToString();
                    item.AppendChild(keyid);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                     // Creating item node
                    XmlElement element = xdoc.CreateElement("element");
                    persist_metadata<Key, Data>(elem, key, ref xdoc, ref root, ref item,ref element);
                    XmlElement payload = xdoc.CreateElement("payload");
                    payload.InnerText = elem.payload.ToString();
                    element.AppendChild(payload);
                    item.AppendChild(element);
                    root.AppendChild(item);
                    return true;
                }
                else
                {
                    Write("\n Key {0} already exists in Persisted database.", key);
                    return true;
                }
            }
            return false;
        }
        // persist payload of element of type 2
        public bool persist_element_type2<Key, Value, Data,T>(DBEngine<Key, Value> db, Key key, ref XmlDocument xdoc, ref XmlNode root, string filename) where Data : IEnumerable<T>
        {

            Value value;
            if (db.getValue(key, out value))
            {
                if (check_key<Key>(key, filename))  // add to database only if the key does not exist
                {
                    XmlElement item = xdoc.CreateElement("item");
                    item.SetAttribute("key", key.ToString());
                    XmlElement keyid = xdoc.CreateElement("key"); //Create key node
                    keyid.InnerText = key.ToString();
                    item.AppendChild(keyid);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    XmlElement element = xdoc.CreateElement("element");
                    persist_metadata<Key, Data>(elem, key, ref xdoc, ref root, ref item, ref element);
                    XmlElement payload = xdoc.CreateElement("payload");
                    if (elem.payload != null)
                    {
                        IEnumerable<object> p_items = elem.payload as IEnumerable<object>;
                        foreach (Key p in p_items)
                        {
                            XmlElement payload_item = xdoc.CreateElement("data");
                            payload_item.InnerText = p.ToString();
                            payload.AppendChild(payload_item);
                        }

                    }
                    element.AppendChild(payload);
                    item.AppendChild(element);
                    root.AppendChild(item);
                    return true;
                }
                else
                {
                    Write("\n Key {0} already exists in Persisted database.", key);
                    return true;
                }
            }
            return false;
        }
        //checks if key exist in persisted databsae
        public bool check_key<Key>(Key key, string fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            XmlNode root;
            xdoc.Load(@fileName);
            root = xdoc.GetElementsByTagName("nosqldb").Item(0);
            XmlNodeList items = xdoc.GetElementsByTagName("item");
            foreach(XmlNode item in items)
            {
                string key_value = item.Attributes.GetNamedItem("key").Value;
                if (key_value == key.ToString())
                    return false;
            }
            return true;
        }
        //to persist database of type 1 : <int,string>
        public bool persist_db_type1(DBEngine<int, DBElement<int, string>> db, string fileName)
        {
            XMLFactory.create(fileName);
            XmlDocument xdoc = new XmlDocument();
            XmlNode root;
            fileName = XMLFactory.correct_path(fileName);
            if (File.Exists(fileName))
                xdoc.Load(@fileName);
            else
            {
                WriteLine("\n  File \" {0} \" does not exist.");
                return false;
            }
            bool flag = false;
            root = xdoc.GetElementsByTagName("nosqldb").Item(0);
            XmlNodeList keycheck = xdoc.GetElementsByTagName("keytype");
            XmlNodeList payloadchek = xdoc.GetElementsByTagName("payloadtype");
            if (keycheck.Count == 0 && payloadchek.Count == 0)
            {
                XmlElement keytype = xdoc.CreateElement("keytype");
                XmlElement payloadtype = xdoc.CreateElement("payloadtype");
                keytype.InnerText = "int";
                payloadtype.InnerText = "string";
                root.AppendChild(keytype);
                root.AppendChild(payloadtype);
                flag = true;
            }
            else
            {
                string keytype = xdoc.GetElementsByTagName("keytype").Item(0).InnerText;
                string payloadtype = xdoc.GetElementsByTagName("payloadtype").Item(0).InnerText;
                if (keytype == "int" && payloadtype == "string")
                    flag = true;
                else
                    return false;
            }
            if (flag)
            {
                IEnumerable<int> keys = db.Keys();
                foreach (int k in keys)
                {
                    persist_element_type1<int, DBElement<int, string>, string>(db, k,ref xdoc,ref root,fileName);
                }
            }
            xdoc.Save(@fileName);
            WriteLine("\n ***** Database persisted to XML file located at \" {0} \"", fileName);
            return true;
        }
        //to persist database of type 2 : <string,List<string>>
        public bool persist_db_type2(DBEngine<string, DBElement<string, List<string>>> db, string fileName)
        {
            XMLFactory.create(fileName);
            XmlDocument xdoc = new XmlDocument();
            XmlNode root;
            fileName = XMLFactory.correct_path(fileName);
            if (File.Exists(fileName))
                xdoc.Load(@fileName);
            else
            {
                WriteLine("\n  File \" {0} \" does not exist.");
                return false;
            }
            bool flag = false;
            root = xdoc.GetElementsByTagName("nosqldb").Item(0);
            XmlNodeList keycheck = xdoc.GetElementsByTagName("keytype");
            XmlNodeList payloadchek = xdoc.GetElementsByTagName("payloadtype");
            if (keycheck.Count == 0 && payloadchek.Count == 0)
            {
                XmlElement keytype = xdoc.CreateElement("keytype");
                XmlElement payloadtype = xdoc.CreateElement("payloadtype");
                keytype.InnerText = "string";
                payloadtype.InnerText = "listofstring";
                root.AppendChild(keytype);
                root.AppendChild(payloadtype);
                flag = true;
            }
            else
            {
                string keytype = xdoc.GetElementsByTagName("keytype").Item(0).InnerText;
                string payloadtype = xdoc.GetElementsByTagName("payloadtype").Item(0).InnerText;
                if (keytype == "string" && payloadtype == "listofstring")
                    flag = true;
                else
                    return false;
            }
            if (flag)
            {
                IEnumerable<string> keys = db.Keys();
                foreach (string k in keys)
                {
                    persist_element_type2<string, DBElement<string, List<string>>, List<string>, string>(db, k, ref xdoc, ref root,fileName);
                }
            }
            xdoc.Save(@fileName);
            WriteLine("\n ***** Database persisted to XML file located at \" {0} \"", fileName);
            return true;
        }


        public bool auugment_metadata<Key,Value>(XmlNode item,ref DBElement<Key,Value> elem)
        {
            
            elem.name = item.ChildNodes[1].ChildNodes[0].InnerText.ToString();
            elem.descr = item.ChildNodes[1].ChildNodes[1].InnerText.ToString();
            elem.timeStamp = Convert.ToDateTime(item.ChildNodes[1].ChildNodes[2].InnerText.ToString());
            if (item.ChildNodes[1].ChildNodes[4].HasChildNodes)
            {
                List<string> elem_categories = new List<string>();
                XmlNodeList categories = item.ChildNodes[1].ChildNodes[4].ChildNodes;
                foreach (XmlNode child in categories)
                    elem_categories.Add(child.InnerText.ToString());
                elem.category = elem_categories;
            }
            return true;
        }
        //to agument database of type 1 : <int,string> into given filename.xml
        public bool augment_db_type1(ref DBEngine<int, DBElement<int, string>> db, string fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            XmlNode root;
            fileName = XMLFactory.correct_path(fileName);
            if (File.Exists(fileName))
            {
                try
                {  xdoc.Load(@fileName); }
                catch(Exception e)
                {   WriteLine("\n Wrong Input file. Error Message : {0}\n",e.Message);  }
            } 
            else
            {
                WriteLine("\n  File \" {0} \" does not exist.",fileName);
                return false;
            }
            root = xdoc.GetElementsByTagName("nosqldb").Item(0);
            if (xdoc.GetElementsByTagName("keytype").Count <= 0 && xdoc.GetElementsByTagName("payloadtype").Count <= 0)
                return false;
            string keytype = xdoc.GetElementsByTagName("keytype").Item(0).InnerText;
            string payloadtype = xdoc.GetElementsByTagName("payloadtype").Item(0).InnerText;
            if (keytype == "int" && payloadtype == "string") { }
            else
                return false;
            XmlNodeList items = xdoc.GetElementsByTagName("item");
            foreach (XmlNode item in items) {
                DBElement<int, string> elem = new DBElement<int, string>();
                int key = Int32.Parse(item.ChildNodes[0].InnerText.ToString());
                auugment_metadata<int, string>(item, ref elem);
                if (item.ChildNodes[1].ChildNodes[3].HasChildNodes)
                {
                    List<int> elem_children = new List<int>();
                    XmlNodeList children = item.ChildNodes[1].ChildNodes[3].ChildNodes;
                    foreach (XmlNode child in children)
                        elem_children.Add(Int32.Parse(child.InnerText.ToString()));
                    elem.children = elem_children;
                }
                elem.payload = item.ChildNodes[1].ChildNodes[5].InnerText.ToString();
                db.insert(key,elem);
                db.insertDictionary(key, elem.category);
            }
            WriteLine("\n ***** Database augmented/restored to database from file located at \" {0} \"", fileName);
            return true;
        }
        //to agument database of type 2 : <string,List<string>> into given filename.xml
        public bool augment_db_type2(ref DBEngine<string, DBElement<string, List<string>>> db, string fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            XmlNode root;
            fileName = XMLFactory.correct_path(fileName);
            if (File.Exists(fileName)) {
                try { xdoc.Load(@fileName); }
                catch (Exception e) {  WriteLine("\n Wrong Input file. Error Message : {0} \n",e.Message);
                }
            }
            else  {
                WriteLine("\n  File \" {0} \" does not exist.",fileName);
                return false;
            }
            root = xdoc.GetElementsByTagName("nosqldb").Item(0);
            if (xdoc.GetElementsByTagName("keytype").Count <= 0 && xdoc.GetElementsByTagName("payloadtype").Count <= 0)
                return false;
            string keytype = xdoc.GetElementsByTagName("keytype").Item(0).InnerText;
            string payloadtype = xdoc.GetElementsByTagName("payloadtype").Item(0).InnerText;
            if (keytype == "string" && payloadtype == "listofstring") { }
            else
                return false;
            XmlNodeList items = xdoc.GetElementsByTagName("item");
            foreach (XmlNode item in items)  {
                DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
                string key = item.ChildNodes[0].InnerText.ToString();
                auugment_metadata<string, List<string>>(item, ref elem);
                if (item.ChildNodes[1].ChildNodes[3].HasChildNodes) {
                    List<string> elem_children = new List<string>();
                    XmlNodeList children = item.ChildNodes[1].ChildNodes[3].ChildNodes;
                    foreach (XmlNode child in children)
                        elem_children.Add(child.InnerText.ToString());
                    elem.children = elem_children;
                }
                if (item.ChildNodes[1].ChildNodes[5].HasChildNodes) {
                    List<string> elem_payload = new List<string>();
                    XmlNodeList pitems = item.ChildNodes[1].ChildNodes[5].ChildNodes;
                    foreach (XmlNode pitem in pitems)
                        elem_payload.Add(pitem.InnerText.ToString());
                    elem.payload = elem_payload;
                }
                else
                    elem.payload = new List<string> { };
                db.insert(key, elem);
                db.insertDictionary(key, elem.category);
            }
            WriteLine("\n ***** Database augmented/restored to database from file located at \" {0} \"", fileName);
            return true;
        }
        //to restore database of type 1 : <int,string> into given filename.xml
        public bool restore_db_type1(ref DBEngine<int, DBElement<int, string>> db, string fileName)
        {
            db.deleteAll();
            augment_db_type1(ref db, fileName);
            return true;
        }
        //to restore database of type 2 : <string,List<string>> into given filename.xml
        public bool restore_db_type2(ref DBEngine<string, DBElement<string, List<string>>> db, string fileName) {
            db.deleteAll();
            augment_db_type2(ref db, fileName);
            return true;
        }
    }
        
    
    class TestPersistEngine
    {
        static void Main(string[] args)
        {
            WriteLine("\n ==inserting 3 elements of type < int , string >==");
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            DBElement<int, string> elem5 = new DBElement<int, string>();
            elem5.payload = "a payload";
            DBElement<int, string> elem6 = new DBElement<int, string>();
            elem6.payload = "a payload";
            DBElement<int, string> elem7 = new DBElement<int, string>();
            elem7.payload = "a payload";
            DBElement<int, string> elem8 = new DBElement<int, string>();
            elem8.payload = "a payload";
            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";
            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.children.AddRange(new List<int> { 1, 5, 23 });
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";
            int key = 0;
            Func<int> keyGen = () => { ++key; return key; };  // anonymous function to generate keys
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            bool p1 = db.insert(1, elem1);
            bool p2 = db.insert(10, elem2);
            bool p3 = db.insert(3, elem3);
            db.insert(4,elem5);
            db.insert(6, elem5);
            db.insert(8, elem6);
            db.insert(12, elem7);
            db.insert(14, elem8);
            db.show<int, DBElement<int, string>, string>();
            
            WriteLine("\n ==inserting 2 elements of type < string , List<string>>==");
            DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
            newerelem1.name = "newerelem1";
            newerelem1.descr = "better formatting";
            newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
            newerelem1.payload.Add("delta");
            newerelem1.payload.Add("epsilon");
            WriteLine();
            DBElement<string, List<string>> newerelem2 = new DBElement<string, List<string>>();
            newerelem2.name = "newerelem2";
            newerelem2.descr = "better formatting";
            newerelem1.children.AddRange(new[] { "first", "second" });
            newerelem2.payload = new List<string> { "a", "b", "c" };
            newerelem2.payload.Add("d");
            newerelem2.payload.Add("e");
            WriteLine();
            DBEngine<string, DBElement<string, List<string>>> newdb =
              new DBEngine<string, DBElement<string, List<string>>>();
            
            newdb.insert("12", newerelem1);
            newdb.insert("10", newerelem2);
            newdb.show<string, DBElement<string, List<string>>, List<string>, string>();
            WriteLine();

            WriteLine("\n== Persisting both database into XML files ==");
            PersistEngine p = new PersistEngine();
            p.persist_db_type1(db, p.getPDBType1FileName());
            p.persist_db_type2(newdb, p.getPDBType2FileName());

            WriteLine("\n== Augmenting both database from mentioned files in function. ==\n");
            p.augment_db_type1(ref db, p.getPDBType1FileName());
            p.augment_db_type2(ref newdb, p.getPDBType2FileName());
            db.show<int, DBElement<int, string>, string>();
            newdb.show<string, DBElement<string, List<string>>, List<string>, string>();

            WriteLine("\n== Restoring both database from mentioned files in function. (Database will get empty in restore function.) ==\n");
            p.restore_db_type1(ref db, p.getPDBType1FileName());
            p.restore_db_type2(ref newdb, p.getPDBType2FileName());
            db.show<int, DBElement<int, string>, string>();
            newdb.show<string, DBElement<string, List<string>>, List<string>, string>();
            
        }
    }
}
