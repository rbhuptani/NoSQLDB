///////////////////////////////////////////////////////////////
// DBFactoryTest.cs - Test DBFactory                         //
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
 * This package replaces DBFactory test stub to remove
 * circular package references.
 *
 * Now this testing depends on the class definitions in DBElement,
 * DBEngine, and the extension methods defined in DBExtensions.
 * We no longer need to define extension methods in DBFactory.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBFactoryTest.cs,  DBElement.cs, DBEngine.cs,  
 *   DBExtensions.cs, UtilityExtensions.cs, DBFactory.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 24 Sep 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2Starter
{
    class DBFactoryTest
    {
        static void Main(string[] args)
        {
            "Testing DBFactory Package".title('=');
            Console.WriteLine();
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";
            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";
            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.children.AddRange(new List<int> { 1, 5, 23 });
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            bool p1 = db.insert(1, elem1);
            bool p2 = db.insert(2, elem2);
            bool p3 = db.insert(3, elem3);
            Console.WriteLine("\n Below database is an instance of DBFactory which only have 2 keys of original database.");
           
            DBFactory<int, DBElement<int, string>> dbfactory = new DBFactory<int, DBElement<int, string>>(db, new List<int> { 2, 3 });
            dbfactory.show<int, DBElement<int, string>, string>();
        }
    }
}
