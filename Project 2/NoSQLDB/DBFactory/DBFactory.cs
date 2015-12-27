///////////////////////////////////////////////////////////////
// DBFactory.cs - To create immutable database               //
// Ver 1.1                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    MSI GE62, Core-i7, Windows 10                //
// Author:      Ronak Bhuptani, SUID#429019830, Syracuse     //
//              University, rmbhupta@syr.edu                 //
///////////////////////////////////////////////////////////////
/*
* Following Project requirements are met here :
*   - Requirement 8:  support the creation of a new immutable 
*                     database constructed from the result of 
*                     any query that returns a collection of keys
*/

/*
 * Package Operations:
 * -------------------
 * This package fulfills requirement 8 of thi project.
 * Using this package we can create immutable databse from
 * results of query.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBElement.cs, DBEngine.cs,  
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;


namespace Project2Starter 
{
    public class DBFactory<Key, Value>
    { 
        private Dictionary<Key, Value> dbStore;
        //Consutrctor of DBFactory Object
        public DBFactory(DBEngine<Key,Value> db,List<Key> keyCollection)
        {
            dbStore = new Dictionary<Key, Value>();
            foreach (Key key in keyCollection)
            {
                Value value;
                db.getValue(key, out value);
                dbStore.Add(key, value);
            }
        }

        public DBFactory()
        {
        }

        //Funciton to return all the keys of dbstore
        public IEnumerable<Key> Keys()
        {
            return dbStore.Keys;
        }
        //function to get value of specified key
        public bool getValue(Key key, out Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                val = dbStore[key];
                return true;
            }
            val = default(Value);
            return false;
        }
    }
    class TestDBFactory
    {
        static void Main(string[] args)
        {
            "Testing DBEngine Package".title('=');
            WriteLine();

            Write("\n  All testing of DBFactory class moved to DBFactoryTest package.");
            Write("\n  This allow use of DBExtensions package without circular dependencies.");

            Write("\n\n");
        }
    }
}
