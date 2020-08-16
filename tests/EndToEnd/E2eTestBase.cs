﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using EndToEnd.Helpers;
using EndToEnd.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Serilog;

namespace EndToEnd
{
    public class E2ETestBase : IDisposable
    {
        private const string SettingsFile = "testsettings.json";
        public PaypalSettings PaypalSettings;
        public static RemoteWebDriver Driver;
        public static EndToEndTestHelper Helper;
        public static ILogger _logger;
        private static string TestRunId = Guid.NewGuid().ToString().Substring(0, 5);
        public static TestSettings TestSettings;

        public E2ETestBase()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(SettingsFile).Build();
            _logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            TestSettings = configuration.Get<TestSettings>();
            PaypalSettings = JsonConvert.DeserializeObject<PaypalSettings>(File.ReadAllText(TestSettings.PaypalSettingsPath));
            Helper = new EndToEndTestHelper(TestSettings.ConnectionString);
            InitDriver();
        }

        private void InitDriver()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TestSettings.ImplicitWait);
            Driver.Manage().Window.Maximize();
        }

        public void Dispose()
        {
            Cleanup();
            Driver.Close();
            Driver.Dispose();
        }


        protected async Task Cleanup()
        {
            await using var conn = new SqlConnection(TestSettings.ConnectionString);

            //deletes
            var queries = new List<string>()
            {
                "DELETE FROM dbo.Cart",
                "DELETE FROM dbo.Styles",
                "DELETE FROM dbo.ProductCategories",
                "DELETE FROM dbo.ProductImages",
                "DELETE FROM dbo.AspNetUsers",
                "DELETE FROM dbo.AspNetUserRoles",
                "DELETE FROM dbo.AspNetRoles",
                "DELETE FROM dbo.ProductCategories",
                "DELETE FROM dbo.Categories",
                "DELETE FROM dbo.Colors",
                "DELETE FROM dbo.Sizes",
                "DELETE FROM dbo.Products",
                "DELETE FROM dbo.Images",
                "DELETE FROM dbo.SiteSettings",
                "DELETE FROM dbo.OrderItems",
                "DELETE FROM dbo.Orders"
            };
            foreach (var query in queries)
            {
                await conn.ExecuteAsync(query);
            }


            //erase dead letter files
            var di = new DirectoryInfo(TestSettings.DeadLetterPath);
            foreach (var file in di.GetFiles()) { file.Delete(); }
        }
    }
}
