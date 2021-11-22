﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MissGuided.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MissGuided.Services
{
    public class CartAPI : ContentPage
    {
        HttpClient client;
        string url = "https://miss-guided-server.herokuapp.com";
        JsonSerializer json_serializer = new JsonSerializer();
        static ProductAPI _getService;
        public static ProductAPI shared = new ProductAPI();
        // GET method
        public static ProductAPI getService
        {
            get
            {
                _getService = new ProductAPI();
                return _getService;
            }
        }

        public CartAPI()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> AddToCart(string productID)
        {
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(productID), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/user/addToCart", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public async Task<bool> RemoveFromCart(string productID)
        {
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(productID), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/user/removeFromCart", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public async Task<List<Product>> FetchCart()
        {
            try
            {
                List<Product> products = new List<Product>();
                //var content = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
                //string strPage = page.ToString();
                string userEmail = Preferences.Get("userEmail", "No email");
                var response = await client.GetAsync("/user/getCart/"+userEmail);

                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    var jsonContent = json_serializer.Deserialize<Products>(json);
                    products = jsonContent.products;
                    return products;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }
    }
}
