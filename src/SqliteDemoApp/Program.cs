using Database.SQLite;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace SqliteDemoApp
{
    public class Program
    {
        public static void Main()
        {

            //belum bisa run karena di firmware perlu di tambah lib native utk sqlite
            using (var db = new SQLiteDatabase())
            {

                Debug.WriteLine("Executing 1...");
                db.ExecuteNonQuery("CREATE TABLE Test (Var1 TEXT, Var2 INTEGER, Var3 DOUBLE);");

                Debug.WriteLine("Executing 2...");
                db.ExecuteNonQuery("INSERT INTO Test(Var1, Var2, Var3) VALUES ('Hello, World!', 25, 3.14);");

                Debug.WriteLine("Executing 3...");
                db.ExecuteNonQuery("INSERT INTO Test(Var1, Var2, Var3) VALUES('Goodbye, World!', 15, 6.28); ");

                Debug.WriteLine("Executing 4...");
                db.ExecuteNonQuery("INSERT INTO Test (Var1) VALUES('Red'),('Blue'),('Green'),('White');");

                Debug.WriteLine("Executing 5...");
                var result1 = db.ExecuteQuery("SELECT Var1 FROM Test;");

                Debug.WriteLine("Executing 6...");
                var result2 = db.ExecuteQuery("SELECT Var1, Var2, Var3 FROM Test WHERE Var2 > 10;");

                Debug.WriteLine("Executing 7...");
                var result3 = db.ExecuteQuery("SELECT Var1, Var2, Var3 FROM Test WHERE Var2 BETWEEN 24 AND 26");

                Debug.WriteLine("Executing 7...");
                Debug.WriteLine(result2.ColumnCount.ToString() + " " +
                    result2.RowCount.ToString());

                var str = "";

                //foreach (var j in result1.ColumnNames)
                //    str += j + " ";

                //Debug.WriteLine(str);

                foreach (ArrayList i in result1.Data)
                {
                    str = "";

                    foreach (object j in i)
                        str += j.ToString() + " ";

                    Debug.WriteLine(str);

                }

                foreach (ArrayList i in result2.Data)
                {
                    str = "";

                    foreach (object j in i)
                        str += j.ToString() + " ";

                    Debug.WriteLine(str);
                }

                foreach (ArrayList i in result3.Data)
                {
                    str = "";

                    foreach (object j in i)
                        str += j.ToString() + " ";

                    Debug.WriteLine(str);
                }

            }
            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
