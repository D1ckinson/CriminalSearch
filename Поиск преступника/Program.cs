using System;
using System.Collections.Generic;
using System.Linq;

namespace Поиск_преступника
{
    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            int dossierQuantity = 40;

            List<CriminalDossier> criminalDossiers = new List<CriminalDossier>();
            CriminalDossierFabrik criminalsDossierFabrik = new CriminalDossierFabrik();

            for (int i = 0; i < dossierQuantity; i++)
                criminalDossiers.Add(criminalsDossierFabrik.CreateDossier());

            ActionBuilder actionBuilder = new ActionBuilder(criminalDossiers);
            Menu menu = new Menu(actionBuilder.GiveMenuActions());

            menu.Work();
        }
    }

    class CriminalDossier
    {
        public CriminalDossier(string fullName, string nationality, int height, int weight, bool isInJail)
        {
            FullName = fullName;
            Nationality = nationality;
            Height = height;
            Weight = weight;
            IsInJail = isInJail;
        }

        public string FullName { get; private set; }
        public string Nationality { get; private set; }
        public int Height { get; private set; }
        public int Weight { get; private set; }
        public bool IsInJail { get; private set; }
    }

    class CriminalDossierFabrik
    {
        private List<string> _names;
        private List<string> _surnames;
        private List<string> _nationalities;

        private int[] _heightStats = { 175, 176 };
        private int[] _weightStats = { 80, 81 };
        private bool[] _states = { true, false };

        private Random _random = new Random();

        public CriminalDossierFabrik()
        {
            FillNames();
            FillSurnames();
            FillNationalities();
        }

        public CriminalDossier CreateDossier()
        {
            string name = _names[_random.Next(0, _names.Count)];
            string surname = _surnames[_random.Next(_surnames.Count)];
            string fullName = $"{name} {surname}";

            string nationality = _nationalities[_random.Next(_nationalities.Count)];

            int height = _random.Next(_heightStats[0], _heightStats[1]);
            int weight = _random.Next(_weightStats[0], _weightStats[1]);

            bool isInJail = _states[_random.Next(_states.Length)];

            return new CriminalDossier(fullName, nationality, height, weight, isInJail);
        }

        private void FillNames() =>
            _names = new List<string>
            {
                "Геннадий",
                "Дмитрий",
                "Максим",
                "Александр",
                "Валерий",
                "Михаил"
            };

        private void FillSurnames() =>
            _surnames = new List<string>
            {
                "Немичев",
                "Величко",
                "Андреев",
                "Кузнецов",
                "Емельянов",
                "Киррилов",
                "Мамонов"
            };

        private void FillNationalities() =>
            _nationalities = new List<string>
            {
                "Русский",
                "Татарин",
                "Украинец",
                "Чуваш",
                "Башкир"
            };
    }

    class ActionBuilder
    {
        private List<CriminalDossier> _criminalDossiers;

        public ActionBuilder(List<CriminalDossier> criminalDossiers) =>
            _criminalDossiers = criminalDossiers;

        public Dictionary<string, Action> GiveMenuActions() =>
            new Dictionary<string, Action>()
            {
                { "Найти досье", FindDossier }
            };

        private void FindDossier()
        {
            string nationality = ReadString("национальность");
            int weight = ReadInt("вес");
            int height = ReadInt("рост");

            IEnumerable<CriminalDossier> filteredDossiers = _criminalDossiers.Where(
                dossier =>
                dossier.Nationality.ToLower().Contains(nationality.ToLower())
                && dossier.Weight == weight
                && dossier.Height == height
                && dossier.IsInJail == false);

            WriteSearchResult(filteredDossiers);
        }

        private void WriteSearchResult(IEnumerable<CriminalDossier> filteredDossiers)
        {
            Console.Clear();

            if (filteredDossiers.Count() == 0)
            {
                WriteMessage("Досье с такими параметрами не найдено.\nНажмите любую кнопку...");
                Console.ReadKey(true);
                Console.Clear();

                return;
            }

            foreach (var dossier in filteredDossiers)
                Console.WriteLine(
                    $"{dossier.FullName}\n" +
                    $"Национальность - {dossier.Nationality}\n" +
                    $"Рост - {dossier.Height}, Вес - {dossier.Weight}.\n");

            Console.WriteLine("\nДля продолжения нажмите любую кнопку.");
            Console.ReadKey(true);
            Console.Clear();
        }

        private void WriteMessage(string text)
        {
            Console.Clear();
            Console.Write(text);
        }

        private string ReadString(string parameter)
        {
            WriteMessage($"Введите {parameter} преступника: ");

            return Console.ReadLine();
        }

        private int ReadInt(string parameter)
        {
            int number;
            string userInput = ReadString(parameter);

            while (int.TryParse(userInput, out number) == false)
                userInput = ReadString(parameter);

            return number;
        }
    }

    class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        private ConsoleColor _backgroundColor = ConsoleColor.White;
        private ConsoleColor _foregroundColor = ConsoleColor.Black;

        private int _itemIndex = 0;
        private bool _isRunning;
        private string[] _items;

        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public Menu(Dictionary<string, Action> actions)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
        }

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                DrawItems();

                ReadKey();
            }
        }

        private void SetItemIndex(int index)
        {
            int lastIndex = _items.Length - 1;

            if (index > lastIndex)
                index = lastIndex;

            if (index < 0)
                index = 0;

            _itemIndex = index;
        }

        private void ReadKey()
        {
            switch (Console.ReadKey(true).Key)
            {
                case MoveSelectionDown:
                    SetItemIndex(_itemIndex + 1);
                    break;

                case MoveSelectionUp:
                    SetItemIndex(_itemIndex - 1);
                    break;

                case ConfirmSelection:
                    _actions[_items[_itemIndex]].Invoke();
                    break;
            }
        }

        private void DrawItems()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < _items.Length; i++)
                if (i == _itemIndex)
                    WriteColoredText(_items[i]);
                else
                    Console.WriteLine(_items[i]);
        }

        private void WriteColoredText(string text)
        {
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }

        private void Exit() =>
            _isRunning = false;
    }
}
