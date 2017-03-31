using HViewer.Core;
using HViewer.Model;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace HViewer.View
{
    public sealed partial class CollectionPage : Page
    {
        public CollectionPage()
        {
            this.InitializeComponent();

            DoWorkAsync();
        }

        public int StartPage;
        public int CurrentPage;
        private Site site;
        private Dictionary<string, string> matchResult;
        private string currUrl;
        private string keyword;

        public async Task DoWorkAsync()
        {
            using (HttpClient hc = new HttpClient())
            {
                Uri uri = new Uri("https://raw.githubusercontent.com/H-Viewer-Sites/Index/master/sites/wnacg.txt");
                string s1 = await hc.GetStringAsync(uri);

                if (!string.IsNullOrEmpty(s1))
                {
                    site = JsonConvert.DeserializeObject<Site>(s1);

                    if (site != null)
                    {
                        
                        int page = 0;
                        ParseUrl(site.indexUrl);
                        currUrl = site.indexUrl;
                        string url;
                        if (CurrentPage == StartPage)
                        {
                            url = currUrl.Replace("{pageStr:(.*?{.*?}.*?)}", "")
                                    .Replace("{page:" + StartPage + "}", "" + page)
                                    .Replace("{keyword:}", keyword);
                        }
                        else
                        {
                            url = currUrl.Replace("{page:" + StartPage + "}", "" + page).Replace("{keyword:}", keyword);
                            if (matchResult.ContainsKey("pageStr"))
                            {
                                url.Replace("{pageStr:(.*?{.*?}.*?)}", (page == StartPage) ? "" : "" + matchResult["pageStr"]);
                            }

                        }
                        

                        string s = null;
                        try
                        {
                            //s = await hc.GetStringAsync(uri);
                            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                            httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36");
                            HttpResponseMessage responseAsync = await hc.SendRequestAsync(httpRequest);

                            responseAsync.EnsureSuccessStatusCode();

                            IBuffer asyncBuffer = await responseAsync.Content.ReadAsBufferAsync();
                            byte[] resultByteArray = asyncBuffer.ToArray();

                            EncodingProvider provider = CodePagesEncodingProvider.Instance;
                            Encoding.RegisterProvider(provider);

                            s = Encoding.GetEncoding("UTF-8").GetString(resultByteArray, 0, resultByteArray.Length);
                        }
                        catch (Exception e)
                        {
                            new MessageDialog(e.StackTrace, e.Message).ShowAsync();
                        }
                        if (!string.IsNullOrEmpty(s))
                        {
                            List<Collection> list = new List<Collection>();
                            RuleParser.GetCollections(list, s, site.indexRule, url, false);

                            gridView.ItemsSource = list;
                        }
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                else
                {
                    throw new Exception("Error");
                }
            }
        }

        private void ParseUrl(string url)
        {
            matchResult = RuleParser.ParseUrl(url);

            string pageStr = matchResult["page"];
            try
            {
                StartPage = (pageStr != null) ? int.Parse(pageStr) : 0;
            }
            catch (Exception)
            {
                StartPage = 0;
            }
            CurrentPage = StartPage;
        }

        private void Cover_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ImageEx image = (ImageEx)sender;
            image.Source = new BitmapImage(new Uri("ms-appx:///Assets/ImageLoadError.png"));
        }

        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var collection = e.ClickedItem as Collection;
            MainPage.Current.DetailFrame.Navigate(typeof(GalleryPage), new { site = site, collection = collection });
        }
    }
}
