﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Xml.Linq;

namespace Test1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool toggle = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void showBtn_Click(object sender, RoutedEventArgs e)
        {
            // IP API URL
            var Ip_Api_Url = "http://ip-api.com/json/"+ this.ipTbx.Text; // This is a sample IP address. You can pass yours if you want to test          

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
                        result += "Country: " + geolocationInfo.Country +"\n";
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

        private void geoButton_Click(object sender, RoutedEventArgs e)
        {
            var geo_API_Url = "https://api.opencagedata.com/geocode/v1/xml?key=a8b334461f3a45ca8f6bb6cfcca729e5&language=he&pretty=1" + "&q=" + geoTxtBx.Text;
            // Use HttpClient to get the details from the Json response
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

                    this.geoTxtBlk.Text = String.Format("{0}, {1}", xElement.Element("lat").Value, xElement.Element("lng").Value);

                }
            }
        }

        private void geoToggle_Click(object sender, RoutedEventArgs e)
        {
            this.geoTxtBlk.Text = "";
            //this.geoTxtBx.Text = "31.8414127, 35.2473471";
            switch (toggle)
            {
                case false:
                    this.geoTxtBx.Text = "32.17094, 35.08297";
                    toggle = true;
                    this.reverseBtn.IsEnabled = true;
                    this.geoButton.IsEnabled = false;
                    break;
                case true:
                    this.geoTxtBx.Text = "Kfar Ivri 10, Jerusalem";
                    toggle = false;
                    this.reverseBtn.IsEnabled = false;
                    this.geoButton.IsEnabled = true;
                    break;
            }
        }

        private void reverseBtn_Click(object sender, RoutedEventArgs e)
        {
            var geo_API_Url = "https://api.opencagedata.com/geocode/v1/xml?key=a8b334461f3a45ca8f6bb6cfcca729e5&language=en&pretty=1" + "&q=" + geoTxtBx.Text;
            // Use HttpClient to get the details from the Json response
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
                    var xElement = XElement.Load(geolocationInfo1)?.Element("results")?.Element("result")?.Element("formatted");

                    this.geoTxtBlk.Text = xElement?.Value.ToString()  ;

                }
            }
        }
    }
}

