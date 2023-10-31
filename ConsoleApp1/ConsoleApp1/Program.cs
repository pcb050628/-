using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

class Program
{
    static void Main(string[] args)
    {
        string email = "아이디";
        string password = "비번";

        // 크롬 브라우저 인스턴스 생성
        var driver = new ChromeDriver();

        // 구글 홈페이지 열기
        driver.Navigate().GoToUrl("네이버 카페 주소");

        IWebElement loginButton = driver.FindElement(By.CssSelector("a.gnb_btn_login"));

        // 로그인 버튼을 클릭합니다.
        loginButton.Click();

        IWebElement idInput = driver.FindElement(By.Id("id"));
        IWebElement pwInput = driver.FindElement(By.Id("pw"));

        idInput.SendKeys(email);
        pwInput.SendKeys(password);

        while (true) // 로그인 후 콘솔창에서 엔터를 눌러주세요
        {
            if (Console.ReadKey().Key == ConsoleKey.Enter)
                break;
        }

        var myActivityLink = driver.FindElement(By.CssSelector("li.tit-action a._rosRestrict"));
        myActivityLink.Click();

        //아래 글과 댓글 중 하나만 주석 지워주세요

        // '내가 쓴 글 보기' 버튼 찾기
        var myArticlesButton = driver.FindElement(By.CssSelector("strong.gm-tcol-c > a[href*='/members/'][href*='tab=articles']"));

        // '내가 쓴 댓글보기' 버튼찾기
        //var myCommentButton = driver.FindElement(By.CssSelector("strong.gm-tcol-c > a[href*='/members/'][href*='tab=comments']"));

        // Actions 객체 생성
        var actions = new Actions(driver);

        // '내가 쓴 글 보기' 버튼을 Ctrl 키를 누른 상태에서 클릭
        actions.KeyDown(Keys.Control).Click(myArticlesButton).KeyUp(Keys.Control).Build().Perform();

        // '내가 쓴 댓글보기' 버튼을 Ctrl 키를 누른 상태에서 클릭
        //actions.KeyDown(Keys.Control).Click(myCommentButton).KeyUp(Keys.Control).Build().Perform();

        // 대기 시간 추가 (필요한 경우 값을 조정하세요)
        System.Threading.Thread.Sleep(3000);

        // 현재 열려있는 모든 탭의 핸들을 가져옵니다.
        var allTabHandles = driver.WindowHandles;

        // 새 탭으로 전환합니다.
        driver.SwitchTo().Window(allTabHandles[allTabHandles.Count - 1]);

        // 로딩이 완료될 때까지 기다립니다.
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        // 내가 쓴 글 보기 용
        wait.Until(drv => drv.Url.Contains("tab=articles"));

        // 내가 쓴 댓글보기 용
        //wait.Until(drv => drv.Url.Contains("tab=comments"));


        while (true)
        {
            // '전체선택' 체크박스가 나타날 때까지 기다립니다.
            wait.Until(drv => drv.FindElements(By.CssSelector("div.FormInputCheck.check_all input#chk_all")).Count > 0);

            // '전체선택' 체크박스를 찾습니다.
            var selectAllCheckbox = WaitUntilElementExists(driver, By.CssSelector("div.FormInputCheck.check_all label"));

            if (selectAllCheckbox == null)
                break;

            // 체크박스를 클릭합니다.
            selectAllCheckbox.Click();

            // '삭제' 버튼 찾기
            var deleteButton = WaitUntilElementExists(driver, By.CssSelector("button.BaseButton.BaseButton--skinGray.size_default"));

            // '삭제' 버튼 클릭
            deleteButton.Click();

            // 0.5초 기다리기
            System.Threading.Thread.Sleep(500);

            // Alert 창이 나타날 때까지 기다립니다.
            WebDriverWait alertWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IAlert alert = alertWait.Until(drv =>
            {
                try
                {
                    return drv.SwitchTo().Alert();
                }
                catch (NoAlertPresentException)
                {
                    return null;
                }
            });

            // Alert 창의 확인 버튼을 누릅니다.
            alert.Accept();

            // 글 삭제할때는 5초 권장
            System.Threading.Thread.Sleep(5000);
            //System.Threading.Thread.Sleep(2500);
        }

        // 브라우저 종료
        driver.Quit();
    }

    static IWebElement WaitUntilElementExists(IWebDriver driver, By by, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        return wait.Until(drv => drv.FindElement(by));
    }
}
