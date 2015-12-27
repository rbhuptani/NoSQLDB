///////////////////////////////////////////////////////////////
// TestExec.cs - Test Requirements for Project #2            //
// Ver 1.2                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    MSI GE62, Core-i7, Windows 10                //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
// Saurce Name: Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////

/*
 * Package Operations:
 * -------------------
 * This package begins the demonstration of meeting requirements.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, Display, 
 *   DBExtensions.cs, UtilityExtensions.cs,PersistEngine.cs
 *   QueryEngine.cs, Scheduler.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.2 : 03 Oct 15
 * - added other requiremnts
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
  class TestExec
  {
    private DBEngine<int, DBElement<int, string>> dbtype1 = new DBEngine<int, DBElement<int, string>>();
    private DBEngine<string, DBElement<string, List<string>>> dbtype2 = new DBEngine<string, DBElement<string, List<string>>>();
    private DBFactory<int, DBElement<int, string>> dbftype1 = new DBFactory<int, DBElement<int, string>>();
    private DBFactory<string, DBElement<string, List<string>>> dbftype2 = new DBFactory<string, DBElement<string, List<string>>>();
        //To test requirement 2
    void TestR2()
    {
      "Demonstrating Requirement #2".title();
      Write("\n ==Inserting < int , string> type of data==");
      DBElement<int, string> elem = new DBElement<int, string>();
      elem.name = "element";
      elem.descr = "test element";
      elem.timeStamp = DateTime.Now;
      elem.children.AddRange(new List<int>{ 2, 3 });
      elem.payload = "elem's payload";
      if(dbtype1.insert(1, elem))
            {
                elem.showElement();
                dbtype1.showDB();
            }
     else
            {
                Write("\nNew Element is not added.\n");
            }
      WriteLine();

      Write("\n ==Inserting < string , Liststring>> type of data==");
      DBElement<string, List<string>> elem1 = new DBElement<string, List<string>>();
      elem1.name = "element";
      elem1.descr = "test element";
      elem1.timeStamp = DateTime.Now;
      elem1.children.AddRange(new List<string> { "c1", "c2", "c3" });
      elem1.payload = new List<string> {"payload 1, payload 2, payload 3" };
      if (dbtype2.insert("stringkey1", elem1))
        {
            elem1.showElement();
            dbtype2.showDB();
        }
      else
        {
            Write("\nNew Element is not added.\n");
        }
     
      WriteLine();
        }
    //To test requirement 3
    void TestR3()
    {
      "Demonstrating Requirement #3".title();
        Write("\n ==Inserting < int , string> type of data==");
        DBElement<int, string> elem = new DBElement<int, string>();
        int key = 10; 
        elem.name = "element 2";
        elem.descr = "test element 2";
        elem.timeStamp = DateTime.Now;
        elem.children.AddRange(new List<int> { 1, 2 });
        elem.payload = "New Payload";
        if (dbtype1.insert(key, elem))
        {
            dbtype1.showDB();
        }
        else
            Write("\n New Element is not added.\n");
        WriteLine();
        Write("\n ==Deleting < int , string> type of data==");
        if (dbtype1.delete(key))
            {
                Write("\n Element with key {0} deleted successfully.",key);
                dbtype1.showDB();
            }   
        else
                Write("\nKey {0} not found.\n",key);
        Write("\n ==Inserting < string , Liststring>> type of data==");
        DBElement<string, List<string>> elem1 = new DBElement<string, List<string>>();
        string skey = "strkey2";
        elem1.name = "element";
        elem1.descr = "test element";
        elem1.timeStamp = DateTime.Now;
        elem1.children.AddRange(new List<string> { "c13", "c12", "c33" });
        elem1.payload = new List<string> { "payload 11, payload 12, payload 31" };
        if (dbtype2.insert(skey, elem1))
            dbtype2.showDB();
        else
            Write("\n New Element is not added.\n");
        WriteLine();
        Write("\n ==Deleting < string , List<string>> type of data==");
        if (dbtype2.delete(skey))
        {
            Write("\n Element with key {0} deleted successfully.", skey);
            dbtype2.showDB();
        }
        else
            Write("\n Key {0} not found.\n", skey);
        }
    //To test requirement 4
    void TestR4()
    {
      "Demonstrating Requirement #4".title();
        int key = 1;
        Write("\n\n == Before Editing Metadata And Before adding/removing any children Type of Database ==");
        dbtype1.showDB();
        WriteLine();
        bool b1 = dbtype1.editMetadata<string>(key, "editedmetadata in type 1");
        bool b2 = dbtype1.editDescription<string>(key, "edited description in type 1");
        bool b3 = dbtype1.editTimestamp<string>(key);
        bool b4 = dbtype1.editChildren<string>(key, new List<int> { 4, 2 });
        bool b5 = dbtype1.editPayload(key, "new payload in type 1");
        if (b1 && b2 && b3 && b4 && b5){ }
        else
            Write("\nKey {0} not found.\n", key);
        WriteLine();
        dbtype1.addChild<string>(1, 10);
        dbtype1.removeChild<string>(1, 2);
        Write("\n\n == After Editing Metadata And Before adding/removing any children == \n");
        dbtype1.showDB();
        WriteLine();

        string skey = "stringkey1";
        Write("\n\n == Before Editing Metadata And Before adding/removing any children Type of Database == \n");
        dbtype2.showDB();
        WriteLine();
        b1 = dbtype2.editMetadata<List<string>>(skey, "editedmetadata in type 2");
        b2 = dbtype2.editDescription<List<string>>(skey, "edited description in type 2");
        b3 = dbtype2.editTimestamp<List<string>>(skey);
        b4 = dbtype2.editChildren<List<string>>(skey, new List<string> { "editedChild 1","e2","e3" });
        b5 = dbtype2.editPayload(skey, new List<string> { "edited payload in type 2, payload list item 2" });
        if (b1 && b2 && b3 && b4 && b5){ }
        else
            Write("\nKey {0} not found.\n", skey);
        WriteLine();
        dbtype2.addChild<List<string>>(skey,"e5");
        dbtype2.removeChild<List<string>>(skey,"e3");
        Write("\n\n == After Editing Metadata And Before adding/removing any children == \n");
        dbtype2.showDB();
        WriteLine();
    }
    //To test requirement 5
    void TestR5()
    {
      "Demonstrating Requirement #5".title();
      WriteLine();
        var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
        elem3.children.AddRange(new List<int> { 1, 5, 23 });
        elem3.payload = "X-Wing fighter in swamp - Oh oh!";
        dbtype1.insert(20, elem3);

        DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
        newerelem1.name = "newerelem1";
        newerelem1.descr = "better formatting";
        newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
        newerelem1.payload.Add("delta");
        newerelem1.payload.Add("epsilon");
        dbtype2.insert("str1232", newerelem1);
        WriteLine();

        WriteLine("\n Persisting both database into XML files");
        PersistEngine p = new PersistEngine();
        p.persist_db_type1(dbtype1, p.getPDBType1FileName());
        p.persist_db_type2(dbtype2, p.getPDBType2FileName());

        WriteLine("\n== Restoring both database from mentioned files in function. (Database will get empty in restore function.) ==\n");
        p.restore_db_type1(ref dbtype1, "RestoreDBType1");
        p.restore_db_type2(ref dbtype2, "RestoreDBType2");
        

        WriteLine("\n== Augmenting both database from mentioned files in function. ==\n");
        p.augment_db_type1(ref dbtype1, p.getPDBType1FileName());
        p.augment_db_type2(ref dbtype2, p.getPDBType2FileName());
        dbtype1.show<int, DBElement<int, string>, string>();
        dbtype2.show<string, DBElement<string, List<string>>, List<string>, string>();
        }
    //To test requirement 6
    void TestR6()
    {
      "Demonstrating Requirement #6".title();
      DBElement<int, string> elem = new DBElement<int, string>();
      elem.name = "element";
      elem.descr = "test element";
      elem.timeStamp = DateTime.Now;
      elem.children.AddRange(new List<int>{ 1, 2, 3 });
      elem.payload = "elem's payload";
      dbtype1.insert(154, elem);
      DBElement<string, List<string>> elem1 = new DBElement<string, List<string>>();
      elem1.name = "element1232";
      elem1.descr = "test element213";
      elem1.timeStamp = DateTime.Now;
      elem1.children.AddRange(new List<string> { "c1", "c2", "c3" });
      elem1.payload = new List<string> {"payload 1, payload 2, payload 3" };
      dbtype2.insert("stringkey1123", elem1);
      Write("\n == Scheduler has turned on for type 1 database : < int, string > == ");
      Scheduler scheduler_type1 = new Scheduler(dbtype1);
      Write("\n == Scheduler has turned on for type 2 database : < string, List<string>> ==");
      Scheduler scheduler_type2 = new Scheduler(dbtype2);
      WriteLine();
    }
    //To test requirement 7
    void TestR7()
    {
        "Demonstrating Requirement #7".title();
        TestR7_Part1();
        TestR7_Part2();
        WriteLine();
            
    }
    //To test requirement 8
    void TestR8()
    {
       "Demonstrating Requirement #8".title();
        QueryEngine<int, DBElement<int, string>, string, string> qeType1 = new QueryEngine<int, DBElement<int, string>, string, string>(dbtype1);
        WriteLine("\n == Performing Query 3 on type1 Database : <int ,string> ==\n");
        string regEx = "[2-4]";
        Func<int, bool> query3 = qeType1.Query3(regEx);
        string queryDesc = "Key with reguler expression " + regEx;
        WriteLine("\n == Showing immutable database of list of keys returned from above query. ==");
        qeType1.showQueryResults(qeType1.processQuery(query3, out dbftype1), dbftype1, queryDesc);
        WriteLine(" == There are no write methods in DBFactory class which shows that the following database will be immutable. To check go to Line no. 65 in DBFactory.cs ==\n");
        WriteLine();QueryEngine<string, DBElement<string, List<string>>, List<string>, string> qeType2 = new QueryEngine<string, DBElement<string, List<string>>, List<string>, string>(dbtype2);
        WriteLine("\n == Performing Query 4 on type2 Database : <string ,List<string>> ==\n");
        string subString = "xyz";
        Func<string, bool> queryType2_4 = qeType2.Query4(subString);
        queryDesc = "Metadata with Substring : " + subString;
        qeType2.showQueryResults(qeType2.processQuery(queryType2_4, out dbftype2), dbftype2, queryDesc);
        WriteLine("\n == Showing immutable database of list of keys returned from above query. ==");
        WriteLine(" == There are no write methods in DBFactory class which shows that the following database will be immutable. To check go to Line no. 65 in DBFactory.cs ==\n");
        qeType2.showQueryResults(qeType2.processQuery(queryType2_4, out dbftype2), dbftype2, queryDesc);
        WriteLine();
    }
    //To test requirement 9
    void TestR9()
    {
      "Demonstrating Requirement #9".title();
        PersistEngine pd = new PersistEngine();
        DBEngine<int, DBElement<int, string>> dbstructure = new DBEngine<int, DBElement<int, string>>();
        pd.restore_db_type1(ref dbstructure, "ProjectStructure");
        WriteLine("\n == This database shows the current solution's project structure.");
        dbstructure.show<int, DBElement<int, string>, string>();
        WriteLine();
    }    
    //To test requirement 12
    void TestR12()
    {
       "Demonstrating Requirement #12".title();
        PersistEngine p = new PersistEngine();
        string input = "Input_R12.xml";
        WriteLine("\n Restoring type1 : <int , string> Elements into database from \" {0} \".", input);
        p.restore_db_type1(ref dbtype1, input);
        dbtype1.showDB();
        WriteLine("\n == Categorized KeyCollecyion ==");
        dbtype1.showDictionary();
        DBEngine<int, DBElement<int, string>> categorizeddb = new DBEngine<int, DBElement<int, string>>();
        dbtype1.getCategorizedDB("category 1", dbtype1, ref categorizeddb);
        Write("\n == Showing database which has category 1 ==");
        categorizeddb.showDB();
        dbtype1.getCategorizedDB("uncategorized", dbtype1, ref categorizeddb);
        Write("\n == Showing uncategorized database ==");
        categorizeddb.showDB();
        WriteLine();
        }
    //To test requirement 7
    void TestR7_Part1()
    {
        PersistEngine p = new PersistEngine();
        string queryInput1 = "QueryInputType1.xml";
        WriteLine("\n Restoring type1 : <int , string> Elements into database from \" {0} \".", queryInput1);
        p.restore_db_type1(ref dbtype1, queryInput1);
        dbtype1.showDB();
        QueryEngine<int, DBElement<int, string>, string, string> qeType1 = new QueryEngine<int, DBElement<int, string>, string, string>(dbtype1);
        WriteLine("\n == Performing Query 1 on type1 Database : <int ,string> ==\n");
        qeType1.Query1(11);
        WriteLine("\n == Performing Query 2 on type1 Database : <int ,string> ==\n");
        List<int> resultType1;
        qeType1.Query2(12, out resultType1);
        WriteLine("\n == Performing Query 3 on type1 Database : <int ,string> ==\n");
        string regEx = "[2-4]";
        Func<int, bool> query3 = qeType1.Query3(regEx);
        string queryDesc = "Key with reguler expression " + regEx;
        qeType1.showQueryResults(qeType1.processQuery(query3, out dbftype1), dbftype1, queryDesc);
        WriteLine("\n == Performing Query 4 on type1 Database : <int ,string> ==\n");
        string subString = "abc";
        Func<int, bool> query4 = qeType1.Query4(subString);
        resultType1 = null;
        queryDesc = "Metadata with Substring : " + subString;
        qeType1.showQueryResults(qeType1.processQuery(query4, out dbftype1), dbftype1, queryDesc);
        WriteLine("\n == Performing Query 5 on type1 Database : <int ,string> ==\n");
        DateTime startTime = DateTime.Parse("9/2/2014 12:32:11 AM");
        DateTime endTime = DateTime.Parse("11/2/2017 12:32:11 AM");
        Func<int, bool> query5 = qeType1.Query5(startTime, endTime);
        resultType1 = null;
        queryDesc = "Data with timestamp between " + startTime.ToString() + " and " + endTime.ToString();
        qeType1.showQueryResults(qeType1.processQuery(query5, out dbftype1), dbftype1, queryDesc);

    }
    //To test requirement 7
    void TestR7_Part2()
    {
            PersistEngine p = new PersistEngine();
            string queryInput2 = "QueryInputType2.xml";
            WriteLine("\n Restoring type2 : <int , string> Elements into database from \" {0} \".", queryInput2);
            p.restore_db_type2(ref dbtype2, queryInput2);
            dbtype2.showDB();
            QueryEngine<string, DBElement<string, List<string>>, List<string>, string> qeType2 = new QueryEngine<string, DBElement<string, List<string>>, List<string>, string>(dbtype2);
            WriteLine("\n == Performing Query 1 on type2 Database : <string ,List<string>> ==\n");
            qeType2.Query1("keykey");
            WriteLine("\n == Performing Query 2 on type2 Database : <string ,List<string>> ==\n");
            List<string> resultType2;
            qeType2.Query2("12key12", out resultType2);
            WriteLine("\n == Performing Query 3 on type2 Database : <string ,List<string>> ==\n");
            string regEx = ".*ke.*y.*";
            Func<string, bool> queryType2_3 = qeType2.Query3(regEx);
            string queryDesc = "Key with reguler expression " + regEx;
            qeType2.showQueryResults(qeType2.processQuery(queryType2_3, out dbftype2), dbftype2, queryDesc);
            WriteLine("\n == Performing Query 4 on type2 Database : <string ,List<string>> ==\n");
            string subString = "xyz";
            Func<string, bool> queryType2_4 = qeType2.Query4(subString);
            resultType2 = null;
            queryDesc = "Metadata with Substring : " + subString;
            qeType2.showQueryResults(qeType2.processQuery(queryType2_4, out dbftype2), dbftype2, queryDesc);
            WriteLine("\n == Performing Query 5 on type2 Database : <string ,List<string>> ==\n");
            DateTime startTime = DateTime.Parse("9/2/2014 12:32:11 AM");
            Func<string, bool> queryType2_5 = qeType2.Query5(startTime);
            resultType2 = null;
            queryDesc = "Data with timestamp between " + startTime.ToString() + " and " + DateTime.Now.ToString();
            qeType2.showQueryResults(qeType2.processQuery(queryType2_5, out dbftype2), dbftype2, queryDesc);
     }
    static void Main(string[] args)
    {
      DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
      
      TestExec exec = new TestExec();
      "Demonstrating  Project#2 Requirements".title('=');
      WriteLine();
      exec.TestR2();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR3();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR4();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR5();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR6();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR7();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR8();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR9();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      exec.TestR12();
      WriteLine("\nPress Enter to Continue...");
      Console.ReadKey();
      
     }
  }
}
