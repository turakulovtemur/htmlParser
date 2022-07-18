using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Task2.Models;
using Task2.service;

namespace Task2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly PizzaService _pizzaService ;
        public HomeController(ILogger<HomeController> logger)
        {
            //_pizzaService = pizzaService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //List<Pizza> pizzas = new List<Pizza>()
            //{
            //    new Pizza
            //    {
            //        id = 0,
            //        name = "XXXL",
            //        photo = "/image/1.png",
            //        desc = "Сыр моцарелла, Соус '1000 островов', Куриный рулет, Ветчина, Колбаски охотничьи, Орегано, Бекон, Сервелат, Огурцы маринованные",
            //        size1 = "30",
            //        size2 = "40",
            //        size3 = "60",
            //        price = "690 ",
            //        weight = "1440 гр"
            //    },
            //    new Pizza
            //    {
            //        id = 1,
            //        name = "4 вкуса",
            //        photo = "/image/2.png",
            //        desc = "Соус '1000 островов', Сыр моцарелла, Рулет куриный, Ветчина, Пепперони, Сыр пармезан, Орегано,",
            //        size1 = "30",
            //        size2 = "40",
            //        size3 = "60",
            //        price = "420 ",
            //        weight = "600 гр"
            //    },
            //    new Pizza
            //    {
            //        id = 2,
            //        name = "Амазонка",
            //        photo = "/image/3.png",
            //        desc = "Сыр моцарелла, Соус '1000 островов', Куриный рулет, Ветчина, Колбаски охотничьи, Орегано, Бекон, Сервелат, Огурцы маринованные",
            //        size1 = "30",
            //        size2 = "40",
            //        size3 = "60",
            //        price = "420 ",
            //        weight = "600 гр"
            //    },
            //    new Pizza
            //    {
            //        id = 3,
            //        name = "БананZZа",
            //        photo = "/image/4.png",
            //        desc = "Бананы, Соус Гавайский, Сыр моцарелла, Ананас, Шоколад молочный, Кокос / миндаль, Орегано,",
            //        size1 = "30",
            //        size2 = "40",
            //        size3 = "60",
            //        price = "520 ",
            //        weight = "390 гр"
            //    }
            //};

            var pizzaFromHml = await GetPizzaFromHtml();

            return View(pizzaFromHml);            
        }
        //[HttpGet]
        //public IActionResult GetPizzas()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult GetPizzas()
        //{
        //    var testModel = _pizzaService.GetPizza();
        //    return View(testModel);
        //}

        public IActionResult Privacy()
        {
            return View();
        } 

        private async Task<IEnumerable<Pizza>> GetPizzaFromHtml()
        {
            string URL = "https://cagliari-pizza.ru/pizza.html";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);


            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //ответ от сервера
                var result = streamReader.ReadToEnd();

                IConfiguration config = Configuration.Default;
                IBrowsingContext context = BrowsingContext.New(config);
                IDocument document = await context.OpenAsync(req => req.Content(result.ToString()));

                var pizzaBlocks = document.QuerySelectorAll(".product_preview");

                List<Pizza> pizzas = new List<Pizza>();

                foreach(var item in pizzaBlocks)
                {
                    var photoUrl = item.QuerySelector(".product_preview_image")
                        .GetAttribute("style")
                        .Split(" ")[1];
                    var startIndex = 4;
                    var endLength = 2; // сколько отсекаем смволов с конца
                    var photo = photoUrl.Substring(startIndex, photoUrl.Length - endLength - startIndex);
                    item.QuerySelector(".product_preview_price").QuerySelector(".ruble").Remove();//удаляем знак рубля
                    var ingredient = item.QuerySelector(".product_preview_desc").QuerySelectorAll(".del_ingredient");//удаляем некоторый состав пиццы
                    foreach(var it in ingredient)
                    {
                        it.Remove();
                    }

                    pizzas.Add(new Pizza
                    {
                        name = item.QuerySelector(".product_preview_name").TextContent,
                        photo = $"https://cagliari-pizza.ru/{photo}",
                        desc = item.QuerySelector(".product_preview_desc").TextContent.TrimEnd(new char[] { ',', ' ' }),
                        size1 = item.QuerySelector(".size_1").QuerySelector(".name").TextContent.Split("/")[0],
                        size2 = item.QuerySelector(".size_2").QuerySelector(".name").TextContent.Split("/")[0],
                        size3 = item.QuerySelector(".size_3").QuerySelector(".name").TextContent.Split("/")[0],
                        price = item.QuerySelector(".product_preview_price").TextContent,
                        weight=item.QuerySelector(".product_preview_weight").TextContent
                    });
                }

                return pizzas;
            }
        }
    }
}
