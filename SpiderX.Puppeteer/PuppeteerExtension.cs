using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpiderX.Puppeteer
{
	public static class PuppeteerExtension
	{
		public static async Task ScrollBy(this Page page, int x, int y)
		{
			await page.EvaluateFunctionAsync("_ => {window.scrollBy(" + x.ToString() + "," + y.ToString() + ")}");
		}
	}
}