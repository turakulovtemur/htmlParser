using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Task2.Models;

namespace Task2.service
{
    public class PizzaService
    {
        string URL = "https://cagliari-pizza.ru/pizza.html";

        public string GetPizza()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);


            httpWebRequest.ContentType = "text/json";

            httpWebRequest.Method = "GET";//Можно GET
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            //httpWebRequest.Headers.Add("api_key", "3qoY9i4HFXTbdsjOKQ9oHJ6PSWNOnSXM");
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //ответ от сервера
                var result = streamReader.ReadToEnd();

                //Десериализация
                Pizza pizzaModel = JsonConvert.DeserializeObject<Pizza>(result);

                return pizzaModel.ToString();
            }

        }
    }
}
