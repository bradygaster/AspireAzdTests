using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Should;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AppTests : PageTest
{
    private async Task GetHomePage()
    {
        await Page.GotoAsync("https://webfrontend.livelytree-d998503c.eastus.azurecontainerapps.io/");

        await Expect(Page).ToHaveTitleAsync("Home");
    }

    [Test]
    public async Task AppIsDeployed()
    {
        await GetHomePage();
    }

    [Test]
    public async Task CounterWorks()
    {
        await GetHomePage();

        // create a locator
        var counterLink = Page.GetByRole(AriaRole.Link, new() { Name = "Counter" });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(counterLink).ToHaveAttributeAsync("href", "counter");

        // Click the get started link.
        await counterLink.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*counter"));

        // locate the counter status label
        (await Page.GetByText("Current count: 0").TextContentAsync())
            .ShouldEqual("Current count: 0");
    }

    [Test]
    public async Task ServiceToServiceWorks()
    {
        await GetHomePage();

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Weather" })
            .ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*weather"));

        // wait for the weather forecast to load
        await Task.Delay(2000);

        // make sure we have data
        (await Page.EvalOnSelectorAsync<int>("//table", "tbl => tbl.rows.length"))
            .ShouldBeGreaterThan(3);
    }

    [Test]
    public async Task RedisPubSubWorks()
    {
        await GetHomePage();

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Redis" })
            .ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*redis"));

        // enter text into the textbox
        (await Page.InputValueAsync("input[name=messageEntered]"))
            .ShouldEqual("asdfasdf");

        // expect the page to have a button
        var button = Page.GetByRole(AriaRole.Button, new() { Name = "Send" });

        (await Page.TextContentAsync("#messageContainer"))
            .ShouldEqual("No message yet");

        // click the button to send the message
        await button.ClickAsync(new()
        {
            Timeout = 5000
        });

        var val = await Page.TextContentAsync("#messageContainer");

        (await Page.TextContentAsync("#messageContainer"))
            .ShouldEqual("Sent to Redis: asdfasdf");
    }
}