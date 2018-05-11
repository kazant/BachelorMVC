using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using client = MySql.Data.MySqlClient;
using BachelorMVC.Models;

namespace BachelorMVC.Services
{
    
    public class DBService
    {
        MySqlConnection conn;
        MySqlDataReader reader;
        public DBService(string connStr)
        {
            conn = new MySqlConnection(connStr);
        }

        private MySqlDataReader GetDocuments()
        {
            conn.Open();
            string query = "select * from dokument";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Close();
            return reader = cmd.ExecuteReader();
        }

        private MySqlDataReader GetUsers()
        {
            conn.Open();
            string query = "select * from oppretter";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Close();
            return reader = cmd.ExecuteReader();
        }

        private MySqlDataReader GetCustomers()
        {
            conn.Open();
            string query = "select * from signatør";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Close();
            return reader = cmd.ExecuteReader();
        }

        public List<Dokument> DokumentModellerer()
        {
            List<Dokument> dokumenter = new List<Dokument>();
            MySqlDataReader relDokumenter = GetDocuments();

            while(relDokumenter.Read())
            {
                dokumenter.Add(new Dokument
                {
                    signatører = null,
                    oppretter = null,
                    antallSignaturer = relDokumenter.GetByte("AntallSignaturer"),
                    ferdigSignertLink = "Persistence/Dokumenter/" + relDokumenter.GetString("filnavn"),
                });

            }

            return dokumenter;
        }

        public List<Bruker> BrukerModellerer()
        {
            List<Bruker> brukere = new List<Bruker>();
            MySqlDataReader relBrukere = GetUsers();
            int Id = 0;

            while(relBrukere.Read())
            {
                brukere.Add(new Bruker
                {
                    Id = Id,
                    Fornavn = relBrukere.GetString("fornavn"),
                    Etternavn = relBrukere.GetString("etternavn"),

                });

                Id++;
            }

            return brukere;
        }

        public List<Testbruker> KundeModellerer()
        {
            List<Testbruker> kunder = new List<Testbruker>();
            MySqlDataReader relKunder = GetCustomers();

            
            while(relKunder.Read())
            {
                kunder.Add(new Testbruker
                {
                    email = relKunder.GetString("email"),
                    user_id = relKunder.GetString("id"),
                    user_metadata = new Usermetadata
                    {
                        godkjent = relKunder.GetInt32("godkjent"),
                        nickname = relKunder.GetString("nickname")
                    }

                });
            }

            return kunder;
        }

    }
}
