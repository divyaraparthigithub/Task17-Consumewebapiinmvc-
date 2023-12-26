using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Task17_Consumewebapiinmvc_.Models;
using X.PagedList;

namespace Task17_Consumewebapiinmvc_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ProductListAsync(int? page, string searchString)
        {
            //int pageSize = 5;
            //ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.PriceSortParm = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            //ViewBag.PriceSortParm = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            //var products1 = from p in Product
            //               select p;

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            using (var httpClient = _httpClientFactory.CreateClient("ApiHttpClient"))
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}api/Product/GetProducts");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(json);
                    //ViewBag.CurrentSort = sortOrder;
                    //ViewBag.SearchString = searchString;
                    //ViewBag.CurrentPage = products.Page;
                    //ViewBag.TotalPages = products.TotalPages;

                    //var pagedList = products.ToPagedList(page, pageSize);
                    //pagination
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        products = products.Where(p => p.name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    var pageNumber = page ?? 1;
                    var pageSize = 5;

              
                    var pagedList = products.ToPagedList(pageNumber, pageSize);
                    ViewBag.CurrentSort = ""; 
                    ViewBag.SearchString = searchString;

                    return View(pagedList);
                    //return View(products);
                }
                else
                {
                   
                    return View(new List<Product>());
                }

            }
            
            //public IActionResult ProductList(Product product)
            //{
            //    string Connectionstring = _configuration.GetConnectionString("DefaultConnection");
            //    List<Product> list = new List<Product>();
            //    SqlConnection connection = new SqlConnection(Connectionstring);
            //    using (SqlCommand command = new SqlCommand("sp_list", connection))
            //    {
            //        command.CommandType = CommandType.StoredProcedure;
            //        connection.Open();
            //        SqlDataReader dr = command.ExecuteReader();
            //        while (dr.Read())
            //        {
            //            Product cs = new Product
            //            {
            //                Id = Convert.ToInt32(dr["Id"]),
            //                Name = dr["Name"].ToString(),
            //                Price = Convert.ToInt32(dr["Price"]),
            //                Quantity= Convert.ToInt32(dr["Quantity"])


            //            };
            //            list.Add(cs);
            //        }
            //        connection.Close();
            //    }

            //    return View(list);
            //}
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> CreateAsync(Product product)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            using (var httpClient = _httpClientFactory.CreateClient("ApiHttpClient"))
            {

                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiBaseUrl}api/Product/CreateProduct", product);

                if (response.IsSuccessStatusCode)
                {
                   
                    return RedirectToAction("Index");
                }
                else
                {
                   
                    TempData["ErrorMessage"] = "Failed to insert customer.";
                    return RedirectToAction("Create", product);
                }
            }
        }

        //public IActionResult Create(Product product)
        //{
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("insert into Product values(@Name,@Price,@Quantity)", con);
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Id", customer.Id);
        //        cmd.Parameters.AddWithValue("@Id", product.id);
        //        cmd.Parameters.AddWithValue("@Name", product.name);
        //        cmd.Parameters.AddWithValue("@Price", product.price);
        //        cmd.Parameters.AddWithValue("@Quantity", product.quantity);

        //        cmd.ExecuteNonQuery();
        //    }

        //    return View(product);
        //}
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            //Product product=new Product();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            using (var httpClient = _httpClientFactory.CreateClient("ApiHttpClient"))
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}api/Product/GetProductById?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<Product>(json);
                    return View(product);
                    //return await response.Content.ReadAsAsync<Product>();
                   
                }
                else
                {

                }
               
            }

           return View();

        }
        //public ActionResult Edit(int id)
        //{
        //    Product list = new Product();
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("select * from Product where id=@id", con);
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        using (SqlDataReader dr = cmd.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                list.name = dr["Name"].ToString();
        //                list.price = Convert.ToInt32(dr["Price"]);
        //                list.quantity = Convert.ToInt32(dr["Quantity"]);

        //            }

        //        }
        //    }

        //    return View(list);


        //}


        [HttpPost]
        public async Task<IActionResult> EditAsync(int id,Product product)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            using (var httpClient = _httpClientFactory.CreateClient("ApiHttpClient"))
            {

                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiBaseUrl}api/Product/UpdateProduct", product);

                if (response.IsSuccessStatusCode)
                {
                    // Customer inserted successfully
                    return RedirectToAction("Index");
                }
                else
                {
                    
                    TempData["ErrorMessage"] = "Failed to update customer.";
                    return RedirectToAction("Edit", product);
                }
            }
        }

        //public IActionResult Edit(int id,Product product)
        //{
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();

        //        SqlCommand cmd = new SqlCommand("update Product set name=@name,price=@price,quantity=@quantity where id=@id", con);

        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        cmd.Parameters.AddWithValue("@Name", product.name);
        //        cmd.Parameters.AddWithValue("@Price", product.price);
        //        cmd.Parameters.AddWithValue("@Quantity", product.quantity);
        //        cmd.ExecuteNonQuery();
        //    }

        //    return View(product);
        //}
        [HttpGet]
        public IActionResult Delete()
        {
            Product product=new Product();
            return View(product);
        }
        [HttpPost]


        public async Task<IActionResult> Delete(int id)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            using (var httpClient = _httpClientFactory.CreateClient("ApiHttpClient"))
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{apiBaseUrl}api/Product/DeleteProduct?id="+id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle error
                   // var errorResponse = await response.Content.ReadAsStringAsync();
                   //// var errorMessage = JsonConvert.DeserializeObject<string>(errorResponse);
                   // ModelState.AddModelError(string.Empty, errorMessage); // Add error message to the model state
                    return View(); // Return the view to show the error
                }
            }
        }
        //public ActionResult Delete(int id,Product product)
        //{
        //    //Product product = new Product();
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("delete from Product where id=@id", con);

        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        //cmd.Parameters.AddWithValue("@Name", product.name);
        //        //cmd.Parameters.AddWithValue("@Price", product.price);
        //        //cmd.Parameters.AddWithValue("@Quantity", product.quantity);
        //        cmd.ExecuteNonQuery();
        //    }
        //    return View("ProductList",product);

        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}