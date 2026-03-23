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
        static List<Spell> spells = new List<Spell>();
        static List<Character> characters = new List<Character>();
        static void Main(string[] args)
        {
            try
            {
                SpellekBeolvasasa("HP_spells.csv");
                KarakterekBeolvasasa("HP_characters.csv");

                MentesAdatbazisba();

                Console.WriteLine("Beolvasás sikeres.");
                Console.WriteLine("Spellek száma: " + spells.Count);
                Console.WriteLine("Karakterek száma: " + characters.Count);
                Console.WriteLine();

                Console.WriteLine("Karakterek születési dátum szerint csökkenő sorrendben:");
                var rendezett = characters.OrderByDescending(x => x.Birthdate).ToList();

                foreach (var karakter in rendezett)
                {
                    Console.WriteLine(karakter.FullName + " - " + karakter.Birthdate.ToString("yyyy.MM.dd"));
                }

                Console.WriteLine();

                Character legidosebb = characters.OrderBy(x => x.Birthdate).FirstOrDefault();
                Character legfiatalabb = characters.OrderByDescending(x => x.Birthdate).FirstOrDefault();

                if (legidosebb != null)
                    Console.WriteLine("Legidősebb: " + legidosebb.FullName);

                if (legfiatalabb != null)
                    Console.WriteLine("Legfiatalabb: " + legfiatalabb.FullName);

                Console.WriteLine();
                Console.Write("Adj meg egy becenevet: ");
                string keresettBecenev = Console.ReadLine();

                Character talalat = characters.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Nickname) &&
                    x.Nickname.ToLower() == keresettBecenev.ToLower());

                if (talalat != null)
                {
                    Console.WriteLine("Karakter neve: " + talalat.FullName);

                    Console.Write("Varázslatai: ");
                    if (talalat.KnownSpells.Count > 0)
                        Console.WriteLine(string.Join(", ", talalat.KnownSpells.Select(x => x.Name)));
                    else
                        Console.WriteLine("nincs");

                    Console.Write("Gyermekei: ");
                    if (talalat.Children.Count > 0)
                        Console.WriteLine(string.Join(", ", talalat.Children.Select(x => x.Name)));
                    else
                        Console.WriteLine("nincs");
                }
                else
                {
                    Console.WriteLine("Nincs ilyen becenevű karakter.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt: " + ex.Message);
            }

            Console.ReadKey();
        }
        static void SpellekBeolvasasa(string fajlnev)
        {
            string[] sorok = File.ReadAllLines(fajlnev, Encoding.UTF8);

            for (int i = 1; i < sorok.Length; i++) // 0. sor a fejléc
            {
                List<string> mezok = CsvSorFeldolgozasa(sorok[i]);

                if (mezok.Count < 3)
                    continue;

                string nev = mezok[0].Trim();
                string use = mezok[1].Trim();
                int id;

                if (int.TryParse(mezok[2].Trim(), out id))
                {
                    spells.Add(new Spell(id, nev, use));
                }
            }
        }
        static void KarakterekBeolvasasa(string fajlnev)
        {
            string[] sorok = File.ReadAllLines(fajlnev, Encoding.UTF8);

            for (int i = 1; i < sorok.Length; i++) // 0. sor a fejléc
            {
                List<string> mezok = CsvSorFeldolgozasa(sorok[i]);

                if (mezok.Count < 9)
                    continue;

                Character uj = new Character();

                uj.FullName = mezok[0].Trim();
                uj.Nickname = mezok[1].Trim();
                uj.HogwartsHouse = mezok[2].Trim();
                uj.InterpretedBy = mezok[3].Trim();

                string gyerekekMezo = mezok[4].Trim();
                uj.Image = mezok[5].Trim();

                DateTime datum;
                if (DateTime.TryParse(mezok[6].Trim(), out datum))
                    uj.Birthdate = datum;
                else
                    uj.Birthdate = DateTime.MinValue;

                int id;
                if (int.TryParse(mezok[7].Trim(), out id))
                    uj.Id = id;

                string knownSpellsMezo = mezok[8].Trim();

                if (!string.IsNullOrWhiteSpace(gyerekekMezo))
                {
                    string[] gyerekek = gyerekekMezo.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string gy in gyerekek)
                    {
                        string tisztitott = gy.Trim().Trim('"');
                        if (tisztitott != "")
                            uj.Children.Add(new Child(tisztitott));
                    }
                }

                if (!string.IsNullOrWhiteSpace(knownSpellsMezo))
                {
                    string[] varazslatok = knownSpellsMezo.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string varazs in varazslatok)
                    {
                        string tisztitott = varazs.Trim().Trim('"');

                        Spell talalt = spells.FirstOrDefault(x =>
                            x.Name.ToLower() == tisztitott.ToLower());

                        if (talalt != null)
                            uj.KnownSpells.Add(talalt);
                    }
                }

                characters.Add(uj);
            }
        }
        static List<string> CsvSorFeldolgozasa(string sor)
        {
            List<string> mezok = new List<string>();
            StringBuilder aktualis = new StringBuilder();
            bool idezojelben = false;

            for (int i = 0; i < sor.Length; i++)
            {
                char c = sor[i];

                if (c == '"')
                {
                    idezojelben = !idezojelben;
                }
                else if (c == ',' && !idezojelben)
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
        static void MentesAdatbazisba()
        {
            string connStr = "server=127.0.0.1;port=3307;database=harrypotter;uid=root;pwd=;";

            using (MySql.Data.MySqlClient.MySqlConnection conn =
                   new MySql.Data.MySqlClient.MySqlConnection(connStr))
            {
                conn.Open();

                foreach (var c in characters)
                {
                    string sql = @"INSERT INTO characters
            (`index`, full_name, nickname, hogwarts_house, interpreted_by, image, birthdate)
            VALUES (@index, @full_name, @nickname, @hogwarts_house, @interpreted_by, @image, @birthdate)";

                    using (MySql.Data.MySqlClient.MySqlCommand cmd =
                           new MySql.Data.MySqlClient.MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@index", c.Id);
                        cmd.Parameters.AddWithValue("@full_name", c.FullName);
                        cmd.Parameters.AddWithValue("@nickname", c.Nickname);
                        cmd.Parameters.AddWithValue("@hogwarts_house", c.HogwartsHouse);
                        cmd.Parameters.AddWithValue("@interpreted_by", c.InterpretedBy);
                        cmd.Parameters.AddWithValue("@image", c.Image);

                        if (c.Birthdate == DateTime.MinValue)
                            cmd.Parameters.AddWithValue("@birthdate", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@birthdate", c.Birthdate);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
