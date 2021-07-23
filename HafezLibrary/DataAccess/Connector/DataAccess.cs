using System;
using System.Data;
using System.Data.SqlClient;
using static HafezLibrary.DataAccess.Connector.SqlConnector;

namespace HafezLibrary.DataAccess.Connector
{
    public static class DataAccess
    {
        /*private static readonly SqlConnection Con = new SqlConnection(
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\Data\DB.mdf';Integrated Security=True;Connect Timeout=30");*/

        private static readonly SqlConnection  Con = new SqlConnection(GetConnectionString());
        private static readonly SqlDataAdapter Sda = new SqlDataAdapter();
        private static readonly SqlCommand     Cmd = new SqlCommand();
        private static          DataTable      _dt;

        public static DataTable Select(string query) // method for database select query
        {
            try
            {
                Cmd.Connection    = Con;
                Sda.SelectCommand = Cmd;

                if ( Connect() )
                {
                    _dt             = new DataTable();
                    Cmd.CommandText = query;
                    Sda.Fill(_dt);
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine($"Could not Select. Error #902 \nerror detail : {e.Message}");
                return null;
            }
            finally
            {
                Disconnect();
            }

            return _dt;
        }

        public static void DoCommand(string query) // method for other queries
        {
            //todo change parameter to command

            try
            {
                if ( !Connect() )
                {
                    return;
                }

                Cmd.Connection  = Con;
                Cmd.CommandText = query;
                Cmd.ExecuteNonQuery();
            }
            catch ( Exception e )
            {
                Console.WriteLine($"Could not Do Command. Error #903 \nerror detail : {e.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private static bool Connect() // method for connect to DB
        {
            try
            {
                Con.Open();
                return true;
            }
            catch ( Exception e )
            {
                Console.WriteLine($"Could not Connect. Error #900 \nerror detail : {e.Message}");
                return false;
            }
        }

        private static bool Disconnect() // method for disconnect from DB
        {
            try
            {
                Con.Close();
                return true;
            }
            catch ( Exception e )
            {
                Console.WriteLine($"Could not Disconnect. Error #901 \nerror detail : {e.Message}");
                return false;
            }
        }
    }
}