
using System.Data;
using System.Data.SqlClient;

namespace Lab2.Ads2.Models
{
    public class AdsMetoder
    {

        public AdsMetoder() { }

        public List<Annons> SelectAnnonsLista(out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBAds;Integrated Security=True"; // <- gå in på properties på databasen, under connection string

            String selectSQL = "SELECT * FROM [tbl_ads]";

            SqlCommand dbCommand = new SqlCommand(selectSQL, dbConnection);           
           
            SqlDataReader reader = null;

            List<Annons> alist = new List<Annons>();
            errormsg = "";

            
            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Annons a = new Annons();
                    //if (reader["ad_prenr"] != DBNull.Value) {
                    //    a.ad_prenr = 0;
                    //}
                    //else
                    //{
                    //    a.ad_prenr = Convert.ToInt32(reader["ad_prenr"]);
                    //}
                    a.ad_rubrik = reader["ad_rubrik"].ToString();
                    a.ad_innehall = reader["ad_innehall"].ToString();
                    a.ad_varpris = Convert.ToInt32(reader["ad_varpris"]);

                    //a.ad_orgnr = Convert.ToInt32(reader["ad_orgnr"]);
                    //a.ad_prenr = Convert.ToInt32(reader["ad_prenr"]);
                  

                    a.ad_pris = Convert.ToInt32(reader["ad_pris"]);
                    
                                        
                    alist.Add(a);
                }
                reader.Close();
                return alist;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public int InsertPrenAnnons(Annons annons, out string errormsg)
        {
            // Skapa SQL-connection
            SqlConnection dbConnection = new SqlConnection();

            // Koppling mot SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBAds;Integrated Security=True"; // <- gå in på properties på databasen, under connection string

            // SQL-sträng
            String insertSQL = "INSERT INTO [tbl_ads] ([ad_rubrik], [ad_innehall], [ad_prenr], [ad_varpris], [ad_pris]) VALUES (@rubrik, @innehall, @prenr, @varpris, 0)";

            
            // Lägg till en user
            SqlCommand dbCommand = new SqlCommand(insertSQL, dbConnection);               


            dbCommand.Parameters.Add("rubrik", SqlDbType.NVarChar, 50).Value = annons.ad_rubrik;
            dbCommand.Parameters.Add("innehall", SqlDbType.NVarChar, 50).Value = annons.ad_innehall;
            dbCommand.Parameters.Add("prenr", SqlDbType.Int).Value = annons.ad_prenr;
            dbCommand.Parameters.Add("varpris", SqlDbType.Int).Value = annons.ad_varpris;

            try
            {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "Annonsen kunde inte läggas till i databasen."; }
                return (i);
            }
            catch (Exception e)
            {

                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public int InsertAnnonsorAndAnnons(Annons annons, Annonsor annonsor, out string errormsg)
        {
            // Skapa SQL-connection
            SqlConnection dbConnection = new SqlConnection();

            // Koppling mot SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBAds;Integrated Security=True"; // <- gå in på properties på databasen, under connection string

            // SQL-sträng
            //String insertSQL = "INSERT INTO [tbl_ads] ([ad_rubrik], [ad_innehall], [ad_orgnr], [ad_varpris]) VALUES (@rubrik, @innehall, @orgnr, @varpris)";
            String insertSQL = "AddAdsAndAnnonsor @orgnr, @namn, @telnr, @utadress, @postnr, @ort, @futadress, @fpostnr, @fort, @rubrik, @innehall, @varpris";

            // Lägg till en user
            SqlCommand dbCommand = new SqlCommand(insertSQL, dbConnection);


            dbCommand.Parameters.Add("rubrik", SqlDbType.NVarChar, 50).Value = annons.ad_rubrik;
            dbCommand.Parameters.Add("innehall", SqlDbType.NVarChar, 50).Value = annons.ad_innehall;
            dbCommand.Parameters.Add("orgnr", SqlDbType.Int).Value = annonsor.an_orgnr;
            dbCommand.Parameters.Add("varpris", SqlDbType.Int).Value = annons.ad_varpris;
            dbCommand.Parameters.Add("namn", SqlDbType.NVarChar, 50).Value = annonsor.an_namn;
            dbCommand.Parameters.Add("telnr", SqlDbType.NVarChar, 15).Value = annonsor.an_telnr;
            dbCommand.Parameters.Add("utadress", SqlDbType.NVarChar, 50).Value = annonsor.an_utadress;
            dbCommand.Parameters.Add("ort", SqlDbType.NVarChar, 50).Value = annonsor.an_ort;
            dbCommand.Parameters.Add("futadress", SqlDbType.NVarChar, 50).Value = annonsor.an_f_utadress;
            dbCommand.Parameters.Add("fort", SqlDbType.NVarChar, 50).Value = annonsor.an_f_ort;
            dbCommand.Parameters.Add("postnr", SqlDbType.Int).Value = annonsor.an_postnr;
            dbCommand.Parameters.Add("fpostnr", SqlDbType.Int).Value = annonsor.an_f_postnr;

            try
            {
                dbConnection.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 2) { errormsg = ""; }
                else { errormsg = "Annonsen kunde inte läggas till i databasen."; }
                return (i);
            }
            catch (Exception e)
            {

                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }

    }
}
