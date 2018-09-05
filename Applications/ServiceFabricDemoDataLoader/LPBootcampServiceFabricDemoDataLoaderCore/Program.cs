using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Globalization;

using Microsoft.Extensions.Configuration;

namespace LPBootcampServiceFabricDemoDataLoaderCore
{
    class Program
    {

        static string[] ITEMS = new string[] {
            "111, NIKE PITCH PREMIER LEAGUE FOOTBALL 2017/2018, 29.00",
            "112, Nike Premier League Pitch Soccer Ball, 140.95",
            "113, adidas Performance Confederations Cup Top Glider Soccer Ball, 122.99",
            "114, Ultra-Durable Soccer Balls from One World Play Project, 105.00",
            "115, adidas X Glider II Soccer Ball, 100.00",
            "116, Active Faith Sports Men's EasyDri Cross Training Shirt, 29.00",
            "117, KomPrexx Mens Sports T-Shirts Short Sleeve Training Tee Shirt Breathable Athletic T-Shirt, 12.00",
            "118, Red Plume Men's Compression Sports Shirt Cool Lightning/Flash Running Long Sleeve Tee/3 Colors, 14.99",
            "119, adidas Men's Training Essential Tech Tee, 19.99",
            "120, adidas Womens Athletics Graphic Muscle Tank, 39.00",
            "121, Mizuno Running Men's Breath Thermo Double Knit Half Zip Tee, 19.00",
            "122, Mizuno Running Men's Venture Tee, 19.99",
            "123, Mizuno Running Men's Discover Tee, 19.95",
            "124, Mizuno Running Men's Breath Thermo Body Map Long Sleeve, 17.90",
            "125, Speedo Men's UPF 50+ Short-Sleeve Rashguard Swim Tee, 18.00",
            "126, Speedo Men's UPF 50+ Easy Long Sleeve Rashguard Swim Tee, 15.99",
            "127, Speedo Men's UPF 50 Short Sleeve Rashguard Swim Tee, 19.00",
            "128, Speedo Male Front Logo Crew Neck Tee, 29.00",
            "129, Speedo Big Girls Colorblock Rashguard, 28.96",
            "130, Garmin Fenix 3 Heart Rate (HR) Silver, 349.99",
            "131, Garmin Forerunner 935 GPS Multisport Watch Ultimate Bundle, 519.99",
            "132, Garmin vívoactive HR GPS Smart Watch Regular fit - Black, 168.50",
            "133, Garmin vívoactive 3 GPS Smartwatch, 249.99",
            "134, Garmin Forerunner 235 - Black/Gray, 264.13"
        };

        static string[] CUSTOMERS = new string[] {
            "1, João da Silva, 1980-10-01, joao.silva@gmail.com",
            "2, Carlos Aguiar, 1977-01-07, carlosag@outlook.com",
            "3, José Leme, 1974-04-12, jose.leme74@gmail.com",
            "4, Luiz da Silva Ramos, 1960-11-01, luizdasilvarms@live.com",
            "5, Adriana Gonçalves Mota, 1990-07-09, adri.mota@outlook.com"
        };


        static void Main(string[] args)
        {



            try
            {
                var task = Run();
                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();

        }

        static async Task Run()
        {

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            var endpoint = config["CosmosDBEndpoint"];
            var key = config["CosmosDBKey"];

            var client = new DocumentClient(new Uri(endpoint), key);

            const string DB_NAME = "MainDB";
            const string ORDERS_COLLECTION_NAME = "OrdersColl";
            const string CUSTOMERS_COLLECTION_NAME = "CustomersColl";

            try
            {
                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DB_NAME));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var database = await client.CreateDatabaseIfNotExistsAsync(new Database() { Id = DB_NAME });
            Console.WriteLine($"Database ${database.Resource.Id} created");

            var ordersColl = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DB_NAME),
                new DocumentCollection() { Id = ORDERS_COLLECTION_NAME }, new RequestOptions { OfferThroughput = 400 });
            Console.WriteLine($"Collection ${ordersColl.Resource.Id} created");

            var customersColl = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DB_NAME),
                new DocumentCollection() { Id = CUSTOMERS_COLLECTION_NAME }, new RequestOptions { OfferThroughput = 400 });
            Console.WriteLine($"Collection ${customersColl.Resource.Id} created");

            var orders = GenerateDummyOrders();

            for (int i = 0; i < orders.Length; i++)
            {
                var document = await client.CreateDocumentAsync(ordersColl.Resource.SelfLink,
                    orders[i], disableAutomaticIdGeneration: true);
                Console.WriteLine($"Order {document.Resource.Id} created.");
            }

            for (int i = 0; i < CUSTOMERS.Length; i++)
            {
                string[] custTemplate = CUSTOMERS[i].Split(',');

                DateTime dob;
                DateTime.TryParseExact(custTemplate[2].Trim(), "yyyy-MM-dd", null, DateTimeStyles.None, out dob);

                CustomerProfile profile = new CustomerProfile()
                {
                    Id = custTemplate[0].Trim(),
                    Name = custTemplate[1].Trim(),
                    DateOfBirth = dob,
                    Email = custTemplate[3].Trim()
                };

                var document = await client.CreateDocumentAsync(customersColl.Resource.SelfLink,
                    profile, disableAutomaticIdGeneration: true);
                Console.WriteLine($"Customer {document.Resource.Id} created.");
            }




        }

        static Order[] GenerateDummyOrders()
        {
            const int TOTAL_ORDERS = 20;

            var orders = new Order[TOTAL_ORDERS];

            for (int i = 0; i < TOTAL_ORDERS; i++)
            {
                System.Threading.Thread.Sleep(100);
                orders[i] = new Order();
                orders[i].Id = (i + 1).ToString();
                orders[i].OrderDate = GetOrderRandomDate();
                orders[i].Items = GetRandomItems();
                orders[i].CustomerId = new Random().Next(1, 5);
            }

            return orders;
        }

        private static OrderItems GetRandomItems()
        {
            int numOfItems = 1;

            OrderItem[] orderItems = new OrderItem[numOfItems];

            for (int i = 0; i < numOfItems; i++)
            {
                string[] itemTemplate = ITEMS[new Random().Next(0, ITEMS.Length - 1)].Split(',');

                double val = Double.Parse(itemTemplate[2].Trim(), CultureInfo.InvariantCulture);
                orderItems[i] = new OrderItem()
                {
                    SKU = itemTemplate[0].Trim(),
                    Description = itemTemplate[1].Trim(),
                    Quantity = 1,
                    Price = new PriceInfo()
                    {
                        Value = val
                    }
                };
            }

            OrderItems items = new OrderItems()
            {
                Items = orderItems

            };

            return items;
        }

        static DateTime GetOrderRandomDate()
        {
            return DateTime.Now.AddDays(new Random().Next(-10, -1));
        }

    }

    class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderItems Items { get; set; }
        public int CustomerId { get; set; }

        public PriceInfo OrderPrice
        {
            get
            {
                return Items.ItemsPrice;
            }
        }
    }

    class OrderItems
    {
        public int TotalItems
        {
            get { return Items.Length; }
        }
        public OrderItem[] Items { get; set; }

        public PriceInfo ItemsPrice
        {
            get
            {
                double price = 0;
                foreach (OrderItem item in Items)
                {
                    price += item.Price.Value;
                }

                PriceInfo priceInfo = new PriceInfo()
                {
                    Value = price,
                };

                return priceInfo;
            }
        }
    }

    class OrderItem
    {
        public string SKU { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public PriceInfo Price { get; set; }
    }

    class PaymentInfo
    {
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Shipping { get; set; }
        public string AppliedDiscount { get; set; }
    }

    class PriceInfo
    {
        public double Value { get; set; }
    }

    class CustomerProfile
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
    }
}
