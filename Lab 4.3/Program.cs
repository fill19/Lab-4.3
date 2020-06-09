using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab_4._3
{
    class InvalidZhrepDataException : Exception
    {
        public InvalidZhrepDataException(string message) : base(message) { }
    }

    class Db
    {
        List<Zhrep> Data = new List<Zhrep>();
        string Path;


        public Db(string path)
        {
            Path = path;
            Load();
        }

        public void Add(Zhrep zhrep)
        {
            Data.Add(zhrep);
            Save();
        }

        public void Edit(string name, Zhrep editedZhrep)
        {
            name = name.Trim();
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Name == name)
                {
                    Data[i] = editedZhrep;
                    Save();
                    return;
                }
            }
            Console.WriteLine($"Не можем знайти ЖРЕП з такою назвою: {name}");
        }

        public void Remove(string name)
        {
            name = name.Trim();
            Data = Data.Where(zhrep => zhrep.Name != name).ToList();
            Save();
        }

        public void SeachByDistrict(string district)
        {
            district = district.Trim().ToLower();
            print(Data.Where(zhrep => zhrep.District.Trim().ToLower().Contains(district)).ToList());
        }

        public void SortByName()
        {
            Data = Data.OrderBy(zhrep => zhrep.Name).ToList();
            Save();
        }

        public void Print()
        {
            print(Data);
        }

        private void print(List<Zhrep> Data)
        {
            Console.WriteLine($"{"Назва",10} {"Адреса",10} {"Прізвище начальника",10} {"Кількість підзвітних будинків",10} {"район міста",10}");
            foreach (Zhrep zhrep in Data)
            {
                Console.WriteLine($"{zhrep.Name,10} {zhrep.Address,10} {zhrep.ChiefLastName,10} {zhrep.NoHouses,10} {zhrep.District,10}");
            }
            Console.WriteLine();
        }

        private void Load()
        {
            Data.Clear();

            if (System.IO.File.Exists(Path))
            {
                foreach (string line in System.IO.File.ReadLines(Path))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    Zhrep zhrep = Zhrep.FromString(line);
                    Data.Add(zhrep);
                }
            }
        }

        private void Save()
        {
            List<string> contents = new List<string>(Data.Count);
            foreach (Zhrep zhrep in Data)
            {
                contents.Add(string.Join("|", new string[] {
                    zhrep.Name,
                    zhrep.Address,
                    zhrep.ChiefLastName,
                    zhrep.NoHouses.ToString(),
                    zhrep.District
                }));
            }
            System.IO.File.WriteAllLines(Path, contents);
        }
    }

    class Zhrep
    {
        private string name, address, chiefLastName, district;
        private uint noHouses;

        public Zhrep()
        {

        }

        public Zhrep(string name, string address, string chiefLastName, uint noHouses, string district)
        {
            this.name = name;
            this.address = address;
            this.chiefLastName = chiefLastName;
            this.noHouses = noHouses;
            this.district = district;
        }

        public static Zhrep FromString(string s)
        {
            string[] parsed = s.Trim().Split('|');
            if (parsed.Length != 5)
                throw new InvalidZhrepDataException("Не правильний ввід інформації: не вистачає стовпчиків");

            uint noHouses;
            if (!uint.TryParse(parsed[3], out noHouses))
                throw new InvalidZhrepDataException("Не правильний ввід інформації: не вірна кількість будинків");

            return new Zhrep()
            {
                Name = parsed[0],
                Address = parsed[1],
                ChiefLastName = parsed[2],
                NoHouses = noHouses,
                District = parsed[4],
            };
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string ChiefLastName
        {
            get { return chiefLastName; }
            set { chiefLastName = value; }
        }
        public string District
        {
            get { return district; }
            set { district = value; }
        }
        public uint NoHouses
        {
            get { return noHouses; }
            set { noHouses = value; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Db db = new Db("db.txt");
            bool quit = false;
            while (!quit)
            {
                Console.Write("(q)uit, (p)rint, (a)dd, (e)dit, (r)emove, (f)ind by district, (s)ort by name: ");
                string key = Console.ReadKey().KeyChar.ToString();
                Console.WriteLine();
                switch (key)
                {
                    case "q":
                        quit = true;
                        break;
                    case "p":
                        db.Print();
                        break;
                    case "a":
                        Console.WriteLine("Ввести інформацію про ЖРЕПи у такому форматі: назва|адреса|прізвище начальника|кількість підзвітних будинків|район міста:");
                        Zhrep zhrep = Zhrep.FromString(Console.ReadLine());
                        db.Add(zhrep);
                        break;
                    case "e":
                        Console.Write("Вкажіть назву ЖРЕПу який потрібно відредагувати: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Ввести нові дані ЖРЕПу: назва|адреса|прізвище начальника|кількість підзвітних будинків|район міста:");
                        Zhrep editedZhrep = Zhrep.FromString(Console.ReadLine());
                        db.Edit(name, editedZhrep);
                        break;
                    case "r":
                        Console.Write("Вкажіть назву ЖРЕПу який потрібно видалити: ");
                        db.Remove(Console.ReadLine());
                        break;
                    case "f":
                        Console.Write("Введіть назву району в якому ви хочете шукати ЖРЕП: ");
                        db.SeachByDistrict(Console.ReadLine());
                        break;
                    case "s":
                        db.SortByName();
                        break;
                }

            }
        }
    }
}
