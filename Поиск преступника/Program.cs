using System;
using System.Collections.Generic;
using System.Linq;

namespace Поиск_преступника
{
    class Program
    {
        static void Main()
        {
            int dossierQuantity = 40;

            DossierFactory criminalsDossierFabrik = new DossierFactory();
            List<Dossier> dossiers = criminalsDossierFabrik.Create(dossierQuantity);

            Searcher searcher = new Searcher();

            searcher.FindDossier(dossiers);
        }
    }

    class Dossier
    {
        public Dossier(string fullName, string nationality, int height, int weight, bool isImprisoned)
        {
            FullName = fullName;
            Nationality = nationality;
            Height = height;
            Weight = weight;
            IsImprisoned = isImprisoned;
        }

        public string FullName { get; }
        public string Nationality { get; }
        public int Height { get; }
        public int Weight { get; }
        public bool IsImprisoned { get; }

        public void WriteInfo()
        {
            const int ShortOffset = -5;
            const int MediumOffset = -10;
            const int LongOffset = -20;

            string imprisonedStatus = IsImprisoned ? "в тюрьме" : "на воле";

            Console.WriteLine($"{FullName,LongOffset}{Nationality,MediumOffset} Рост: {Height,ShortOffset} Вес: {Weight,ShortOffset} Нахождение: {imprisonedStatus}.");
        }
    }

    class DossierFactory
    {
        public List<Dossier> Create(int quantity)
        {
            int[] weightStats = { 70, 90 };
            int[] heightStats = { 165, 180 };

            List<Dossier> dossiers = new List<Dossier>();

            for (int i = 0; i < quantity; i++)
            {
                string nationality = UserUtils.GenerateRandomValue(GetNationalities());

                int height = UserUtils.GenerateStat(heightStats);
                int weight = UserUtils.GenerateStat(weightStats);

                bool isImprisoned = UserUtils.GenerateBool();

                dossiers.Add(new Dossier(GetFullName(), nationality, height, weight, isImprisoned));
            }

            return dossiers;
        }

        private string GetFullName()
        {
            string name = UserUtils.GenerateRandomValue(GetNames());
            string surname = UserUtils.GenerateRandomValue(GetSurnames());

            return $"{name} {surname}";
        }

        private List<string> GetNames() =>
            new List<string>
            {
                "Геннадий",
                "Дмитрий",
                "Максим",
                "Александр",
                "Валерий",
                "Михаил"
            };

        private List<string> GetSurnames() =>
            new List<string>
            {
                "Немичев",
                "Величко",
                "Андреев",
                "Кузнецов",
                "Емельянов",
                "Киррилов",
                "Мамонов"
            };

        private List<string> GetNationalities() =>
            new List<string>
            {
                "Русский",
                "Татарин",
                "Украинец",
                "Чуваш",
                "Башкир"
            };
    }

    class Searcher
    {
        public void FindDossier(List<Dossier> dossiers)
        {
            WriteDossiersInfo(dossiers, "Список досье:");

            string nationality = UserUtils.ReadString("национальность");
            int height = UserUtils.ReadInt("рост");
            int weight = UserUtils.ReadInt("вес");

            List<Dossier> filteredDossiers = dossiers.Where(dossier =>
                dossier.Nationality.ToLower().Contains(nationality.ToLower()) &&
                dossier.Weight == weight &&
                dossier.Height == height &&
                dossier.IsImprisoned == false).ToList();

            WriteSearchResult(filteredDossiers);
        }

        private void WriteSearchResult(List<Dossier> filteredDossiers)
        {
            Console.WriteLine();

            if (filteredDossiers.Count() == 0)
            {
                Console.WriteLine("Досье с такими параметрами не найдено.");

                return;
            }

            WriteDossiersInfo(filteredDossiers, "Найденные досье:");
        }

        private void WriteDossiersInfo(List<Dossier> dossiers, string text)
        {
            Console.WriteLine(text);
            dossiers.ForEach(dossier => dossier.WriteInfo());
            Console.WriteLine();
        }
    }

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static string ReadString(string parameter)
        {
            Console.Write($"Введите {parameter} преступника: ");

            return Console.ReadLine();
        }

        public static int ReadInt(string parameter)
        {
            int number;
            string input;

            do
            {
                input = ReadString(parameter);
            }
            while (int.TryParse(input, out number) == false);

            return number;
        }

        public static T GenerateRandomValue<T>(IEnumerable<T> values)
        {
            int index = s_random.Next(values.Count());

            return values.ElementAt(index);
        }

        public static int GenerateStat(int[] stats)
        {
            int maxLength = 2;

            if (stats.Length != maxLength)
            {
                throw new ArgumentException("Массив stats должен содержать ровно 2 элемента.");
            }

            return s_random.Next(stats[0], stats[1] + 1);
        }

        public static bool GenerateBool()
        {
            int falseChance = 1;

            return s_random.Next(falseChance + 1) == 0;
        }
    }
}
