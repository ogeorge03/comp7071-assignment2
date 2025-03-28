/*
====================================================================
📄 ServiceCompletionTests.cs
✅ Selenium UI Tests for Service Appointment Completion Status

🎯 Test Scenarios:
1. CompleteServiceAppointment_ShouldUpdateStatusSuccessfully (Positive Test)
    - Navigates to Service Appointments page.
    - Edits an existing appointment.
    - Updates the completion status.
    - Submits and verifies that the update was successful.

2. CompleteServiceAppointment_ShouldShowErrorOnInvalidData (Negative Test)
    - Edits an existing appointment.
    - Clears required field (Notes).
    - Submits and verifies that validation error is displayed.

⚙️ Dependencies:
- ASP.NET app running at https://localhost:7182 (update port if needed).
- Chrome browser installed.
- ChromeDriver placed in /drivers folder.
- At least one existing Service Appointment in database.

📌 Notes:
- Replace field IDs/names if they differ from actual .cshtml form.
- This is a UI flow test, not a backend or unit test.
- Test data is hardcoded; no rollback done.
- See README.md in project folder for instructions on running tests.

📂 Author: Anadi Mishra
📅 Date: 28-Mar-2024
====================================================================
*/

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;

namespace SeleniumTest1
{
    public class ServiceCompletionTests
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
        public void CompleteServiceAppointment_ShouldUpdateStatusSuccessfully()
        {
            // Navigate to Service Appointments index page
            driver.Navigate().GoToUrl(baseUrl + "/ServiceAppointments");

            // Click Edit on the first appointment (adjust selector if needed)
            driver.FindElement(By.LinkText("Edit")).Click();

            // Change Completion Status - Example: check a checkbox
            var statusCheckbox = driver.FindElement(By.Id("Completion_Status"));
            if (!statusCheckbox.Selected)
            {
                statusCheckbox.Click();
            }

            // Submit the form
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify redirection and/or success confirmation
            Assert.IsTrue(driver.Url.Contains("/ServiceAppointments")
                || driver.PageSource.Contains("Service Appointment updated successfully"));
        }

        [Test]
        public void CompleteServiceAppointment_ShouldShowErrorOnInvalidData()
        {
            // Navigate to Service Appointments index page
            driver.Navigate().GoToUrl(baseUrl + "/ServiceAppointments");

            // Click Edit on the first appointment
            driver.FindElement(By.LinkText("Edit")).Click();

            // Clear a mandatory field (Notes)
            var notesField = driver.FindElement(By.Id("Notes"));
            notesField.Clear();

            // Submit the form
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Check for validation message (adjust text as per your implementation)
            Assert.IsTrue(driver.PageSource.Contains("The Notes field is required")
                || driver.PageSource.Contains("Please enter notes"));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
