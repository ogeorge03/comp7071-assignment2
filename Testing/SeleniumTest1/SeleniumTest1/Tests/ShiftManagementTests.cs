using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;

namespace SeleniumTest1
{

    /*
        ====================================================================
            📄 ShiftManagementTests.cs
            ✅ Selenium UI Tests for Employee Shift Management

            🎯 Test Scenarios:
            - Assign new shift to employee (positive case)
            - Edit existing shift details (positive case)
            - Attempt shift assignment with invalid data (negative case)

            ⚙️ Dependencies:
            - ASP.NET App running at https://localhost:7182
            - Chrome browser & driver in /drivers folder
            - Existing EmployeeId = 1 and ShiftId = 1 (update as needed)

            📌 Test run instructions:
            Refer to README.md in Testing/SeleniumTest1 folder.

            📝 Notes:
            - Make sure EmployeeId and ShiftId used in tests exist in your local database.
            - Adjust form field IDs (EmployeeId, ShiftDate, StartTime, EndTime, SubmitButton) if your .cshtml pages use different names.
            - These are UI flow tests, not API or backend tests.
            - Test data is dummy, no database rollback.
            - Designed for learning, not production-ready QA pipeline.

            📂 Author: Anadi Mishra
            📅 Date: 28-Mar-2024
        ====================================================================
        */

    public class ShiftManagementTests
    {
        private ChromeDriver driver;
        private string baseUrl = "https://localhost:7182"; // Adjust port if needed

        [SetUp]
        public void Setup()
        {
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            driver = new ChromeDriver(path + @"\drivers\");
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        public void AssignShift_ShouldAssignShiftSuccessfully()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Employees/Assign");

            // Replace these IDs with your actual field IDs
            driver.FindElement(By.Id("EmployeeId")).SendKeys("1");
            driver.FindElement(By.Id("ShiftDate")).SendKeys("2025-04-01");
            driver.FindElement(By.Id("StartTime")).SendKeys("09:00 AM");
            driver.FindElement(By.Id("EndTime")).SendKeys("05:00 PM");

            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Success Message (Change text as per your implementation)
            Assert.IsTrue(driver.PageSource.Contains("Shift assigned successfully"));
        }

        [Test]
        public void EditShift_ShouldEditShiftSuccessfully()
        {
            int shiftId = 1; // Replace with valid shift ID
            driver.Navigate().GoToUrl(baseUrl + $"/Employees/EditShift/{shiftId}");

            var startTimeField = driver.FindElement(By.Id("StartTime"));
            startTimeField.Clear();
            startTimeField.SendKeys("10:00 AM");

            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Success Message
            Assert.IsTrue(driver.PageSource.Contains("Shift updated successfully"));
        }

        [Test]
        public void AssignShift_ShouldShowErrorOnInvalidData()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Employees/Assign");
            driver.FindElement(By.Id("EmployeeId")).SendKeys("");
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();
            Assert.IsTrue(driver.PageSource.Contains("This field is required"));
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
