///////////////////////////////////////////////////////////////
// QueryEngine.cs - Test DBEngine and DBExtensions           //
// Ver 1.1                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    MSI GE62, Core-i7, Windows 10                //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////
/*
* Following Project requirements are met here :
*   - Requirement 7:  1) performing queries 2-5 on generic types of data
*                     2) performing query 1 on two different types of data
*/

/*
 * Package Operations:
 * -------------------
 * This package fulfills requirement 7 of thi project.
 * Using this package we can perform queries on different types of 
 * database.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBElement.cs, DBEngine.cs,  
 *   DBExtensions.cs, UtilityExtensions.cs
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
using Project2Starter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
    public class QueryEngine<Key,Value,Data,T>
    {
        private DBEngine<Key, Value> db = new DBEngine<Key, Value>();
        //blank constructor
        public QueryEngine()
        {
        }
        //Constructor which will assign argumentade db to class db.
        public QueryEngine(DBEngine<Key, Value> newDb)
        {
            db = newDb;
        }
        // Query 1 function to show the value of specified key 
        // No need to use lambda fnction as the parameter is fixed.
        public bool Query1(Key findKey)
        {
            foreach (Key key in db.Keys())
            {
                if (findKey.ToString() == key.ToString())
                {
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    if (!typeof(Data).ToString().ToLower().Contains("list"))
                    {
                        WriteLine("\n Payload of Key {0} is \" {1} \" ", findKey, elem.payload.ToString());
                        return true;
                    }
                    else
                    {
                        List<T> payloadcollection = elem.payload as List<T>;
                        if (payloadcollection.Count > 0)
                        {
                            StringBuilder accum = new StringBuilder();
                            accum.Append(String.Format("\n Payload of Key \"{0}\" are : ", findKey));
                            bool first = true;
                            foreach (T payload in payloadcollection)
                            {
                                if (first)
                                {
                                    accum.Append(String.Format("{0}", payload));
                                    first = false;
                                }
                                 else
                                    accum.Append(String.Format(", {0}", payload));
                            }

                            WriteLine(" {0}", accum.ToString());
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        // Query1 Overloaded to use it in Remote NoSQL DB
        public bool Query1(Key findKey,ref string result)
        {
            result = "";
            foreach (Key key in db.Keys())
            {
                if (findKey.ToString() == key.ToString())
                {
                    StringBuilder accum = new StringBuilder();
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    if (!typeof(Data).ToString().ToLower().Contains("list"))
                    {
                        accum.Append(String.Format("\n Payload of Key {0} is \" {1} \" ", findKey, elem.payload.ToString()));
                        result = accum.ToString();
                        return true;
                    }
                    else
                    {
                        List<T> payloadcollection = elem.payload as List<T>;
                        if (payloadcollection.Count > 0)
                        {
                           
                            accum.Append(String.Format("\n Payload of Key \"{0}\" are : ", findKey));
                            bool first = true;
                            foreach (T payload in payloadcollection)
                            {
                                if (first)
                                {
                                    accum.Append(String.Format("{0}", payload));
                                    first = false;
                                }
                                else
                                    accum.Append(String.Format(", {0}", payload));
                            }

                            result = accum.ToString();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        // Query 2 function to show the children of specified key 
        // No need to use lambda fnction as the parameter is fixed.
        public bool Query2(Key findKey, out List<Key> keyCollection)
        {
            keyCollection = null;
            foreach (Key key in db.Keys())
            {
                if (findKey.ToString() == key.ToString())
                {
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    keyCollection = elem.children;
                    if (elem.children.Count > 0)
                    {
                        StringBuilder accum = new StringBuilder();
                        accum.Append(String.Format("\n Children of Key {0} are : ", findKey));
                        bool first = true;
                        foreach (Key child in elem.children)
                        {
                            if (first)
                            {
                                accum.Append(String.Format("{0}", child.ToString()));
                                first = false;
                            }
                            else
                                accum.Append(String.Format(", {0}", child.ToString()));
                        }

                        WriteLine(" {0}", accum.ToString());
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Query2(Key findKey, out string result)
        {
            result = "";
            foreach (Key key in db.Keys())
            {
                if (findKey.ToString() == key.ToString())
                {
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    if (elem.children.Count > 0)
                    {
                        StringBuilder accum = new StringBuilder();
                        accum.Append(String.Format("\n Children of Key {0} are : ", findKey));
                        bool first = true;
                        foreach (Key child in elem.children)
                        {
                            if (first)
                            {
                                accum.Append(String.Format("{0}", child.ToString()));
                                first = false;
                            }
                            else
                                accum.Append(String.Format(", {0}", child.ToString()));
                        }
                        result =  accum.ToString();
                        return true;
                    }
                }
            }
            return false;
        }
        // Query 3 function to show set of all keys matching a specified pattern 
        // it returns lambda function which will be passed to processquery function
        public Func<Key, bool> Query3(string str=".*")
        {
            Func<Key, bool> queryPredicate = (Key key) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    try
                    {
                        Regex rgx = new Regex(@str);
                        if (rgx.IsMatch(key.ToString()))
                            return true;
                    }
                    catch(Exception ex)
                    {
                        WriteLine("\n  Invalid Regular Expression. Error Message : {0}\n",ex.Message);
                        return false;
                    }
                }
                return false;
            };
            return queryPredicate;
        }
        // Query 4 function to show All keys that contain a specified string in their metadata section
        // it returns lambda function which will be passed to processquery function
        public Func<Key, bool> Query4(string str)
        {
            Func<Key, bool> queryPredicate = (Key key) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    
                    Value value;
                    db.getValue(key, out value);
                    DBElement<Key, Data> elem = value as DBElement<Key, Data>;
                    if (elem.name.Contains(str) || elem.descr.Contains(str))
                        return true;
                }
                return false;
            };
            return queryPredicate;
        }
        // Query 5 function to show All keys that contain values written within a specified time-date interval
        // it returns lambda function which will be passed to processquery function
        // it takes optional endtime, if not given function will take current time.
        public Func<Key, bool> Query5(DateTime startTime, DateTime? endTime = null)
        {
            Func<Key, bool> queryPredicate = (Key key) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    if (endTime == null)
                        endTime = DateTime.Now;
                    Value value;
                    db.getValue(key,out value);
                    DBElement<Key,Data> elem = value as DBElement<Key, Data>;
                    int cond1 = DateTime.Compare(elem.timeStamp, startTime);
                    int cond2 = DateTime.Compare(elem.timeStamp, (DateTime) endTime);
                    if (cond1 >= 0 && cond2 <= 0)
                    {
                        return true;
                    }
                }
                return false;
            };
            return queryPredicate;
        }
        //----< process query using queryPredicate >-----------------------
        // processquery function accepts lambda return by above function and perform query on each 
        // key of the database.
        public bool processQuery(Func<Key, bool> queryPredicate, out DBFactory<Key, Value> dbf)
        {
            List<Key> keyCollection = new List<Key>();
            foreach(Key key in db.Keys())
            {
                if (queryPredicate(key))
                {
                    keyCollection.Add(key);
                }
            }
            dbf = new DBFactory<Key, Value>(db, keyCollection);
            if (keyCollection.Count() > 0)
                return true;
            return false;
        }
        // showresult will show all key list which passed the query.
        public void showQueryResults(bool result, DBFactory<int, DBElement<int, string>> dbf, string queryParam)
        {
            WriteLine();
            if (result) // query succeeded for at least one key
            {
                Write("Query : \"{0}\" succeeded for following  Key/Value pairs. \n", queryParam);
                if (!typeof(Data).ToString().ToLower().Contains("list"))
                {
                    dbf.showDBF(); 
                }
            }
            else
            {
                Write(" Could not find any keys for \"{0}\"\n", queryParam);
            }
        }
        public void showQueryResults(bool result, DBFactory<string, DBElement<string, List<string>>> dbf, string queryParam)
        {
            WriteLine();
            if (result) // query succeeded for at least one key
            {
                Write("Query : \"{0}\" succeeded for following  Key/Value pairs. \n", queryParam);
                if (typeof(Data).ToString().ToLower().Contains("list"))
                {
                    dbf.showDBF();
                }
            }
            else
            {
                Write(" Could not find any keys for \"{0}\"\n", queryParam);
            }
        }
        // above function ovweloaded to use it in Remote NoSQL Db
        public string QueryResult(bool result, DBFactory<Key, Value> dbf, string queryParam)
        {
            StringBuilder accum = new StringBuilder();
            if (result) // query succeeded for at least one key
            {
                string resultSet = "";
                foreach (Key k in dbf.Keys())
                    resultSet += k.ToString() + ";";
                resultSet.Remove(resultSet.ToString().LastIndexOf(';') -1,1);
                accum.Append(String.Format("Query : \"{0}\" succeeded for Keys : {1}", queryParam, resultSet));
                return accum.ToString();
            }
            accum.Append(String.Format(" Could not find any keys for \"{0}\"\n", queryParam));
            return accum.ToString();
        }
        

    }
    class TestQueryEngine
    {
        
        static void Main(string[] args)
        {

            WriteLine("\n Inserting type1 : <int , string> Elements into database.");
            DBElement<int, string> elem1 = new DBElement<int, string>("metadata1");
            elem1.timeStamp = DateTime.Parse("10/2/2013 12:32:11 AM");
            elem1.children = new List<int> { 2, 3, 4 };
            elem1.payload = "a payload of elem 1";
            DBElement<int, string> elem2 = new DBElement<int, string>("meacba2");
            elem2.timeStamp = DateTime.Parse("10/2/2014 12:32:11 AM");
            elem2.children = new List<int> { 3, 4 };
            elem2.payload = "a payload of elem 2";
            DBElement<int, string> elem3 = new DBElement<int, string>("metaabca3");
            elem3.timeStamp = DateTime.Parse("10/2/2015 12:32:11 AM");
            elem3.children = new List<int> { 5,9};
            elem3.payload = "a payload of elem 3";
            DBElement<int, string> elem4 = new DBElement<int, string>("abc");
            elem4.timeStamp = DateTime.Parse("10/2/2016 12:32:11 AM");
            elem4.children = new List<int> { 10,12 };
            elem4.payload = "a payload of elem 4";
            DBElement<int, string> elem5 = new DBElement<int, string>("mabcdata5");
            elem5.timeStamp = DateTime.Parse("10/2/2017 12:32:11 AM");
            elem5.children = new List<int> { 15,16,18 };
            elem5.payload = "a payload of elem 5";
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            db.insert(1, elem1);
            db.insert(2, elem2);
            db.insert(3, elem3);
            db.insert(4, elem4);
            db.insert(5, elem5);
            //Creating instance of QuerEngine
            QueryEngine<int, DBElement<int, string>, string, string> qeType1 = new QueryEngine<int, DBElement<int, string>, string, string>(db);
            DBFactory<int, DBElement<int, string>> dbfType1 = new DBFactory<int, DBElement<int, string>>();
            //Performing queries on type databsase
            qeType1.Query1(1);

            List<int> resultType1;
            qeType1.Query2(2,out resultType1);

            string regEx = "[2-4]";
            Func<int, bool> query3 = qeType1.Query3(regEx);
            resultType1 = null;
            string queryDesc = "Key with reguler expression " + regEx ;
            qeType1.showQueryResults(qeType1.processQuery(query3, out dbfType1), dbfType1, queryDesc);

            string subString = "abc";
            Func<int, bool> query4 = qeType1.Query4(subString);
            resultType1 = null;
            queryDesc = "Metadata with Substring : " + subString;
            qeType1.showQueryResults(qeType1.processQuery(query4, out dbfType1), dbfType1, queryDesc);

            DateTime startTime = DateTime.Parse("9/2/2014 12:32:11 AM");
            DateTime endTime = DateTime.Parse("11/2/2017 12:32:11 AM");
            Func<int, bool> query5 = qeType1.Query5(startTime, endTime);
            resultType1 = null;
            queryDesc = "Data with timestamp between " + startTime.ToString() + " and " + endTime.ToString(); 
            qeType1.showQueryResults(qeType1.processQuery(query5, out dbfType1), dbfType1, queryDesc);

            WriteLine("\n Inserting type2 : <string , List<string>> Elements into database.");
            DBElement<string, List<string>> elem1_type2 = new DBElement<string, List<string>>("metxyzsd");
            elem1_type2.timeStamp = DateTime.Parse("10/2/2013 12:32:11 AM");
            elem1_type2.children = new List<string> { "c2", "c3", "c4" };
            elem1_type2.payload = new List<string> { "p11", "p12", "p13" };
            DBElement<string, List<string>> elem2_type2 = new DBElement<string, List<string>>("sxyasz");
            elem2_type2.timeStamp = DateTime.Parse("10/2/2014 12:32:11 AM");
            elem2_type2.children = new List<string> { "c10", "c53", "c41" };
            elem2_type2.payload = new List<string> { "p21", "p22", "p23" };
            DBElement<string, List<string>> elem3_type2 = new DBElement<string, List<string>>("xyz");
            elem3_type2.timeStamp = DateTime.Parse("10/2/2015 12:32:11 AM");
            elem3_type2.children = new List<string> { "c122", "c331", "c124" };
            elem3_type2.payload = new List<string> { "p31", "p32", "p33" };
            DBElement<string, List<string>> elem4_type2 = new DBElement<string, List<string>>("abcxyz");
            elem4_type2.timeStamp = DateTime.Parse("10/2/2016 12:32:11 AM");
            elem4_type2.children = new List<string> { "c253", "c312", "c424" };
            elem4_type2.payload = new List<string> { "p41", "p42", "p43" };
            DBEngine<string, DBElement<string, List<string>>> newdb =
              new DBEngine<string, DBElement<string, List<string>>>();
            newdb.insert("12key12", elem1_type2);
            newdb.insert("keykey", elem2_type2);
            newdb.insert("10asf", elem3_type2);
            newdb.insert("10ke2134y", elem4_type2);

            QueryEngine<string, DBElement<string, List<string>>, List<string>, string> qeType2 = new QueryEngine<string, DBElement<string, List<string>>, List<string>, string>(newdb);
            DBFactory<string, DBElement<string, List<string>>> dbfType2 = new DBFactory<string, DBElement<string, List<string>>>();
            qeType2.Query1("keykey");

            List<string> resultType2;
            qeType2.Query2("12key12", out resultType2);

            regEx = ".*ke.*y.*";
            Func<string, bool> queryType2_3 = qeType2.Query3(regEx);
            resultType2 = null;
            queryDesc = "Key with reguler expression " + regEx;
            qeType2.showQueryResults(qeType2.processQuery(queryType2_3, out dbfType2), dbfType2, queryDesc);

            subString = "xyz";
            Func<string, bool> queryType2_4 = qeType2.Query4(subString);
            resultType2 = null;
            queryDesc = "Metadata with Substring : " + subString;
            qeType2.showQueryResults(qeType2.processQuery(queryType2_4, out dbfType2), dbfType2, queryDesc);

            startTime = DateTime.Parse("9/2/2014 12:32:11 AM");
           
            Func<string, bool> queryType2_5 = qeType2.Query5(startTime);
            resultType2 = null;
            queryDesc = "Data with timestamp between " + startTime.ToString() + " and " + DateTime.Now.ToString();
            qeType2.showQueryResults(qeType2.processQuery(queryType2_5, out dbfType2), dbfType2, queryDesc);
            
        }
    }
}
