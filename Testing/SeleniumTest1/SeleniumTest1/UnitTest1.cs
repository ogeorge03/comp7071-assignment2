using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace SeleniumTest1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            // Assert.Pass();

            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            ChromeDriver driver = new ChromeDriver(path + @"\drivers\");
            string url = "https://localhost:7182";
            //ChromeDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.LinkText("Facilities")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.Id("Name")).SendKeys("ABCD");
            driver.FindElement(By.Id("Room")).SendKeys("123");
            driver.FindElement(By.Id("Description")).SendKeys("Extra room");
            driver.FindElement(By.Id("Room_Rate")).SendKeys("100");
            driver.FindElement(By.Id("St_Address")).SendKeys("1");

            driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
            Assert.Pass();

        }


        [TearDownAttribute]
        public void TearDown()
        {
        }

    }
}