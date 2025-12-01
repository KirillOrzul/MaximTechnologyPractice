using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SearchTaxiSystem;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace BenchmarkTests
{
    public class TaxiSystemBenchmark
    {
        private TaxiSystem taxiSystem;
        private Customers customer;
        private Random rnd;

        [GlobalSetup]
        public void Setup()
        {
            rnd = new Random(42);
            taxiSystem = new TaxiSystem(100, 100);
            for (int i = 0; i < 1000; i++)
            {
                int x = rnd.Next(0, 100);
                int y = rnd.Next(0, 100);
                taxiSystem.cars.Add(new Car(i.ToString(), new Points(x, y)));
            }
            customer = new Customers("TestCustomer", new Points(50, 50));
        }
        [Benchmark]
        public void SearchFromAllBenchmark()
        {
            taxiSystem.SearchFromAll(customer);
        }
        [Benchmark]
        public void SearchByRadiusBenchmark()
        {
            taxiSystem.SearchByRadius(customer);
        }
        [Benchmark]
        public void SearchByGridBenchmark()
        {
            taxiSystem.SearchByGrid(customer);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TaxiSystemBenchmark>();
        }
    }
}