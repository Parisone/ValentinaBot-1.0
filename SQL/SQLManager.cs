using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;

namespace ComplexDialogBot.SQL
{
    public static class SQLManager
    {
        public static void Connect()
        {
            Console.Write("Check 1.");
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }
        }

        public static void AddDBAuto(UserProfile user, string id, string prev)
        {
            SqlCommand cmd;
            user.Nprev++;
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            if (user.Nprev == 1)
            {
                cmd = new SqlCommand("INSERT INTO [USERPROFILE] (USER_ID,TARGA,DATA_NASCITA,MAIL,NUM_PREV) VALUES ('" + id + "','" + user.Targa + "','" + user.DataNascita + "','" + user.Email + "'," + user.Nprev + ");", conn);
            }
            else
            {
                cmd = new SqlCommand("UPDATE [USERPROFILE] SET TARGA = '" + user.Targa + "', DATA_NASCITA = " + user.DataNascita + ", NUM_PREV = " + user.Nprev + " WHERE USER_ID LIKE '" + id + "';", conn);
            }

            try
            {
                conn.Open();

                int result = cmd.ExecuteNonQuery();

                if (result < 0)
                {
                    Console.WriteLine("Error inserting data into Database!");
                    conn.Close();
                }
                else
                {
                    conn.Close();
                    AddDBPrev(user.Nprev, id, prev, "Veicolo");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }
        }

        public static void AddDBSalute(UserProfile user, string id, string prev)
        {
            SqlCommand cmd;
            user.Nprev++;
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            if (user.Nprev == 1)
            {
                cmd = new SqlCommand("INSERT INTO [USERPROFILE] (USER_ID,AGE,PROVINCIA,PATOLOGIE,STRUTTURE,MAIL,NUM_PREV) VALUES ('" + id + "','" + user.Age + "','" + user.ProvRes + "','" + user.Patologie + "','" + user.Strutture + "','" + user.Email + "'," + user.Nprev + ");", conn);
            }
            else
            {
                cmd = new SqlCommand("UPDATE [USERPROFILE] SET AGE = '" + user.Age + "', PROVINCIA = '" + user.ProvRes + "', PATOLOGIE = '" + user.Patologie + "', STRUTTURE = '" + user.Strutture + "', NUM_PREV = " + user.Nprev + " WHERE USER_ID LIKE '" + id + "';", conn);
            }

            try
            {
                conn.Open();

                int result = cmd.ExecuteNonQuery();

                if (result < 0)
                {
                    conn.Close();
                    Console.WriteLine("Error inserting data into Database!");
                }
                else
                {
                    conn.Close();
                    AddDBPrev(user.Nprev, id, prev, "Salute");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }
        }

        public static void AddDBFamiglia(UserProfile user, string id, string prev)
        {
            SqlCommand cmd;
            user.Nprev++;
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            if (user.Nprev == 1)
            {
                cmd = new SqlCommand("INSERT INTO [USERPROFILE] (USER_ID,AGE,CAP,COMUNE,ISTAT,RENDITA,PROPAFF,FIGLI,MAIL,NUM_PREV) VALUES ('" + id + "','" + user.Age + "'," + user.Cap + ",'" + user.Comune + "','" + user.Istat + "','" + user.Ren + "'," + user.Propietario + "," + user.Figli + ",'" + user.Email + "'," + user.Nprev + ");", conn);
            }
            else
            {
                cmd = new SqlCommand("UPDATE [USERPROFILE] SET AGE = '" + user.Age + "', CAP = " + user.Cap + ", COMUNE = '" + user.Comune + "', ISTAT = '" + user.Istat + "', RENDITA = '" + user.Ren + "', PROPAFF = " + user.Propietario + ", FIGLI = " + user.Figli + ", NUM_PREV = " + user.Nprev + " WHERE USER_ID LIKE '" + id + "';", conn);
            }

            try
            {
                conn.Open();

                int result = cmd.ExecuteNonQuery();

                if (result < 0)
                {
                    conn.Close();
                    Console.WriteLine("Error inserting data into Database!");
                }
                else
                {
                    conn.Close();
                    AddDBPrev(user.Nprev, id, prev, "Famiglia");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }
        }

        public static void AddDBViaggi(UserProfile user, string id, string prev)
        {
            SqlCommand cmd;
            user.Nprev++;
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            if (user.Nprev == 1)
            {
                cmd = new SqlCommand("INSERT INTO [USERPROFILE] (USER_ID,NUM_PARTECIPANTI,DATA_INIZIO,DATA_FINE,DESTINAZIONE,MAIL,NUM_PREV) VALUES ('" + id + "'," + user.NumPartecipanti + ",'" + user.DataInizio + "','" + user.DataFine + "','" + user.Dest + "','" + user.Email + "'," + user.Nprev + ");", conn);
            }
            else
            {
                cmd = new SqlCommand("UPDATE [USERPROFILE] SET DATA_INIZIO = '" + user.DataInizio + "', DATA_FINE = '" + user.DataFine + "',NUM_PARTECIPANTI = " + user.NumPartecipanti + ", DESTINAZIONE = '" + user.Dest + "', NUM_PREV = " + user.Nprev + " WHERE USER_ID LIKE '" + id + "';", conn);
            }

            try
            {
                conn.Open();

                int result = cmd.ExecuteNonQuery();

                if (result < 0)
                {
                    conn.Close();
                    Console.WriteLine("Error inserting data into Database!");
                }
                else
                {
                    conn.Close();
                    AddDBPrev(user.Nprev, id, prev, "Viaggi");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }
        }

        public static void AddDBCasa(UserProfile user, string id, string prev)
        {
            SqlCommand cmd;
            user.Nprev++;
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            if (user.Nprev == 1)
            {
                cmd = new SqlCommand("INSERT INTO [USERPROFILE] (USER_ID,MQ,CAP,COMUNE,PROVINCIA,ISTAT,FINITURE,FABBRICATO,PROPAFF,ABISAL,MAIL,NUM_PREV) VALUES ('" + id + "'," + user.MQ + "," + user.Cap + ",'" + user.Comune + "','" + user.ProvRes + "','" + user.Istat + "'," + user.Finiture + "," + user.Fabbricato + "," + user.Propietario + "," + user.AS + ",'" + user.Email + "'," + user.Nprev + ");", conn);
            }
            else
            {
                cmd = new SqlCommand("UPDATE [USERPROFILE] SET MQ = " + user.MQ + ", CAP = " + user.Cap + ",COMUNE = '" + user.Comune + "', PROVINCIA = '" + user.ProvRes + "', ISTAT = '" + user.Istat + "', FINITURE = " + user.Finiture + ", FABBRICATO = " + user.Fabbricato + ", PROPAFF = " + user.Propietario + ", ABISAL = " + user.AS + ", NUM_PREV = " + user.Nprev + " WHERE USER_ID LIKE '" + id + "';", conn);
            }

            try
            {
                conn.Open();

                int result = cmd.ExecuteNonQuery();

                if (result < 0)
                {
                    conn.Close();
                    Console.WriteLine("Error inserting data into Database!");
                }
                else
                {
                    conn.Close();
                    AddDBPrev(user.Nprev, id, prev, "Casa");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }
        }

        public static string GetDBPreventivi(string id)
        {
            string pre = string.Empty;
            SqlConnection conn = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            SqlCommand cmd = new SqlCommand("SELECT PREVENTIVO, TIPO FROM PREV WHERE USER_ID LIKE '" + id + "'", conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                if (dr.GetString(0) != "Errore")
                {
                    if (dr.GetString(1) == "Viaggi")
                    {
                        pre += "Viaggi:\n\n";
                        pre += dr.GetString(0);
                    }
                    else if (dr.GetString(1) == "Veicolo")
                    {
                        pre += "Veicolo:\n\n";
                        pre += dr.GetString(0);
                    }
                    else if (dr.GetString(1) == "Casa")
                    {
                        pre += "Casa:\n\n";
                        pre += dr.GetString(0);
                    }
                    else if (dr.GetString(1) == "Salute")
                    {
                        pre += "Salute:\n\n";
                        pre += dr.GetString(0);
                    }
                    else if (dr.GetString(1) == "Famiglia")
                    {
                        pre += "Famiglia:\n\n";
                        pre += dr.GetString(0);
                    }

                    pre += "\n\n";
                }
            }

            dr.Close();
            conn.Close();
            return pre;
        }

        private static void AddDBPrev(int nprev, string id, string prev, string tipo)
        {
            SqlConnection conn2 = new SqlConnection("Data Source=PORT-PARISI\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=ValeBot");
            SqlCommand cmd2 = new SqlCommand("INSERT INTO PREV (PREV_ID,USER_ID,PREVENTIVO,TIPO) VALUES ('" + nprev + "','" + id + "','" + prev + "','" + tipo + "');", conn2);

            try
            {
                conn2.Open();
                int result = cmd2.ExecuteNonQuery();
                if (result < 0)
                {
                    Console.WriteLine("Error inserting data into Database!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured -->> {0}", e);
            }

            conn2.Close();
        }
    }
}
