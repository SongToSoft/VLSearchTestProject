using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

//Желательно проводить по несколько тестов, в случае запуска всех сразу возможно 
//исключение из-за таймаута в 60 секунд (так было на моём ПК несколько раз).
//Чаще все тесты проходили успешно.

namespace VLSearchTestProject
{
    [TestClass]
    public class SearchTests
    {
        private ChromeDriver Chrome;
        //Набор действий, с которых начинается каждый тест поиска
        private void StartTest()
        {
            Chrome = new ChromeDriver();
            Chrome.Navigate().GoToUrl("https://www.vl.ru/");
            //Находим символ лупы и нажимаем на него
            var SearchButton = Chrome.FindElement(By.ClassName("header__search-form-toggle"));
            SearchButton.Click();
        }
        //Непосредственный поисковый запрос
        private void DoQuery(string StartString, string CheckString)
        {
            StartTest();
            //Непосредственный ввод запроса в поисковую строку
            var SearchPanel = Chrome.FindElement(By.ClassName("header__search-query"));
            SearchPanel.Clear();
            SearchPanel.SendKeys(StartString);
            SearchPanel.Submit();
            var NewSearchPanel = Chrome.FindElement(By.ClassName("search-control"));
            NewSearchPanel.Clear();
            string One = "//*[contains(., '";
            string Two = "')]";
            var Query = Chrome.FindElement(By.XPath(One + CheckString + Two));
            DoCheckQuery(Query);
        }
        //Проверка параметров, которые должны находиться на открывшейся вкладке
        //при корректном запросе
        private void DoCheckQuery(IWebElement Query)
        {
            try
            {
                var QueryResult = Chrome.FindElement(By.XPath("//*[contains(.,'во всех разделах')]"));
                if ((Query != null) && (QueryResult != null))
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (NoSuchElementException)
            {
                try 
                {
                    var QueryList = Chrome.FindElement(By.XPath("//*[contains(.,'По запросу показан раздел')]"));
                    if ((Query != null) && (QueryList != null))
                        Assert.IsTrue(true);
                    else
                        Assert.IsTrue(false);
                }
                catch (NoSuchElementException)
                {
                    try
                    {
                        var QueryList = Chrome.FindElement(By.XPath("//*[contains(.,'По запросу показана компания')]"));
                        if ((Query != null) && (QueryList != null))
                            Assert.IsTrue(true);
                        else
                            Assert.IsTrue(false);
                    }
                    catch (NoSuchElementException)
                    {
                        try
                        {
                            var QueryList = Chrome.FindElement(By.XPath("//*[contains(.,'По запросу показана целевая страница')]"));
                            if ((Query != null) && (QueryList != null))
                                Assert.IsTrue(true);
                            else
                                Assert.IsTrue(false);
                        }
                        catch (NoSuchElementException)
                        {
                            Assert.IsTrue(false);
                        }
                    }
                }
            }
        }
        [TestMethod]
        public void TestVoidQuery()
        {
            StartTest();
            var SearchPanel = Chrome.FindElement(By.ClassName("header__search-query"));
            SearchPanel.Clear();
            SearchPanel.Submit();
            if (Chrome.Url == "https://www.vl.ru/vladivostok?search=&utm_source=vl.ru&utm_medium=header_search&utm_term=")
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }
        [TestMethod]
        public void TestCorrectRussianQuery() => DoQuery("ДНС", "DNS");
        [TestMethod]
        public void TestInCorrectRussianQuery()
        {
            StartTest();
            var SearchPanel = Chrome.FindElement(By.ClassName("header__search-query"));
            SearchPanel.Clear();
            SearchPanel.SendKeys("ывафвавыа");
            SearchPanel.Submit();
            var QueryResult = Chrome.FindElement(By.XPath("//*[contains(.,'Ничего не найдено')]"));
            if (QueryResult != null)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }
        [TestMethod]
        public void TestCorrectEnglishQuery() => DoQuery("PrimPress", "PrimPress");
        [TestMethod]
        public void TestInCorrectEnglishQuery()
        {
            StartTest();
            var SearchPanel = Chrome.FindElement(By.ClassName("header__search-query"));
            SearchPanel.Clear();
            SearchPanel.SendKeys("sdfsdafsadf");
            SearchPanel.Submit();
            var QueryResult = Chrome.FindElement(By.XPath("//*[contains(.,'Ничего не найдено')]"));
            if (QueryResult != null)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }
        [TestMethod]
        public void TestCompany() => DoQuery("Game Forest", "Форест");
        [TestMethod]
        public void TestService() => DoQuery("Аварийное открывание дверей", "Аварийное открывание дверей");
        [TestMethod]
        public void TestProduct() => DoQuery("Компьютеры", "Компьютеры");
        [TestMethod]
        public void TestSwapEE() => DoQuery("цёны на нёдвижимость", "Недвижимость");
        [TestMethod]
        public void TestQuotes() => DoQuery("'Декларант'", "Декларант");
        [TestMethod]
        public void TestDouble() => DoQuery("Smart Smart Smart Smart Smart Smart", "Smart");
        [TestMethod]
        public void TestUpperCase() => DoQuery("HESBURGER", "Hesburger");
        [TestMethod]
        public void TestLowerCase() => DoQuery("farpost", "Farpost");
        [TestMethod]
        public void TestDifferentCase() => DoQuery("аЛьТа-СоФт", "Альта-Софт");
        [TestMethod]
        public void TestRussianError() => DoQuery("Кридеты", "Банки");
        [TestMethod]
        public void TestEnglishError() => DoQuery("Keonjoat", "Koonjoot");
        [TestMethod]
        public void TestInComplete() => DoQuery("нов", "Новый");
        [TestMethod]
        public void TestWithoutSpaces() => DoQuery("DNSSmart", "DNS Smart");
        [TestMethod]
        public void TestDate() => DoQuery("17.09.2018", "17.09.2018");
        [TestMethod]
        public void TestManySpaces() => DoQuery("Карта      Владивостока", "Карта Владивостока");
        [TestMethod]
        public void TestEnglishLayout() => DoQuery("Bkk.pbjy", "Иллюзион");
        [TestMethod]
        public void TestRussianLayout() => DoQuery("афкзщые", "FarPost");
        [TestMethod]
        public void TestAddress() => DoQuery("Давыдова 40", "Давыдова 40");
        [TestMethod]
        public void TestStreet() => DoQuery("ул. Давыдова", "ул. Давыдова");
        [TestMethod]
        public void TestPlus() => DoQuery("Восточная+Торговая+Компания", "Восточная Торговая Компания");
        [TestCleanup]
        public void TearDown() => Chrome.Quit();
    }
}
