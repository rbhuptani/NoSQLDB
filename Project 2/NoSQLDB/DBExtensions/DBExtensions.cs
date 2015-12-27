////////////////////////////////////////////////////////////////////
// DBExtensions.cs - define extension methods for Display         //
// Ver 1.3                                                        //
// Application: Demonstration for CSE681-SMA, Project#2           //
// Language:    C#, ver 6.0, Visual Studio 2015                   //
// Platform:    Dell XPS2700, Core-i7, Windows 10                 //
// Source Name: Jim Fawcett, CST 4-187, Syracuse University       //
//              (315) 443-3948, jfawcett@twcny.rr.com             //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse          //
//              University, rmbhupta@syr.edu                      //
////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements extensions methods to support 
 * displaying DBElements and DBEngine instances.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBExtensions.cs, DBEngine.cs, DBElement.cs, UtilityExtensions
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.4 : 06 Oct 15
  - Added showDictionary function which will show Categories/KeyCollection pairs
 * ver 1.3 : 28 Sep 15
 * - Changed the display of Show functions.
 * ver 1.2 : 24 Sep 15
 * - reduced the number of methods and simplified
 * ver 1.1 : 15 Sep 15
 * - added a few comments
 * ver 1.0 : 13 Sep 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Console;

namespace Project2Starter
{
  /////////////////////////////////////////////////////////////////////////
  // Extension methods class 
  // - Extension methods are static methods of a static class
  //   that extend an existing class by adding functionality
  //   not part of the original class.
  // - These methods are all extending the DBElement<Key, Data> class.
  //
  public static class DBElementExtensions
  {
        //----< write metadata to string >-------------------------------------

        public static string showMetaData<Key, Data>(this DBElement<Key, Data> elem)
        {
            StringBuilder accum = new StringBuilder();
            accum.Append(String.Format("\n  {0,-12} : {1}","Name", elem.name));
            accum.Append(String.Format("\n  {0,-12} : {1}", "Description", elem.descr));
            accum.Append(String.Format("\n  {0,-12} : {1}", "Timestamp", elem.timeStamp));
            if (elem.category.Count() > 0)
            {
                accum.Append(String.Format("\n  {0,-12} : ", "Category"));
                bool first = true;
                foreach (string key in elem.category)
                {
                    if (first)
                    {
                        accum.Append(String.Format("{0}", key.ToString()));
                        first = false;
                    }
                    else
                        accum.Append(String.Format(", {0}", key.ToString()));
                }
            }
            if (elem.children.Count() > 0)
            {
                accum.Append(String.Format("\n  {0,-12} : ", "Children"));
                bool first = true;
                foreach (Key key in elem.children)
                {
                    if (first)
                    {
                        accum.Append(String.Format("{0}", key.ToString()));
                        first = false;
                    }
                    else
                        accum.Append(String.Format(", {0}", key.ToString()));
                }
            }
            return accum.ToString();
        }
        //----< write details of element with simple Data to string >----------

        public static string showElement<Key, Data>(this DBElement<Key, Data> elem)
        {
            StringBuilder accum = new StringBuilder();
            accum.Append(elem.showMetaData());
            if (elem.payload != null)
            {
                accum.Append(String.Format("\n  {0,-12} : {1}", "Payload", elem.payload.ToString()));
            }
            return accum.ToString();
        }
        //----< write details of element with enumerable Data to string >------

        public static string showElement<Key, Data, T>(this DBElement<Key, Data> elem)
          where Data : IEnumerable<T>  // constraint clause
        {
            StringBuilder accum = new StringBuilder();
            accum.Append(elem.showMetaData());
            if (elem.payload != null)
            {
                IEnumerable<object> d = elem.payload as IEnumerable<object>;
                if (d == null) //("\n  {0,-12} : {1}", "Timestamp",
                    accum.Append(String.Format("\n  {0,-12} : {1}","Payload", elem.payload.ToString()));
                else
                {
                    bool first = true;
                    accum.Append(String.Format("\n  {0,-12} : ", "Payload"));
                    foreach (var item in elem.payload)  // won't compile without constraint clause
                    {
                        if (first)
                        {
                            accum.Append(String.Format("{0}", item));
                            first = false;
                        }
                        else
                            accum.Append(String.Format(", {0}", item));
                    }
                }
            }
            return accum.ToString();
        }
        
    }
  public static class DBEngineExtensions
  {
        //----< write simple db elements out to Console >------------------
        public static void show<Key, Value, Data>(this DBEngine<Key, Value> db)
        {
            Write("\n\n == Current Database, Type : < int, string > ==");
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  {0,-12} : {1}", "Key", key);
                Write(elem.showElement());
             }
        }
        public static string DBtoXML<Key, Value, Data,T>(this DBEngine<Key, Value> db)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlNode rootNode = doc.CreateElement("database");
            doc.AppendChild(rootNode);

            foreach (Key key in db.Keys())
            {
                XmlNode element = doc.CreateElement("element");
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                XmlNode xkey = doc.CreateElement("key");
                xkey.InnerText = key.ToString();
                XmlNode xname = doc.CreateElement("name");
                xname.InnerText = elem.name.ToString();
                XmlNode xdesc = doc.CreateElement("desc");
                xdesc.InnerText = elem.descr.ToString();
                XmlNode xtimestamp = doc.CreateElement("timestamp");
                xtimestamp.InnerText = elem.timeStamp.ToString();
                string s = "";
                if (elem.children.Count > 0)
                {
                    bool first = true;
                    foreach (Key c in elem.children)
                    {
                        if (first)
                        {
                            s += c.ToString();
                            first = false;
                        }
                        else
                            s += ", " + c.ToString();
                    }
                }
                XmlNode xchildren = doc.CreateElement("children");
                xchildren.InnerText = s;
                s = "";
                XmlNode xpayload = doc.CreateElement("payload");
                if (!typeof(Data).ToString().ToLower().Contains("list"))
                    xpayload.InnerText = elem.payload.ToString();
                else{
                    bool first = true;
                    List<T> payloadcollection = elem.payload as List<T>;
                    foreach (T item in payloadcollection)  // won't compile without constraint clause
                    {
                        if (first)
                        {
                            s += item.ToString();
                            first = false;
                        }
                        else
                            s += ", " + item.ToString();
                    }
                    xpayload.InnerText = s;
                }
                element.AppendChild(xkey);
                element.AppendChild(xname);
                element.AppendChild(xdesc);
                element.AppendChild(xtimestamp);
                element.AppendChild(xchildren);
                element.AppendChild(xpayload);
                rootNode.AppendChild(element);
            }
            return doc.OuterXml;
        }
        //----< write enumerable db elements out to Console >--------------
        public static void show<Key, Value, Data, T>(this DBEngine<Key, Value> db)   where Data : IEnumerable<T>
  {
            Write("\n\n == Current Database, Type : < string, List<string> > ==");
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  {0,-12} : {1}", "Key", key);
                Write(elem.showElement<Key, Data, T>());
            }
  }
        // this functions shows categorized list collection.
  public static void showDictionary<Key, Value>(this DBEngine<Key, Value> db)
  {
            WriteLine("\n {0,-15}  : {1}"," Category","KeyCollection");
            Dictionary<string, List<Key>> dict = db.getDBDict();
            foreach (string s in dict.Keys)
            {
                StringBuilder accum = new StringBuilder();
                accum.Append(String.Format(" {0,-15} : ", s));
                if (dict[s].ToList().Count() > 0)
                {

                    bool first = true;
                    foreach (Key i in dict[s].ToList())
                    {
                        if (first)
                        {
                            accum.Append(String.Format("{0}", i.ToString()));
                            first = false;
                        }
                        else
                            accum.Append(String.Format(", {0}", i.ToString()));
                    }
                }
                Console.WriteLine(" {0}", accum);
            }
   }
}

    public static class DBFactoryExtensions
    {
        //----< write simple db elements out to Console >------------------
        public static void show<Key, Value, Data>(this DBFactory<Key, Value> db)
        {   
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  -- key = {0} --", key);
                Write(elem.showElement());
            }
        }
        //----< write enumerable db elements out to Console >--------------
        public static void show<Key, Value, Data, T>(this DBFactory<Key, Value> db)
          where Data : IEnumerable<T>
        {
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                Write("\n\n  -- key = {0} --", key);
                Write(elem.showElement<Key, Data, T>());
            }
        }

        public static string dbf_to_string<Key, Value, Data, T>(this DBFactory<Key, Value> db)
          where Data : IEnumerable<T>
        {
            StringBuilder accum = new StringBuilder();
            foreach (Key key in db.Keys())
            {
                Value value;
                db.getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                accum.Append(String.Format("\n\n  -- key = {0} --", key));
                accum.Append(String.Format(elem.showElement<Key, Data, T>()));
            }
            return accum.ToString();
        }
    }

#if (TEST_DBEXTENSIONS)

    class TestDBExtensions
  {
    static void Main(string[] args)
    {
      "Testing DBExtensions Package".title('=');
      WriteLine();

      Write("\n --- Test DBElement<int,string> ---");
      DBElement<int, string> elem1 = new DBElement<int, string>();
      elem1.payload = "a payload";
      Write(elem1.showElement<int, string>());

      DBEngine<int, DBElement<int, string>> dbs = new DBEngine<int, DBElement<int, string>>();
      dbs.insert(1, elem1);
      dbs.show<int, DBElement<int,string>, string>();
      WriteLine();

      Write("\n --- Test DBElement<string,List<string>> ---");
      DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
      newelem1.name = "newelem1";
      newelem1.descr = "test new type";
      newelem1.children = new List<string> { "Key1", "Key2" };
      newelem1.payload = new List<string> { "one", "two", "three" };
      Write(newelem1.showElement<string, List<string>, string>());

      DBEngine<string, DBElement<string, List<string>>> dbe = new DBEngine<string, DBElement<string, List<string>>>();
      dbe.insert("key1", newelem1);
      dbe.show<string, DBElement<string, List<string>>, List<string>, string>();

      Write("\n\n");
    }
  }
#endif
}
