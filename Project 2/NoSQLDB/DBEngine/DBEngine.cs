///////////////////////////////////////////////////////////////
// DBEngine.cs - define noSQL database                       //
// Ver 1.3                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Source Name: Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 *
 * This class is a starter for the DBEngine package you need to create.
 * It doesn't implement many of the requirements for the db, e.g.,
 * It doesn't remove elements, doesn't persist to XML, doesn't retrieve
 * elements from an XML file, and it doesn't provide hook methods
 * for scheduled persistance.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.4 : 06 Oct 15
 * - Implemented categoriess by using a Dictionary
 * ver 1.3 : 29 Sep 15
 * - added edit and delete functions
 * ver 1.2 : 24 Sep 15
 * - removed extensions methods and tests in test stub
 * - testing is now done in DBEngineTest.cs to avoid circular references
 * ver 1.1 : 15 Sep 15
 * - fixed a casting bug in one of the extension methods
 * ver 1.0 : 08 Sep 15
 * - first release
 *
 */
//todo add placeholders for Shard
//todo add reference to class text XML content

using Project2Starter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
  public class DBEngine<Key, Value> 
  {
    private Dictionary<Key, Value> dbStore;
    private Dictionary<string, List<Key>> dbdictionary;
    //Consutrctor of DBEngine Object 
    public DBEngine()
    {
      dbStore = new Dictionary<Key, Value>();
      dbdictionary = new Dictionary<string, List<Key>>() ;
    }
    //insert function to add data in database
    public bool insert(Key key, Value val)
    {
      if (dbStore.Keys.Contains(key))
        return false;
      dbStore[key] = val;
     // Write("\n New Element inserted with Key {0}\n", key);
      return true;
    }
    //getValue function to get value from database by its key
    public bool getValue(Key key, out Value val)
    {
      if(dbStore.Keys.Contains(key))
      { 
        val = dbStore[key];
        return true;
      }
      val = default(Value);
      return false;
    }
    //Keys function to get list of all keys
    public IEnumerable<Key> Keys()
    {
      return dbStore.Keys;
    }
    //Wrap_type1 function is to wrap the Value type to <Key,string> type
    public DBElement<Key, string> wrap_type1(object value) 
        {
            return (DBElement<Key, string>)value;
        }
    //Wrap_type2 function is to wrap the Value type to <Key,List<string>> type
    public DBElement<Key, List<string>> wrap_type2(object value)
        {
            return (DBElement<Key, List<string>>)value;
        }
    //editMetadata, editDescription, editTimestamp, editChildren and editPayload is used to edit the elemets's metadata and data
    public bool editMetadata<Data>(Key key,string Metadata) 
        {
            if (dbStore.Keys.Contains(key))
            {
                Value value;
                getValue(key,out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                string oldName = elem.name;
                elem.name = Metadata;
               // Write("\n  - Metadata of key {0} edited to {1} from {2}",key,Metadata,oldName);
                return true;
            }
            return false;
        }
    public bool editDescription<Data>(Key key, string Description)
        {
            if (dbStore.Keys.Contains(key))
            {
                Value value;
                getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                string oldDescr = elem.descr;
                elem.descr = Description;
                //Write("\n  - Description of key {0} edited to {1} from {2}", key, Description, oldDescr);
                return true;
            }
            return false;
        }
    public bool editChildren<Data>(Key key, List<Key> children)
        {
            if (dbStore.Keys.Contains(key))
            {
                Value value;
                getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                List<Key> oldChildren = elem.children;
                StringBuilder oldc = new StringBuilder();
                foreach (var v in oldChildren)
                {
                    oldc.Append(v);
                }
                elem.children = children;
                StringBuilder newc = new StringBuilder();
                foreach (var v in children)
                {
                    newc.Append(v);
                }
               // Write("\n  - Children of key {0} edited to \" {1} \" from \" {2} \"", key, newc, oldc);
                return true;
            }
            return false;
        }
    public bool editTimestamp<Data>(Key key)
        {
            if (dbStore.Keys.Contains(key))
            {
                Value value;
                getValue(key, out value);
                DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                DateTime oldT = elem.timeStamp;
                elem.timeStamp = DateTime.Now;
               // Write("\n  - Timestamp of key {0} edited to {1} from {2}", key, elem.timeStamp.ToString(), oldT);
                return true;
            }
            return false;
        }
    public bool editPayload(Key key, List<string> payload)
        {
            if (dbStore.Keys.Contains(key))
            {
                DBElement<Key, List<string>> oldElem = wrap_type2(dbStore[key]);
                List<string> oldpayload = oldElem.payload;
                StringBuilder oldP = new StringBuilder();
                foreach (var v in oldpayload)
                {
                    oldP.Append(v);
                }
                oldElem.payload = payload;
                StringBuilder newp = new StringBuilder();
                foreach (var v in payload)
                {
                    newp.Append(v);
                }
                //Write("\n  - Payload of key {0} edited to \" {1} \" from \" {2} \"", key, newp, oldP);
                return true;
            }
            return false;
        }
    public bool editPayload(Key key,string payload) 
        {
            if (dbStore.Keys.Contains(key))
            {
                DBElement<Key, string> oldElem = wrap_type1(dbStore[key]);
                string oldP = oldElem.payload;
                oldElem.payload = payload;
                //Write("\n  - Payload of key {0} edited to {1} from {2}", key, payload, oldP);
                return true;
            
            }
            return false;
        }
    //addChild function will add newChild to specified key element
    public bool addChild<Data>(Key key,Key newChild)
    {
        if (dbStore.Keys.Contains(key))
        {
            Value value;
            getValue(key, out value);
            DBElement<Key, Data> elem = value as DBElement<Key, Data>;
            elem.children.Add(newChild);
            Write("\n  - {0} is added to Children of key {1}", newChild.ToString(),key);
            return true;
        }
        return false;
    }   
    //removeChild function will remove Child from specified key element
    public bool removeChild<Data>(Key key,Key Child)
    {
        if (dbStore.Keys.Contains(key))
        {
            Value value;
            getValue(key, out value);
            DBElement<Key, Data> elem = value as DBElement<Key, Data>;
            if (elem.children.Contains(Child)) {
                elem.children.Remove(Child);
                Write("\n  - {0} is removed from Children of key {1}", Child.ToString(),key);
            } 
            else
                Write("\n  - {0} is not present in Children List.", Child);
            return true;
        }
        return false;
    }    
    //delete fucntion is used to delete Key/value pair from dtabase
    public bool delete(Key key)
    {
        if (dbStore.Keys.Contains(key))
        {
            dbStore.Remove(key);
            return true;
        }
        return false;
    }
    //deleteAll function will delete all the key value pairs of database
    public void deleteAll()
    {
            dbStore.Clear();
            dbdictionary.Clear();
    }
    //insertDictionary function will store Category/KeyCollection pair 
    public bool insertDictionary(Key key,List<string> listofcategory)
    {
        if (listofcategory.Count == 0)
                listofcategory = new List<string>() { "uncategorized" };
        foreach(string category in listofcategory)
        {
            if (dbdictionary.Keys.Contains(category))
            {
                if (!dbdictionary[category].Contains(key))
                    dbdictionary[category].Add(key);
            }
            else
            {
                    dbdictionary[category] = new List<Key>();
                    dbdictionary[category].Add(key);
             }              
        }
        return true;
    }
    //getDBDict will return Dictionary object which has Category/KeyCollection pairs
    public Dictionary<string, List<Key>> getDBDict()
    {
        return dbdictionary;
    }
    //getCategorizedDB will take current ddatabase as an Input and create a new database containing all the keys which are part of specified category
    public bool getCategorizedDB(string category, DBEngine<Key, Value> db, ref DBEngine<Key, Value> cdb)
    {
        cdb.deleteAll();
        if (!dbdictionary.Keys.Contains(category))
            return false;
        List<Key> keyCollection = dbdictionary[category].ToList();
        foreach (Key key in keyCollection)
        {
            Value value;
            db.getValue(key, out value);
            cdb.insert(key, value);
        }
        return true;
    }
 }

#if(TEST_DBENGINE)

  class TestDBEngine
  {
    static void Main(string[] args)
    {
      "Testing DBEngine Package".title('=');
      WriteLine();

      Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
      Write("\n  This allow use of DBExtensions package without circular dependencies.");

      Write("\n\n");
    }
  }
#endif
}
