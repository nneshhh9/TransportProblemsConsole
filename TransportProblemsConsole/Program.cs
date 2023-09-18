namespace TransportProblemsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            int[,] table;
            int[] zapasi;
            int[] potrebnosti;
            int minBuy = 0;
            int quantity = 0;
            int minCost = int.MaxValue;
            int minRow = -1;
            int minCol = -1;
            int[] u;
            int[] v;

        start:
            //Выбор ввода
            Console.Clear();
            Console.WriteLine("Выполнять автоматический ввод? <Да>/<Нет>");
            string bot = Console.ReadLine();

            switch (bot.ToLower())
            {
                //Автоматический ввод
                case "да":
                    Console.Clear();
                    Console.WriteLine("Вы выбрали автоматический ввод");

                    zapasi = new int[rnd.Next(2, 10)];
                    potrebnosti = new int[rnd.Next(2, 10)];
                    table = new int[zapasi.Length, potrebnosti.Length];

                    for (int i = 0; i < zapasi.Length; i++)
                        zapasi[i] = rnd.Next(1, 20);

                    for (int i = 0; i < potrebnosti.Length; i++)
                        potrebnosti[i] = rnd.Next(1, 20);

                    for (int i = 0; i < zapasi.Length; i++)
                        for (int j = 0; j < potrebnosti.Length; j++)
                            table[i, j] = rnd.Next(1, 20);

                    Console.WriteLine();
                    break;

                //Ручной ввод
                case "нет":
                    Console.Clear();
                    Console.WriteLine("Вы выбрали ручной ввод");

                tryRead:
                    try
                    {
                        Console.WriteLine("Введите запасы  по каждому поставщику через пробел: ");
                        zapasi = Console.ReadLine().Split().Select(int.Parse).ToArray();

                        Console.WriteLine("Введите потребности по каждому потребителю через пробел: ");
                        potrebnosti = Console.ReadLine().Split().Select(int.Parse).ToArray();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Вы ввели неверные данные, повторите попытку");
                        goto tryRead;
                    }

                    Console.Clear();

                    table = new int[zapasi.Length, potrebnosti.Length];

                    //Ввод данных в таблицу
                    for (int i = 0; i < zapasi.Length; i++)
                    {
                        for (int j = 0; j < potrebnosti.Length; j++)
                        {
                            for (int k = 0; k < zapasi.Length; k++)
                            {
                                for (int o = 0; o < potrebnosti.Length; o++)
                                {
                                    if (k == i && o == j)
                                        Console.Write("x \t");
                                    else Console.Write(table[k, o] + "\t");
                                }
                                Console.WriteLine("| " + zapasi[k]);
                            }

                            for (int k = 0; k < potrebnosti.Length; k++)
                                Console.Write("------ \t");

                            Console.WriteLine();

                            for (int k = 0; k < potrebnosti.Length; k++)
                                Console.Write(potrebnosti[k] + "\t");
                            Console.Write("\nВведите стоимость для x: ");
                            table[i, j] = Convert.ToInt32(Console.ReadLine());
                            Console.Clear();
                        }
                    }
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Неправильный ввод, введите <Да>/<Нет>");
                    goto start;
            }

            //Вывод таблицы
            Console.WriteLine("Таблица стоимости: ");
            for (int i = 0; i < zapasi.Length; i++)
            {
                for (int j = 0; j < potrebnosti.Length; j++)
                {
                    Console.Write(table[i, j] + "\t");
                }
                Console.WriteLine("| " + zapasi[i]);
            }

            for (int i = 0; i < potrebnosti.Length; i++)
                Console.Write("------ \t");

            Console.WriteLine();

            for (int i = 0; i < potrebnosti.Length; i++)
                Console.Write(potrebnosti[i] + "\t");

            if (zapasi.Sum() != potrebnosti.Sum())
            {
                Console.WriteLine("\n\nСумма запасов: " + zapasi.Sum() + "\nСумма потребностей: " + potrebnosti.Sum() +
                    "\nЗадача открытая, так как сумма запасов не равна сумме потребностей.");
            tryexit:
                Console.WriteLine("\n\nХотите вернуться в начало программы? <Да>/<Нет>");
                string exit = Console.ReadLine();
                switch (exit.ToLower())
                {
                    case "да":
                        goto start;
                    case "нет":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Ошибка ввода, повторите попытку.");
                        goto tryexit;
                }
            }
            else
            {
                Console.WriteLine("\n\nСумма запасов: " + zapasi.Sum() + "\nСумма потребностей: " + potrebnosti.Sum() + "\nЗадача закрытая, так как сумма запасов равна сумме потребностей. Нажмите любую клавишу для продолжения.");
                Console.ReadKey();
            }

            FindPotentials();
            int[,] allocation = new int[zapasi.Length, potrebnosti.Length];

            while (true)
            {
                minCost = int.MaxValue;
                minRow = -1;
                minCol = -1;
                for (int i = 0; i < zapasi.Length; i++)
                {
                    for (int j = 0; j < potrebnosti.Length; j++)
                    {
                        if (table[i, j] < minCost && table[i, j] != -1)
                        {
                            minCost = table[i, j];
                            minRow = i;
                            minCol = j;
                        }
                    }
                }

                if (minRow == -1 || minCol == -1)
                    break;

                if (zapasi[minRow] < potrebnosti[minCol])
                {
                    quantity = zapasi[minRow];
                    removeRow();
                }
                else if (zapasi[minRow] > potrebnosti[minCol])
                {
                    quantity = potrebnosti[minCol];
                    removeCol();
                }
                else
                {
                    quantity = potrebnosti[minCol];
                    removeRow();
                    removeCol();
                }

                allocation[minRow, minCol] = quantity;
                minBuy += quantity * minCost;
                zapasi[minRow] -= quantity;
                potrebnosti[minCol] -= quantity;
            }

            Console.WriteLine("\nОпорный план:");
            for (int i = 0; i < zapasi.Length; i++)
            {
                for (int j = 0; j < potrebnosti.Length; j++)
                    Console.Write(allocation[i, j] + "\t");

                Console.WriteLine();
            }
            Console.WriteLine("\nМинимальные затраты: " + minBuy + "\n");

            PrintPotentials();

            Console.ReadKey();

            void removeRow()
            {
                for (int i = 0; i < potrebnosti.Length; i++)
                    table[minRow, i] = -1;
            }

            void removeCol()
            {
                for (int i = 0; i < zapasi.Length; i++)
                    table[i, minCol] = -1;
            }

            void FindPotentials()
            {
                bool updated = true;
                u = new int[zapasi.Length];
                v = new int[potrebnosti.Length];

                // Инициализация потенциалов
                for (int i = 0; i < zapasi.Length; i++)
                    u[i] = -1;

                for (int j = 0; j < potrebnosti.Length; j++)
                    v[j] = -1;

                // Нахождение потенциалов
                u[0] = 0; // Установка первого потенциала в 0

                while (updated)
                {
                    updated = false;

                    for (int i = 0; i < zapasi.Length; i++)
                    {
                        for (int j = 0; j < potrebnosti.Length; j++)
                        {
                            if (u[i] != -1 && v[j] == -1)
                            {
                                v[j] = table[i, j] - u[i];
                                updated = true;
                            }
                            else if (u[i] == -1 && v[j] != -1)
                            {
                                u[i] = table[i, j] - v[j];
                                updated = true;
                            }
                        }
                    }
                }
            }

            void PrintPotentials()
            {
                Console.WriteLine("Потенциалы u[i]:");
                for (int i = 0; i < zapasi.Length; i++)
                {
                    Console.WriteLine("u[{0}] = {1}", i, u[i]);
                }
                Console.WriteLine("Потенциалы v[j]:");
                for (int j = 0; j < potrebnosti.Length; j++)
                {
                    Console.WriteLine("v[{0}] = {1}", j, v[j]);
                }
            }
        }
    }
}
