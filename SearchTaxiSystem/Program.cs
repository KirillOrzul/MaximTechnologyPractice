using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SearchTaxiSystem
{
    public class Points
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Points(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Points other)
        {
            int dX = X - other.X;
            int dY = Y - other.Y;
            double sX = dX * dX;
            double sY = dY * dY;
            double sum = sX + sY;
            return Math.Sqrt(sum);
        }
    }
    // Класс машин
    public class Car
    {
        public string ID { get; set; }
        public Points Position { get; set; }
        public Car(string id, Points position)
        {
            Position = position;
            ID = id;
        }
    }
    // Класс заказчика
    public class Customers
    {
        public Points Location { get; set; }
        public string Name { get; set; }
        public Customers(string customersName, Points location)
        {
            Name = customersName;
            Location = location;
        }
    }
    public class TaxiSystem
    {
        public List<Car> cars;
        private int MapWidth;
        private int MapHeight;
        public TaxiSystem(int width, int height)
        {
            MapWidth = width;
            MapHeight = height;
            cars = new List<Car>();
        }
        // Создание машин
        public void CreatCar(Random rnd)
        {
            for (int i = 1; i <= rnd.Next(5, 15); i++)
            {
                int x = rnd.Next(1, MapWidth);
                int y = rnd.Next(1, MapHeight);
                cars.Add(new Car($"{rnd.Next(1000, 9999)}", new Points(x, y)));
            }

        }
        // Создание заказчика
        public Customers CreatCustomers(string customersName, Random rnd)
        {
            int x = rnd.Next(1, MapWidth);
            int y = rnd.Next(1, MapHeight);
            Points location = new Points(x, y);
            var order = new Customers(customersName, location);
            Console.WriteLine($"\n СОЗДАН ЗАКАЗ: \n'{customersName}' в точке ({x};{y}) на карте");
            return order;
        }
        // Перечисление всех водителей на карте
        public void PrintDriversInfo()
        {
            Console.WriteLine($"\n ВСЕ ВОДИТЕЛИ НА КАРТЕ ({cars.Count}):");
            foreach (var car in cars)
            {
                Console.WriteLine($"· Машина №{car.ID} на ({car.Position.X};{car.Position.Y})");
            }
        }

        // ПЕРВЫЙ АЛГОРИТМ: ПРОСТО СОРТИРОВКА ВСЕХ МАШИН ОТ САМОЙ БЛИЖАЙШЕЙ ДО САМОЙ ДАЛЬНЕЙ
        public List<(Car car, double distance)> SearchFromAllSilent(Customers customer)
        {
            var carDistanceA = new List<(Car car, double distance)>();
            foreach (var car in cars)
            {
                double distance = car.Position.DistanceTo(customer.Location);
                carDistanceA.Add((car, distance));
            }
            carDistanceA.Sort((a, b) => a.distance.CompareTo(b.distance));
            return carDistanceA.Take(5).ToList();
        }
        public void SearchFromAll(Customers customer)
        {
            var result = SearchFromAllSilent(customer); // ← Получаем данные

            Console.WriteLine($"\n БЛИЖАЙШИЕ 5 МАШИН...");
            foreach (var (car, distance) in result) // ← Выводим данные
            {
                Console.WriteLine($"Машина №{car.ID} в {distance * 100:F0} метрах");
            }
        }


        // ВТОРОЙ АЛГОРИТМ: С УВЕЛИЧИВАЮЩИМСЯ РАДИУСОМ ПОИСКА МАШИН ВОКРУГ ЗАКАЗА
        public void SearchByRadius(Customers customer)
        {
            var carDistanceB = new List<(Car car, double distance)>();
            var carsWithDistance = new List<(Car car, double distance)>();
            foreach (var car in cars)
            {
                double distance = car.Position.DistanceTo(customer.Location);
                carsWithDistance.Add((car, distance));
            }
            double radius = 1.0;
            double radiusMax = MapHeight * Math.Sqrt(2);
            while (carDistanceB.Count < 5 && radius <= radiusMax)
            {
                foreach (var (car, distance) in carsWithDistance)
                {

                    if (distance <= radius && !carDistanceB.Any(fc => fc.car == car))
                    {
                        carDistanceB.Add((car, distance));
                        if (carDistanceB.Count >= 5) break;
                    }
                }
                radius += 1.0;
            }
            carDistanceB.Sort((a, b) => a.distance.CompareTo(b.distance));
            var result = carDistanceB.Take(5).ToList();
            Console.WriteLine($"\nБЛИЖАЙШИЕ 5 МАШИН МЕТОДОМ ПОИСКА В РАДИУСЕ:");
            for (int i = 0; i < Math.Min(5, result.Count); i++)
            {
                var (car, distance) = result[i];
                Console.WriteLine($"Машина №{car.ID} в {distance * 100:F0} метрах");
            }
        }


        // ТРЕТИЙ АЛГОРИТМ: ПОИСК МАШЫН НА РАЗБИТОЙ НА КВАДРАТЫ КАРТЕ ОТ БЛИЖАЙШИХ КВАДРАТОВ К ЗАКАЗЧИКУ
        public void SearchByGrid(Customers customer)
        {
            var foundCars = new List<(Car car, double distance)>();
            int cellSize = 5;
            int custCellX = customer.Location.X / cellSize;
            int custCellY = customer.Location.Y / cellSize;
            int maxCellX = (MapWidth - 1) / cellSize;
            int maxCellY = (MapHeight - 1) / cellSize;
            for (int ring = 0; ring <= 10; ring++)
            {
                for (int dx = -ring; dx <= ring; dx++)
                {
                    for (int dy = -ring; dy <= ring; dy++)
                    {
                        if (Math.Abs(dx) != ring && Math.Abs(dy) != ring) continue;
                        int cellX = custCellX + dx, cellY = custCellY + dy;
                        if (cellX < 0 || cellX > maxCellX || cellY < 0 || cellY > maxCellY) continue;
                        var carsInCell = new List<(Car car, double distance)>();
                        foreach (var car in cars)
                        {
                            int carCellX = car.Position.X / cellSize;
                            int carCellY = car.Position.Y / cellSize;

                            if (carCellX == cellX && carCellY == cellY)
                            {
                                double distance = car.Position.DistanceTo(customer.Location);
                                carsInCell.Add((car, distance));
                            }
                        }
                        foreach (var carInCell in carsInCell)
                        {
                            if (!foundCars.Any(fc => fc.car == carInCell.car))
                            {
                                foundCars.Add(carInCell);
                            }
                        }
                    }
                }
            }
            foundCars.Sort((a, b) => a.distance.CompareTo(b.distance));
            var result = foundCars.Take(5).ToList();
            Console.WriteLine($"\nБЛИЖАЙШИЕ 5 МАШИН МЕТОДОМ РАЗДЕЛЕНИЯ НА КВАДРАТЫ:");
            for (int i = 0; i < Math.Min(5, result.Count); i++)
            {
                var (car, distance) = result[i];
                Console.WriteLine($"Машина №{car.ID} в {distance * 100:F0} метрах");
            }
        }
    }
    class SearchTaxiSystem
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            TaxiSystem taxisys = new TaxiSystem(20, 20);
            var name = new List<string>() { "Кирилл", "Алексей", "Иван", "Петр", "Сергей", "Дмитрий", "Андрей", "Михаил", "Анна", "Мария", "Елена", "Ольга", "Наталья", "Артем", "София", "5Виктория", "Станислав", "Алиса", "Роман", "Полина", "Владислав", "Дарья" };
            Customers order = taxisys.CreatCustomers(name[rnd.Next(0, name.Count)], rnd);
            taxisys.CreatCar(rnd);
            taxisys.PrintDriversInfo();
            taxisys.SearchFromAll(order);
            taxisys.SearchByRadius(order);
            taxisys.SearchByGrid(order);
            Console.ReadKey();
        }
    }
}
