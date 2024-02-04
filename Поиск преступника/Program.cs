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

        private int[] _heightStats = { 175, 180 };
        private int[] _weightStats = { 80, 85 };
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
                { "Найти досье", SortDossiers }
            };

        private void SortDossiers()
        {
            List<CriminalDossier> dossiers = new List<CriminalDossier>();

            dossiers = FindByName(_criminalDossiers);

            dossiers = FindByNationality(dossiers);

            dossiers = FindByHeight(dossiers);

            dossiers = FindByWeight(dossiers);

            WriteResult(dossiers);
        }

        private List<CriminalDossier> FindByNationality(List<CriminalDossier> dossiers)
        {
            string searchDescription = "национальность";

            List<CriminalDossier> dossiersResult = SearchInRealTime(dossiers, searchDescription, IsNationalityInDossier, AddNationalityAction);

            return dossiersResult;
        }

        private List<CriminalDossier> FindByWeight(List<CriminalDossier> dossiers)
        {
            string parameterName = "вес";
            int weight = ReadInt(parameterName);

            List<CriminalDossier> dossiersResult =
                dossiers.FindAll(dossier => dossier.Weight == weight && dossier.IsInJail == false);

            return dossiersResult;
        }

        private List<CriminalDossier> FindByHeight(List<CriminalDossier> dossiers)
        {
            string parameterName = "рост";
            int height = ReadInt(parameterName);

            List<CriminalDossier> dossiersResult =
                dossiers.FindAll(dossier => dossier.Height == height && dossier.IsInJail == false);

            return dossiersResult;
        }

        private int ReadInt(string parameterName)
        {
            string requestMessage = $"Введите {parameterName}: ";
            string failureMessage = "Некорректный ввод. ";
            bool isInputCorrect = false;
            int result;

            WriteMessage(requestMessage);

            do
            {
                if (int.TryParse(Console.ReadLine(), out result))
                    isInputCorrect = true;
                else
                    WriteMessage(failureMessage + requestMessage);
            } while (isInputCorrect == false);

            return result;
        }

        private void WriteMessage(string text)
        {
            Console.Clear();
            Console.Write(text);
        }

        private List<CriminalDossier> FindByName(List<CriminalDossier> dossiers)
        {
            string searchDescription = "имя";

            dossiers = SearchInRealTime(dossiers, searchDescription, IsNameInDossier, AddNameAction);

            return dossiers;
        }

        private List<CriminalDossier> SearchInRealTime(List<CriminalDossier> dossiers, string searchDescription, Func<CriminalDossier, string, bool> matchPredicate, Action<List<string>, CriminalDossier> addingParameterAction)
        {
            const char ConfirmButton = (char)ConsoleKey.Enter;
            const char EraseButton = (char)ConsoleKey.Backspace;

            string inputText = "";
            string space = " ";
            string requestMessage = $"Введите {searchDescription}: ";
            string resultMessage = "Подходящие досье:";

            int currentDossiersCount = 0;
            int startCursorPositionX = 0;
            int startCursorPositionY = 0;
            int requestMessageLength = requestMessage.Length;
            int dossierCursorPositionYIndent = 2;
            int dossiersCursorPositionY = startCursorPositionY + dossierCursorPositionYIndent;

            bool isRunning = true;

            List<CriminalDossier> dossiersResult = new List<CriminalDossier>();
            List<string> result = new List<string>();

            Console.Clear();
            Console.CursorVisible = true;

            while (isRunning)
            {
                Console.SetCursorPosition(startCursorPositionX, startCursorPositionY);
                Console.Write(requestMessage);
                Console.CursorLeft = requestMessageLength + inputText.Length;

                char input = Console.ReadKey(true).KeyChar;

                switch (input)
                {
                    case ConfirmButton:
                        isRunning = false;
                        break;

                    case EraseButton:
                        WriteInputsText(inputText = DeleteLastSymbol(inputText), requestMessageLength);
                        break;

                    default:
                        WriteInputsText(inputText += input, requestMessageLength);
                        break;
                }

                if (inputText == space)
                    DeleteLastSymbol(inputText);

                dossiersResult =
                    dossiers.FindAll(dossier => matchPredicate(dossier, inputText) && dossier.IsInJail == false);

                if (dossiersResult.Count != currentDossiersCount)
                {
                    EraseSearchInfo(resultMessage, currentDossiersCount, dossiersCursorPositionY);
                    currentDossiersCount = dossiersResult.Count;
                }

                result.Clear();
                dossiersResult.ForEach(dossier => addingParameterAction(result, dossier));
                result = result.Distinct().ToList();

                WriteSearchInfo(resultMessage, result, dossiersCursorPositionY);
            }

            Console.CursorVisible = false;
            Console.Clear();

            return dossiersResult;
        }

        private void AddNameAction(List<string> result, CriminalDossier dossier) =>
            result.Add(dossier.FullName);

        private void AddNationalityAction(List<string> result, CriminalDossier dossier) =>
            result.Add(dossier.Nationality);

        private bool IsNationalityInDossier(CriminalDossier dossier, string text) =>
            dossier.Nationality.ToLower().Contains(text.ToLower());

        private bool IsNameInDossier(CriminalDossier dossier, string text) =>
            dossier.FullName.ToLower().Contains(text.ToLower());

        private void WriteInputsText(string text, int cursorPositionY)
        {
            Console.CursorLeft = cursorPositionY;
            Console.Write($"{text} \b");
        }

        private string DeleteLastSymbol(string text)
        {
            if (text.Any())
                text = text.Substring(0, text.Length - 1);

            return text;
        }

        private void EraseSearchInfo(string text, int eraseValue, int CursorPositionY)
        {
            char space = ' ';
            int charQuantity = 40;

            Console.SetCursorPosition(0, CursorPositionY);
            Console.WriteLine(text);

            for (int i = 0; i < eraseValue; i++)
                Console.WriteLine(new string(space, charQuantity));
        }

        private void WriteSearchInfo(string text, List<string> results, int CursorPositionY)
        {
            Console.SetCursorPosition(0, CursorPositionY);
            Console.WriteLine(text);

            results.ForEach(result => Console.WriteLine(result));
        }

        private void WriteResult(List<CriminalDossier> dossiers)
        {
            Console.Clear();

            if (dossiers.Count == 0)
            {
                WriteMessage("Досье с такими параметрами не найдено.\nНажмите любую кнопку...");
                Console.ReadKey(true);
                Console.Clear();

                return;
            }

            foreach (var dossier in dossiers)
                Console.WriteLine(
                    $"{dossier.FullName}\n" +
                    $"Национальность - {dossier.Nationality}\n" +
                    $"Рост - {dossier.Height}, Вес - {dossier.Weight}.\n");

            Console.WriteLine("\nДля продолжения нажмите любую кнопку.");
            Console.ReadKey(true);
            Console.Clear();
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
