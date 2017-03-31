using HViewer.Core;
using HViewer.Data;
using HViewer.Model;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class GalleryPage : Page
    {
        private Collection _collection;
        private Site _site;

        private Collection _myCollection;

        private int _page = 1;
        private int _startPage;

        public GalleryPage()
        {
            this.InitializeComponent();


        }

        public async System.Threading.Tasks.Task LoadAsync()
        {
            string url = _site.getGalleryUrl(_collection.idCode, _page, _collection.pictures);

            using (HttpClient hc = new HttpClient())
            {
                string html = null;
                try
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
                    httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36");
                    HttpResponseMessage responseAsync = await hc.SendRequestAsync(httpRequest);

                    responseAsync.EnsureSuccessStatusCode();

                    IBuffer asyncBuffer = await responseAsync.Content.ReadAsBufferAsync();
                    byte[] resultByteArray = asyncBuffer.ToArray();

                    EncodingProvider provider = CodePagesEncodingProvider.Instance;
                    Encoding.RegisterProvider(provider);

                    html = Encoding.GetEncoding("UTF-8").GetString(resultByteArray, 0, resultByteArray.Length);
                }
                catch (Exception e)
                {
                    new MessageDialog(e.StackTrace, e.Message).ShowAsync();
                }
                if (!string.IsNullOrEmpty(html))
                {
                    _myCollection = RuleParser.GetCollectionDetail(_collection, html, _site.galleryRule, url);

                    this.galleryView.ItemsSource = _myCollection.pictures;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var type = e.Parameter?.GetType();
            if (type == null)
            {
                return;
            }

            this._site = type.GetProperty("site").GetValue(e.Parameter) as Site ;
            this._collection = type.GetProperty("collection").GetValue(e.Parameter) as Collection;

            LoadAsync();
        }

        private void Cover_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ImageEx image = (ImageEx)sender;
            image.Source = new BitmapImage(new Uri("ms-appx:///Assets/ImageLoadError.png"));
        }

        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

    }
}
