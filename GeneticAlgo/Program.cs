using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    // Функція для обчислення евклідової відстані між двома містами
    static double CalculateDistance((double x, double y) city1, (double x, double y) city2)
    {
        return Math.Sqrt(Math.Pow(city1.x - city2.x, 2) + Math.Pow(city1.y - city2.y, 2));
    }

    // Функція для створення випадкового маршруту
    static List<(double x, double y)> CreateRandomRoute(List<(double x, double y)> cities)
    {
        var random = new Random();
        return cities.OrderBy(c => random.Next()).ToList(); // Перемішуємо міста для випадкового маршруту
    }

    // Функція для обчислення загальної довжини маршруту
    static double CalculateRouteDistance(List<(double x, double y)> route)
    {
        double totalDistance = 0;
        for (int i = 0; i < route.Count; i++)
        {
            totalDistance += CalculateDistance(route[i], route[(i + 1) % route.Count]); // Повертаємося до початкового міста
        }
        return totalDistance;
    }

    // Функція для створення початкової популяції маршрутів
    static List<List<(double x, double y)>> CreateInitialPopulation(int popSize, List<(double x, double y)> cities)
    {
        var population = new List<List<(double x, double y)>>();
        for (int i = 0; i < popSize; i++)
        {
            population.Add(CreateRandomRoute(cities)); // Додаємо випадковий маршрут до популяції
        }
        return population;
    }

    // Функція для відбору батьків на основі довжини маршруту
    static List<List<(double x, double y)>> SelectParents(List<List<(double x, double y)>> population)
    {
        return population.OrderBy(route => CalculateRouteDistance(route)).Take(2).ToList(); // Повертаємо два найкращих маршрути
    }

    // Функція для схрещування двох батьків
    static List<(double x, double y)> Crossover(List<(double x, double y)> parent1, List<(double x, double y)> parent2)
    {
        var random = new Random();
        int start = random.Next(0, parent1.Count);
        int end = random.Next(start, parent1.Count);

        var child = new List<(double x, double y)>(new (double x, double y)[parent1.Count]);

        // Копіюємо частину з першого батька
        for (int i = start; i < end; i++)
        {
            child[i] = parent1[i];
        }

        // Заповнюємо решту містами з другого батька в порядку їх проходження
        int pointer = 0;
        foreach (var city in parent2)
        {
            if (!child.Contains(city))
            {
                while (child[pointer] != default)
                {
                    pointer++;
                }
                child[pointer] = city;
            }
        }

        return child;
    }

    // Функція для мутації: міняємо місцями два випадкових міста
    static List<(double x, double y)> Mutate(List<(double x, double y)> route, double mutationRate)
    {
        var random = new Random();
        for (int i = 0; i < route.Count; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                int j = random.Next(0, route.Count);
                // Міняємо місцями два міста
                var temp = route[i];
                route[i] = route[j];
                route[j] = temp;
            }
        }
        return route;
    }

    // Основна функція генетичного алгоритму
    static List<(double x, double y)> GeneticAlgorithm(List<(double x, double y)> cities, int popSize, int generations, double mutationRate, (double x, double y) startCity)
    {
        // Створюємо початкову популяцію
        var population = CreateInitialPopulation(popSize, cities);

        // Основний цикл на кількість поколінь
        for (int generation = 0; generation < generations; generation++)
        {
            // Відбираємо найкращих батьків
            var parents = SelectParents(population);

            // Створюємо нове покоління шляхом кросовера і мутації
            var newPopulation = new List<List<(double x, double y)>>();
            for (int i = 0; i < popSize / 2; i++)
            {
                var child1 = Crossover(parents[0], parents[1]);
                var child2 = Crossover(parents[1], parents[0]);

                // Застосовуємо мутацію
                newPopulation.Add(Mutate(child1, mutationRate));
                newPopulation.Add(Mutate(child2, mutationRate));
            }

            population = newPopulation;
        }

        // Повертаємо найкращий маршрут після всіх поколінь
        var bestRoute = population.OrderBy(route => CalculateRouteDistance(route)).First();

        // Робимо так, щоб стартове місто було першим
        var startCityIndex = bestRoute.IndexOf(startCity);
        if (startCityIndex != -1)
        {
            var rearrangedRoute = bestRoute.Skip(startCityIndex).Concat(bestRoute.Take(startCityIndex)).ToList();
            return rearrangedRoute;
        }

        return bestRoute;
    }

    // Приклад використання
    static void Main()
    {
        // Випадкові координати міст
        var random = new Random();
        var cities = new List<(double x, double y)>();
        for (int i = 0; i < 10; i++)
        {
            cities.Add((random.Next(0, 100), random.Next(0, 100)));
        }
        
        int n = 1;
        foreach (var city in cities)
        {
            Console.WriteLine("City: "+ n + city);

            n++;
        }

        Console.WriteLine("Choose first town:");
        var startCityNumber = int.Parse(Console.ReadLine()) - 1;
        var startCity = cities[startCityNumber];

        

        // Запуск генетичного алгоритму
        var bestRoute = GeneticAlgorithm(cities, popSize: 100, generations: 500, mutationRate: 0.01, startCity);

        // Виведення результату
        Console.WriteLine("Best Route:");
        foreach (var city in bestRoute)
        {

            Console.WriteLine(city);
        }

        Console.WriteLine("Lenght: " + CalculateRouteDistance(bestRoute));
    }
}
