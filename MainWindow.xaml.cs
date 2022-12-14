using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace UsingGeoLocation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool toggle = false;

        readonly String APIKey = "a8b334461f3a45ca8f6bb6cfcca729e5";
        readonly String site = "https://api.opencagedata.com/geocode/v1/";

        public MainWindow()
        {
            InitializeComponent();
        }

        #region IP to address with Json
        private void showBtn_Click(object sender, RoutedEventArgs e)
        {
            // IP API URL
            var Ip_Api_Url = "http://ip-api.com/json/" + this.ipTbx.Text; // This is a sample IP address. You can pass yours if you want to test          

            // Use HttpClient to get the details from the Json response
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                // Pass API address to get the Geolocation details 
                httpClient.BaseAddress = new Uri(Ip_Api_Url);
                HttpResponseMessage httpResponse = httpClient.GetAsync(Ip_Api_Url).GetAwaiter().GetResult();
                // If API is success and receive the response, then get the location details
                if (httpResponse.IsSuccessStatusCode)
                {
                    //var geolocationInfo = httpResponse.Content.ReadAsAsync<LocationDetails_IpApi>().GetAwaiter().GetResult();
                    var geolocationInfo = httpResponse.Content.ReadFromJsonAsync<LocationDetails_IpApi>().GetAwaiter().GetResult();
                    if (geolocationInfo != null)
                    {
                        String result = "";
                        result += "Country: " + geolocationInfo.Country + "\n";
                        result += "Region: " + geolocationInfo.RegionName + "\n";
                        result += "City: " + geolocationInfo.City + "\n";
                        result += "Zip: " + geolocationInfo.Zip + "\n";
                        result += "Latitude: " + geolocationInfo.Lat + "\n";
                        result += "Longitude: " + geolocationInfo.Lon + "\n";

                        this.infoTxtBlk.Text = result;
                    }
                }
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            this.infoTxtBlk.Text = "";
        }
        #endregion IP to address with Json

        #region with XML
        private void geoXMLButton_Click(object sender, RoutedEventArgs e)
        {
            string geo_API_Url = site + "xml";
            geo_API_Url += "?key=" + APIKey;
            geo_API_Url += "&language=he&pretty=1";
            geo_API_Url += "&q=" + geoTxtBx.Text;

            // Use HttpClient to get the details from the xml response
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                // Pass API address to get the Geolocation details 
                httpClient.BaseAddress = new Uri(geo_API_Url);
                HttpResponseMessage httpResponse = httpClient.GetAsync(geo_API_Url).GetAwaiter().GetResult();
                // If API is success and receive the response, then get the location details
                if (httpResponse.IsSuccessStatusCode)
                {
                    var geolocationInfo1 = httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                    var xElement = XElement.Load(geolocationInfo1)?.Element("results")?.Element("result")?.Element("geometry");

                    this.geoTxtBlk.Text = String.Format("{0}, {1}", xElement?.Element("lat")?.Value, xElement?.Element("lng")?.Value);

                }
            }
        }

        private void reverseXMLBtn_Click(object sender, RoutedEventArgs e)
        {
            string geo_API_Url = site + "xml";
            geo_API_Url += "?key=" + APIKey;
            geo_API_Url += "&language=en&pretty=1";
            geo_API_Url += "&q=" + geoTxtBx.Text;

            //var geo_API_Url = "https://api.opencagedata.com/geocode/v1/xml?key=a8b334461f3a45ca8f6bb6cfcca729e5&language=en&pretty=1" + "&q=" + geoTxtBx.Text;
            //// Use HttpClient to get the details from the xml response
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                // Pass API address to get the Geolocation details 
                httpClient.BaseAddress = new Uri(geo_API_Url);
                HttpResponseMessage httpResponse = httpClient.GetAsync(geo_API_Url).GetAwaiter().GetResult();
                // If API is success and receive the response, then get the location details
                if (httpResponse.IsSuccessStatusCode)
                {
                    var geolocationInfo1 = httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                    var result = XElement.Load(geolocationInfo1)?.Element("results")?.Element("result")?.Element("formatted");

                    this.geoTxtBlk.Text = result?.Value.ToString();

                }
            }
        }
        #endregion with XML

        #region with Json
        private void geoJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO:
            string geo_API_Url = site + "json";
            geo_API_Url += "?key=" + APIKey;
            geo_API_Url += "&language=he&pretty=1";
            geo_API_Url += "&q=" + geoTxtBx.Text;

            // Use HttpClient to get the details from the xml response
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                // Pass API address to get the Geolocation details 
                httpClient.BaseAddress = new Uri(geo_API_Url);
                HttpResponseMessage httpResponse = httpClient.GetAsync(geo_API_Url).GetAwaiter().GetResult();
                // If API is success and receive the response, then get the location details
                if (httpResponse.IsSuccessStatusCode)
                {
                    var stream = httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult();

                    var serializer = new JsonSerializer();

                    using (var sr = new StreamReader(stream))
                    {
                        using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                        {
                            string? result = serializer.Deserialize(jsonTextReader)?.ToString();
                            if (result != null)
                            {
                                JToken? token = JObject.Parse(result).SelectToken("results[0].geometry");
                                this.geoTxtBlk.Text = $"{(string?)token?.SelectToken("lat")}, {(string?)token?.SelectToken("lng")}";
                            }
                        }
                    }


                }
            }
        }
        private void reverseJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            string geo_API_Url = site + "json";
            geo_API_Url += "?key=" + APIKey;
            geo_API_Url += "&language=en&pretty=1";
            geo_API_Url += "&q=" + geoTxtBx.Text;

            //var geo_API_Url = "https://api.opencagedata.com/geocode/v1/json?key=a8b334461f3a45ca8f6bb6cfcca729e5&language=en&pretty=1" + "&q=" + geoTxtBx.Text;
            //// Use HttpClient to get the details from the xml response
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                // Pass API address to get the Geolocation details 
                httpClient.BaseAddress = new Uri(geo_API_Url);
                HttpResponseMessage httpResponse = httpClient.GetAsync(geo_API_Url).GetAwaiter().GetResult();
                // If API is success and receive the response, then get the location details
                if (httpResponse.IsSuccessStatusCode)
                {
                    var stream = httpResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                    //TODO: replace with Jason similar feature
                    var serializer = new JsonSerializer();

                    using (var sr = new StreamReader(stream))
                    {
                        using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                        {
                            string? result = serializer.Deserialize(jsonTextReader)?.ToString();
                            if (result != null)
                            {
                                var txt = JObject.Parse(result)?.SelectToken("results[0].formatted")?.ToString();
                                this.geoTxtBlk.Text = txt;
                            }
                        }
                    }
                }
             }
        }
        #endregion with Json

        private void geoToggle_Click(object sender, RoutedEventArgs e)
        {
            // should be change using WPF binding feature

            this.geoTxtBlk.Text = "";
            //this.geoTxtBx.Text = "31.8414127, 35.2473471";
            switch (toggle)
            {
                case false:
                    this.geoTxtBx.Text = "31.8414127, 35.2473471";
                    toggle = true;
                    this.reverseXMLBtn.IsEnabled = true;
                    this.reverseJsonBtn.IsEnabled = true;
                    this.geoXMLButton.IsEnabled = false;
                    this.geoJsonButton.IsEnabled = false;
                    break;
                case true:
                    this.geoTxtBx.Text = "Kfar Ivri 10, Jerusalem";
                    toggle = false;
                    this.reverseXMLBtn.IsEnabled = false;
                    this.reverseJsonBtn.IsEnabled = false;
                    this.geoXMLButton.IsEnabled = true;
                    this.geoJsonButton.IsEnabled = true;
                    break;
            }
        }

    }
}

