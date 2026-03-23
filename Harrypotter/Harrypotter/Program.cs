using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Harrypotter
{
    public class Program
    {
        static List<Character> characters = new List<Character>();
        static void Main(string[] args)
        {
            try
            {
                Beolvasas("HP_characters.csv");

                Console.WriteLine("Beolvasás sikeres.");
                Console.WriteLine("Karakterek száma: " + characters.Count);
                Console.WriteLine();

                // Rendezés 
                var rendezett = characters.OrderByDescending(x => x.Birthdate);

                foreach (var c in rendezett)
                {
                    Console.WriteLine(c.FullName + " - " + c.Birthdate.ToString("yyyy.MM.dd"));
                }

                Console.WriteLine();

                // Legidősebb / legfiatalabb
                var legidosebb = characters.OrderBy(x => x.Birthdate).First();
                var legfiatalabb = characters.OrderByDescending(x => x.Birthdate).First();

                Console.WriteLine("Legidősebb: " + legidosebb.FullName);
                Console.WriteLine("Legfiatalabb: " + legfiatalabb.FullName);

                Console.WriteLine();

                // Keresés
                Console.Write("Adj meg egy becenevet: ");
                string keres = Console.ReadLine();

                var talalat = characters.FirstOrDefault(x =>
                    x.Nickname.ToLower() == keres.ToLower());

                if (talalat != null)
                {
                    Console.WriteLine("Név: " + talalat.FullName);

                    Console.WriteLine("Varázslatok:");
                    foreach (var s in talalat.KnownSpells)
                        Console.WriteLine("- " + s);

                    Console.WriteLine("Gyermekek:");
                    foreach (var gy in talalat.Children)
                        Console.WriteLine("- " + gy);
                }
                else
                {
                    Console.WriteLine("Nincs találat.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba: " + ex.Message);
            }

            Console.ReadKey();
        }
        static void Beolvasas(string fajlnev)
        {
            var sorok = File.ReadAllLines(fajlnev, Encoding.UTF8);

            for (int i = 1; i < sorok.Length; i++)
            {
                var mezok = CsvFelbontas(sorok[i]);

                if (mezok.Count < 9)
                    continue;

                Character c = new Character();

                c.FullName = mezok[0].Trim();
                c.Nickname = mezok[1].Trim();
                c.HogwartsHouse = mezok[2].Trim();
                c.InterpretedBy = mezok[3].Trim();

                string childrenMezo = mezok[4].Trim();
                c.Image = mezok[5].Trim();

                DateTime datum;
                if (DateTime.TryParse(mezok[6], out datum))
                    c.Birthdate = datum;
                else
                    c.Birthdate = DateTime.MinValue;

                int id;
                if (int.TryParse(mezok[7], out id))
                    c.Id = id;

                string spellsMezo = mezok[8].Trim();

                // Gyerekek
                if (!string.IsNullOrWhiteSpace(childrenMezo))
                {
                    var gyerekek = childrenMezo.Split(new char[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var gy in gyerekek)
                    {
                        string nev = gy.Trim().Trim('"');
                        if (nev != "")
                            c.Children.Add(nev);
                    }
                }

                // Spellek 
                if (!string.IsNullOrWhiteSpace(spellsMezo))
                {
                    var varazslatok = spellsMezo.Split(new char[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var v in varazslatok)
                    {
                        string nev = v.Trim().Trim('"');
                        if (nev != "")
                            c.KnownSpells.Add(nev);
                    }
                }

                characters.Add(c);
            }
        }
        static List<string> CsvFelbontas(string sor)
        {
            List<string> mezok = new List<string>();
            StringBuilder aktualis = new StringBuilder();
            bool idezo = false;

            foreach (char c in sor)
            {
                if (c == '"')
                {
                    idezo = !idezo;
                }
                else if (c == ',' && !idezo)
                {
                    mezok.Add(aktualis.ToString());
                    aktualis.Clear();
                }
                else
                {
                    aktualis.Append(c);
                }
            }

            mezok.Add(aktualis.ToString());

            return mezok;
        }
    }
}
