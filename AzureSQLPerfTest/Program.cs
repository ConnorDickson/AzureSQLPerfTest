using AzureSQLPerfTest.DAL;
using AzureSQLPerfTest.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AzureSQLPerfTest
{
    class Program
    {
        public static TelemetryClient logger;
        static void Main(string[] args)
        {
            Console.WriteLine("Obtaining SQL Connection String");
            var sqlConnectionString = Environment.GetEnvironmentVariable("TestSQLConnectionString");

            Console.WriteLine("Obtaining App Insights Instrumentation Key");
            var instrumentationKey = Environment.GetEnvironmentVariable("InstrumentationKey");

            Console.WriteLine("Connection to App Insights");
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = instrumentationKey;
            logger = new TelemetryClient(configuration);

            //I'm purposefully not keeping a single context alive for these tests
            Console.WriteLine("Starting SQL StoreRecords Test Run");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Removing added records {i}");
                DeleteAllTestsFromRecords(sqlConnectionString);
                Console.WriteLine($"Executing SQL StoreRecords Run {i}");
                PerfTestStoreRecords(1000, sqlConnectionString);
            }

            Console.WriteLine("Starting SQL ReadRecords Test Run");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Executing SQL ReadRecords Run {i}");
                PerfTestReadRecords(sqlConnectionString);
            }

            // You have to wait for App Insights to finish...
            logger.Flush();
            Task.Delay(5000).GetAwaiter().GetResult();
        }

        private static void DeleteAllTestsFromRecords(string connectionString)
        {
            using (var context = new TestContext(connectionString))
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Tests]");
            }
        }

        private static void PerfTestReadRecords(string connectionString)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            using (var context = new TestContext(connectionString))
            {
                var tests = context.Tests;

                int count = 0;

                foreach(var test in tests)
                {
                    count++;
                }

                stopwatch.Stop();

                logger.TrackMetric($"ReadRecords{count}", stopwatch.ElapsedMilliseconds);
            }
        }

        public static void PerfTestStoreRecords(int countOfRecords, string connectionString)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            using (var context = new TestContext(connectionString))
            {
                var tests = new List<Test>();
                for (int i = 0; i < countOfRecords; i++)
                {
                    tests.Add(new Test { FirstName = "Test", LastName = "Test", DateOfBirth = "", FavouriteColour = FavouriteColour.Black });
                }

                tests.ForEach(t => context.Tests.AddOrUpdate(t));
                context.SaveChanges();
                stopwatch.Stop();

                logger.TrackMetric($"StoreRecords{countOfRecords}", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
