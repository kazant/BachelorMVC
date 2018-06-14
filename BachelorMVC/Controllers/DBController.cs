using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Web.Mvc;
using BachelorMVC.Models;

namespace BachelorMVC.Controllers
{

    public class DBController : Controller
    {
        MySqlConnection conn;
        MySqlDataReader reader;

        public DBController()
        {
            MySqlConnectionStringBuilder connStr = new MySqlConnectionStringBuilder
            {
                Server = "158.36.13.131",
                UserID = "dokumentpartner",
                Password = "123abc",
                Database = "bachelor",
                SslMode = MySqlSslMode.None

            };

            conn = new MySqlConnection(connStr.GetConnectionString(true));
            conn.Open();
        }

        //Lager instanser av Signeringsoppdrag som synliggjøres i frontend
        public List<Signeringsoppdrag> OppdragModellerer(String email)
        {
            List<Signeringsoppdrag> oppdrag = new List<Signeringsoppdrag>();

            //Hent alle dokumentene tilhørende epost
            List<string> dokumenter = HentDokumenterForBruker(email);

            //Konstruer en liste av signeringsoppdrag
            foreach (string dokument in dokumenter)
            {
                oppdrag.Add(
                    new Signeringsoppdrag
                    {
                        Oppretter = HentOppretter(email),
                        Signatører = HentSignatører(email, dokument),
                        Dokument = HentDokument(dokument)
                    }
                );
            }

            return oppdrag;

        }

        //Returnerer en liste med stringer som identifiserer en bruker sine opplastede dokumenter
        public List<string> HentDokumenterForBruker(string email)
        {
            List<string> dokumenter = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from dokument where email = '" + email + "'";
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dokumenter.Add(reader.GetString("DokumentID"));
                }
                return dokumenter;
            }
        }


        public Bruker HentOppretter(String email)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "select * from oppretter " +
                    "left join kunde using(email) where oppretter.email = '" + email + "'";
                reader = cmd.ExecuteReader();
                Bruker oppretter = new Bruker();

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
        public Dokument HentDokument(String dokumentID)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "select * from dokument where DokumentID = '" + dokumentID + "'";
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return new Dokument
                    {
                        AntallSignaturer = reader.GetInt32("AntallSignaturer"),
                        Filnavn = reader.GetString("filnavn"),
                        Id = reader.GetString("DokumentID")
                    };
                }
            }

            return null;
        }

        //Returnerer en liste av kunder som har signert et spesifikt dokument
        public Bruker[] HentSignatører(string email, string dokumentID)
        {
            List<Bruker> kunder = new List<Bruker>();

            using (MySqlCommand cmd = new MySqlCommand())
            {
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
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    kunder.Add(
                        new Bruker
                        {
                            email = reader.GetString("email"),
                            user_id = "email er ID",
                            user_metadata = new Usermetadata
                            {
                                nickname = "todo: nickname",
                                firma = reader.GetString("firma"),
                                antallSigneringer = reader.GetInt32("AntallSigneringer")
                            }
                        }
                    );
                }
                return kunder.ToArray();
            }
        }

        public void DokumentTilDatabase(Guid dokumentId, string filnavn, int antallSignaturer, string navn, string email)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "insert into dokument(DokumentID,filnavn,AntallSignaturer,Navn,email) " +
                "values(@DokumentID, @filnavn, @AntallSignaturer, @Navn, @email)";
                cmd.Parameters.AddWithValue("@DokumentID", dokumentId.ToString("D"));
                cmd.Parameters.AddWithValue("@filnavn", filnavn);
                cmd.Parameters.AddWithValue("@AntallSignaturer", antallSignaturer);
                cmd.Parameters.AddWithValue("@Navn", navn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
            }
        }

        public void OppretterTilDatabase(string email, string fornavn, string etternavn, string firma, string auth0Id)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "insert into oppretter(email,fornavn,etternavn,firma,auth0Id,godkjent" +
                "values(@email, @fornavn, @etternavn, @firma, @auth0Id, @godkjent";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@fornavn", "");
                cmd.Parameters.AddWithValue("@etternavn", "");
                cmd.Parameters.AddWithValue("@firma", "");
                cmd.Parameters.AddWithValue("@auth0Id", auth0Id);
                cmd.Parameters.AddWithValue("@godkjent", "0");
                cmd.ExecuteNonQuery();
            }
        }

        public void SlettOppretter(string auth0Id)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "delete from oppretter where auth0Id = '" + auth0Id + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public void SetGodkjent(string auth0Id)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "update oppretter set godkjent = '1' where auth0Id =  '" + auth0Id + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public void KundeTilDatabase(string[] emails)
        {
            List<string> nyeKunder = new List<string>(emails);

            //spør etter eposter i databasen, hvis den ikke finnes, legg i "todolisten"
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from kunde";
                reader = cmd.ExecuteReader();

                //Finn eposter som allerede finnes i tabell kunde og fjern dem fra nyeKunder
                while (reader.Read())
                {
                    string kunde = reader.GetString("email");
                    for (int i = 0; i < nyeKunder.Count; i++)
                    {

                        if (kunde == nyeKunder[i])
                        {
                            nyeKunder.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            using (MySqlCommand cmd = new MySqlCommand())
            {
                //Skriv de resterende kundene som ikke finnes i kunde-tabellen
                if (nyeKunder.Count != 0)
                {
                    cmd.Connection = conn;
                    foreach (string kunde in nyeKunder)
                    {
                        cmd.CommandText = "insert into kunde(email) values(@email2)";
                        cmd.Parameters.AddWithValue("@email2", kunde);
                        cmd.ExecuteNonQuery();
                    }
                }
            }



        }

        public void SignaturTilDatabase()
        {
            Console.WriteLine("svar fra assently");
            Console.ReadLine();
        }
    }
}
