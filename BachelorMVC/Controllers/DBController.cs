using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using client = MySql.Data.MySqlClient;
using System.Web.Mvc;
using BachelorMVC.Models;

namespace BachelorMVC.Controllers {

    public class DBController : Controller {
        MySqlConnection conn;
        MySqlDataReader reader;

        public DBController () {
            MySqlConnectionStringBuilder connStr = new MySqlConnectionStringBuilder
            {
                Server = "158.36.13.131",
                UserID = "dokumentpartner",
                Password = "123abc",
                Database = "bachelor",
                SslMode = MySqlSslMode.None

            };

            conn = new MySqlConnection (connStr.GetConnectionString (true));
            conn.Open ();
        }

        //Lager instanser av Signeringsoppdrag som synliggjøres i frontend
        public List<Signeringsoppdrag> OppdragModellerer (String email) {
            List<Signeringsoppdrag> oppdrag = new List<Signeringsoppdrag> ();

            //Hent alle dokumentene tilhørende epost
            List<string> dokumenter = GetDokumenterForBruker (email);

            //Konstruer en liste av signeringsoppdrag
            foreach (string dokument in dokumenter) {
                oppdrag.Add (
                    new Signeringsoppdrag {
                        oppretter = GetOppretter (email),
                        signatører = GetCustomers (email, dokument),
                        dokument = GetDokument (dokument)
                    }
                );
            }

            return oppdrag;

        }

        //Returnerer en liste med stringer som identifiserer en bruker sine opplastede dokumenter
        public List<string> GetDokumenterForBruker (string email) {
            List<string> dokumenter = new List<string> ();
            using (MySqlCommand cmd = new MySqlCommand ()) {
                cmd.Connection = conn;
                cmd.CommandText = "select * from dokument where email = '" + email + "'";
                reader = cmd.ExecuteReader ();

                while (reader.Read ()) {
                    dokumenter.Add (reader.GetString ("DokumentID"));
                }
                return dokumenter;
            }
        }


        public Testbruker GetOppretter (String email)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "select * from oppretter " +
                    "left join kunde using(email) where oppretter.email = '" + email + "'";
                reader = cmd.ExecuteReader();
                Testbruker oppretter = new Testbruker();

                while (reader.Read())
                {
                    oppretter.user_metadata = new Usermetadata();
                    oppretter.user_metadata.firma = reader.GetString("firma");
                    oppretter.email = reader.GetString("email");
                    

                    while (reader.Read())
                    {
                        if (reader.GetString("firma") != null)
                        {
                            oppretter.user_metadata.antallSigneringer = reader.GetInt32("AntallSigneringer");
                            oppretter.user_metadata.nickname = reader.GetString("nickname");
                        }
                        
                    }
                    
                    
                }
                return oppretter;
            }
        }

        //Returnerer et dokumentobjekt. Dokumentdata hentes fra databasen
        public Dokument GetDokument (String dokumentID) {
            using (MySqlCommand cmd = new MySqlCommand ()) {
                cmd.Connection = conn;
                cmd.CommandText =
                    "select * from dokument where DokumentID = '" + dokumentID + "'";
                reader = cmd.ExecuteReader ();

                while (reader.Read ()) {
                    return new Dokument {
                        antallSignaturer = reader.GetInt32 ("AntallSignaturer"),
                            filnavn = reader.GetString ("filnavn"),
                            Id = reader.GetString ("DokumentID")
                    };
                }
            }

            return null;
        }

        //Returnerer en liste av kunder som har signert et spesifikt dokument
        public Testbruker[] GetCustomers (string email, string dokumentID) {
            List<Testbruker> kunder = new List<Testbruker> ();

            using (MySqlCommand cmd = new MySqlCommand ()) {
                cmd.Connection = conn;
                cmd.CommandText =
                    "select " +
                    "o.email," +
                    "d.DokumentID," +
                    "k.email," +
                    "k.firma, " +
                    "k.AntallSigneringer " +
                    "from oppretter o left join " +
                    "(dokument d inner join " +
                    " (" +
                    "kunde_signerer_dokument inner join kunde k on kunde_signerer_dokument.email = k.email" +
                    " )" +
                    " on d.DokumentID = kunde_signerer_dokument.DokumentID) on o.email = d.email " +
                    " where o.email = '" + email + "' and d.DokumentID = '" + dokumentID + "'";
                reader = cmd.ExecuteReader ();

                while (reader.Read ()) {
                    kunder.Add (
                        new Testbruker {
                            email = reader.GetString ("email"),
                                user_id = "email er ID",
                                user_metadata = new Usermetadata {
                                    nickname = "todo: nickname",
                                        firma = reader.GetString ("firma"),
                                        antallSigneringer = reader.GetInt32 ("AntallSigneringer")
                                }
                        }
                    );
                }
                return kunder.ToArray ();
            }
        }

        public void WriteDocument(Guid dokumentId, string filnavn, int antallSignaturer, string navn, string email)
        {
            using(MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "insert into dokument(DokumentID,filnavn,AntallSignaturer,Navn,email) " +
                "values(@DokumentID, @filnavn, @AntallSignaturer, @Navn, @email)";
                cmd.Parameters.AddWithValue("@DokumentID", dokumentId.ToString("B"));
                cmd.Parameters.AddWithValue("@filnavn", filnavn);
                cmd.Parameters.AddWithValue("@AntallSignaturer", antallSignaturer);
                cmd.Parameters.AddWithValue("@Navn", navn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
 