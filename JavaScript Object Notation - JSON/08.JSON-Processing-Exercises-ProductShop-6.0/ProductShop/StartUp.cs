using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
            //string result = ImportCategoryProducts(context, inputJson);

            string result = GetUsersWithProducts(context);

            Console.WriteLine(result);
        }

        //--8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold
                            .Count(p => p.Buyer != null),
                        products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            })
                            .ToArray()
                    }
                })
                .OrderByDescending(u => u.soldProducts.count)
                .ToArray();

            var usersWrapper = new
            {
                usersCount = users.Count(),
                users = users
            };

            return JsonConvert.SerializeObject(usersWrapper, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        //--7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = $"{c.CategoriesProducts.Average(x => x.Product.Price):f2}",
                    totalRevenue = $"{c.CategoriesProducts.Sum(x => x.Product.Price):f2}"
                })
                .ToArray();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        //--6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null) && x.ProductsSold.Count() >= 1)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price,
                                buyerFirstName = p.Buyer.FirstName,
                                buyerLastName = p.Buyer.LastName
                            })
                            .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        //--1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            //User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);
            //context.Users.AddRange(users);
            //context.SaveChanges();
            //return $"Successfully imported {validUsers.Count}";

            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var userDtos = JsonConvert.DeserializeObject<ImportUserDTO[]>(inputJson);

            ICollection<User> validUsers = new HashSet<User>();

            foreach (var userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);

                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        //--2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            //Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            //context.Products.AddRange(products);
            //context.SaveChanges();
            //return $"Successfully imported {products.Count}";

            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            Product[] products = mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //--3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var categoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var categoryDto in categoryDtos)
            {
                if (string.IsNullOrEmpty(categoryDto.Name))
                {
                    Category category = mapper.Map<Category>(categoryDto);
                    validCategories.Add(category);
                }
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //--4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var categoryProductDto = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            CategoryProduct[] categoryProducts = mapper.Map<CategoryProduct[]>(categoryProductDto);

            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }

        //--5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}