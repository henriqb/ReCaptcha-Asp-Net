using System.Globalization;
using hbehr.recaptcha.Internazionalization;
using NUnit.Framework;

namespace hbehr.recaptcha.unittest
{
	[TestFixture]
	public class ExtensionTests
	{
		[Test]
		public void NoExceptionWhenChinese()
		{
			Assert.DoesNotThrow(() =>
			{
				CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");

				ReCaptchaLanguage.Auto.GetLanguage();
			});
		}	
		
		[Test]
		public void NoExceptionWhenNorwegianBokmal()
		{
			Assert.DoesNotThrow(() =>
			{
				CultureInfo.CurrentUICulture = new CultureInfo("nb-NO");

				ReCaptchaLanguage.Auto.GetLanguage();
			});
		}
	}
}