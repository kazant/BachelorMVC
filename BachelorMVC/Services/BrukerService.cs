using BachelorMVC.Models;
using BachelorMVC.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorMVC.Services
{
    public class BrukerService: IbrukerService
    {
        private readonly BachelorDbContext _context;
        public BrukerService(BachelorDbContext contex)
        {
            _context = contex;
        }

        public Bruker addbruker(string fornavn, string etternavn, string id) {

            Bruker nybruker = new Bruker { Fornavn = fornavn, Etternavn = etternavn, unikID = id };
            _context.Bruker.Add(nybruker);//lag bruker
            _context.SaveChanges();// lagre med await = kan kjøre flere ting men må vente på at denne er ferdig før den kan returne.
            return nybruker;

        }
        public Bruker findbruker(string navn, string id) {
            string[] fulltnavn = navn.Split(',');
            string fornavn = fulltnavn[1];
            string etternavn = fulltnavn[0];

            var result = _context.Bruker.FirstOrDefault(x => x.unikID == id);


            if (result != null)
            {
                return result;
            }
            else
            {
                Bruker nybruker = addbruker(fornavn, etternavn, id);
                return nybruker;
            }
        }
    }
    public interface IbrukerService{

        Bruker addbruker(string fornavn, string etternavn, string id);
        Bruker findbruker(string navn, string id);

    }
}


//delete testdata

//var bruke = _context.Bruker.Where(x => x.unikID == id);
//_context.Bruker.RemoveRange(bruke);
//_context.SaveChanges();