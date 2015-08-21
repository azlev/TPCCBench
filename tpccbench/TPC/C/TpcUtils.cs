using System;
using System.Threading;
using CommonClasses;
using System.Data.SqlClient;

namespace TPC
{
    public static class TpcUtils
    {
        public static void Heartbeat()
        {
            int i = 1;
            while (true)
            {
                string query = "insert into HEARTBEAT (ID) VALUES(" + i + ")";
                try
                {
                    using (var conn = new SqlConnection(Globals.StrPublisherConn))
                    {
                        conn.Open();
                        using (var comm = new SqlCommand(query, conn))
                        {
                            comm.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    Errhandle.StopProcessing(e, query);
                }
                i++;
                Thread.Sleep(1000);
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns
    }
}