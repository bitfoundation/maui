using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using NUnit.Framework;
using System.Maui.PlatformConfiguration;
using System.Maui.PlatformConfiguration.AndroidSpecific;
using System.Maui.PlatformConfiguration.WindowsSpecific;

using WindowsOS = System.Maui.PlatformConfiguration.Windows;

namespace System.Maui.Core.UnitTests
{
	[TestFixture]
	public class WebViewUnitTests : BaseTestFixture
	{
		[Test]
		public void TestSourceImplicitConversion ()
		{
			var web = new WebView ();
			Assert.Null (web.Source);
			web.Source = "http://www.google.com";
			Assert.NotNull (web.Source);
			Assert.True (web.Source is UrlWebViewSource);
			Assert.AreEqual ("http://www.google.com", ((UrlWebViewSource)web.Source).Url);
		}

		[Test]
		public void TestSourceChangedPropagation ()
		{
			var source = new UrlWebViewSource {Url ="http://www.google.com"};
			var web = new WebView { Source = source };
			bool signaled = false;
			web.PropertyChanged += (sender, args) => {
				if (args.PropertyName == WebView.SourceProperty.PropertyName)
					signaled = true;
			};
			Assert.False (signaled);
			source.Url = "http://www.xamarin.com";
			Assert.True (signaled);
		}

		[Test]
		public void TestSourceDisconnected ()
		{
			var source = new UrlWebViewSource {Url="http://www.google.com"};
			var web = new WebView { Source = source };
			web.Source = new UrlWebViewSource {Url="Foo"};
			bool signaled = false;
			web.PropertyChanged += (sender, args) => {
				if (args.PropertyName == WebView.SourceProperty.PropertyName)
					signaled = true;
			};
			Assert.False (signaled);
			source.Url = "http://www.xamarin.com";
			Assert.False (signaled);
		}

		class ViewModel
		{
			public string HTML { get; set; } = "<html><body><p>This is a WebView!</p></body></html>";

			public string URL { get; set; } = "http://xamarin.com";

		}

		[Test]
		public void TestBindingContextPropagatesToSource ()
		{
			var htmlWebView = new WebView {
			};
			var urlWebView = new WebView {
			};

			var htmlSource = new HtmlWebViewSource ();
			htmlSource.SetBinding (HtmlWebViewSource.HtmlProperty, "HTML");
			htmlWebView.Source = htmlSource;

			var urlSource = new UrlWebViewSource ();
			urlSource.SetBinding (UrlWebViewSource.UrlProperty, "URL");
			urlWebView.Source = urlSource;

			var viewModel = new ViewModel ();

			var container = new StackLayout {
				BindingContext = viewModel,
				Padding = new Size (20, 20),
				Children = {
					htmlWebView,
					urlWebView
				}
			};

			Assert.AreEqual ("<html><body><p>This is a WebView!</p></body></html>", htmlSource.Html);
			Assert.AreEqual ("http://xamarin.com", urlSource.Url);
		}

		[Test]
		public void TestAndroidMixedContent()
		{
			var defaultWebView = new WebView();

			var mixedContentWebView = new WebView();
			mixedContentWebView.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);

			Assert.AreEqual(defaultWebView.On<Android>().MixedContentMode(), MixedContentHandling.NeverAllow);
			Assert.AreEqual(mixedContentWebView.On<Android>().MixedContentMode(), MixedContentHandling.AlwaysAllow);
		}

		[Test]
		public void TestEnableZoomControls()
		{
			var defaultWebView = new WebView();

			var enableZoomControlsWebView = new WebView();
			enableZoomControlsWebView.On<Android>().SetEnableZoomControls(true);

			Assert.AreEqual(defaultWebView.On<Android>().ZoomControlsEnabled(), false);
			Assert.AreEqual(enableZoomControlsWebView.On<Android>().ZoomControlsEnabled(), true);
		}

		[Test]
		public void TestDisplayZoomControls()
		{
			var defaultWebView = new WebView();

			var displayZoomControlsWebView = new WebView();
			displayZoomControlsWebView.On<Android>().SetDisplayZoomControls(false);

			Assert.AreEqual(defaultWebView.On<Android>().ZoomControlsDisplayed(), true);
			Assert.AreEqual(displayZoomControlsWebView.On<Android>().ZoomControlsDisplayed(), false);
		}

		[Test]
		public void TestWindowsSetAllowJavaScriptAlertsFlag()
		{
			var defaultWebView = new WebView();

			var jsAlertsAllowedWebView = new WebView();
			jsAlertsAllowedWebView.On<WindowsOS>().SetIsJavaScriptAlertEnabled(true);

			Assert.AreEqual(defaultWebView.On<WindowsOS>().IsJavaScriptAlertEnabled(), false);
			Assert.AreEqual(jsAlertsAllowedWebView.On<WindowsOS>().IsJavaScriptAlertEnabled(), true);
		}

		[Test]
		public void TestSettingOfCookie()
		{
			var defaultWebView = new WebView();
			var CookieContainer = new CookieContainer();

			CookieContainer.Add(new Cookie("TestCookie", "My Test Cookie...", "/", "microsoft.com"));

			defaultWebView.Cookies = CookieContainer;
			defaultWebView.Source = "http://xamarin.com";

			Assert.IsNotNull(defaultWebView.Cookies);
		}
	}
}
