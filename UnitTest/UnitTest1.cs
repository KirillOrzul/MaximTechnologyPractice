using NUnit.Framework;
using SearchTaxiSystem;
using System;
using System.Collections.Generic;

namespace SearchTaxiSystem.Tests
{
    [TestFixture]
    public class CriticalTests
    {
        [Test]
        public void Test1_BasicSearch_ShouldFindFiveClosestCars()
        {
            var taxi = new TaxiSystem(100, 100);
            taxi.cars.Add(new Car("1001", new Points(10, 10)));
            taxi.cars.Add(new Car("1002", new Points(20, 20)));
            taxi.cars.Add(new Car("1003", new Points(30, 30)));
            taxi.cars.Add(new Car("1004", new Points(40, 40)));
            taxi.cars.Add(new Car("1005", new Points(50, 50)));
            var customer = new Customers("Иван", new Points(15, 15));
            var result = taxi.SearchFromAllSilent(customer);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result[0].car.ID, Is.EqualTo("1001"));
        }
        [Test]
        public void Test2_DistanceCalculation_ShouldBeCorrect()
        {
            var pointA = new Points(0, 0);
            var pointB = new Points(3, 4);
            var distance = pointA.DistanceTo(pointB);
            Assert.That(distance, Is.EqualTo(5).Within(0.001));
        }
        [Test]
        public void Test3_Sorting_ShouldBeFromClosestToFarthest()
        {
            var taxi = new TaxiSystem(100, 100);
            taxi.cars.Add(new Car("Дальняя", new Points(90, 90)));
            taxi.cars.Add(new Car("Ближняя", new Points(11, 10)));
            taxi.cars.Add(new Car("Средняя", new Points(50, 50)));
            var customer = new Customers("Тест", new Points(10, 10));
            var result = taxi.SearchFromAllSilent(customer);
            Assert.That(result[0].car.ID, Is.EqualTo("Ближняя"));
            Assert.That(result[1].car.ID, Is.EqualTo("Средняя"));
            Assert.That(result[2].car.ID, Is.EqualTo("Дальняя"));
            Assert.That(result[0].distance, Is.LessThan(result[1].distance));
            Assert.That(result[1].distance, Is.LessThan(result[2].distance));
        }
    }
}