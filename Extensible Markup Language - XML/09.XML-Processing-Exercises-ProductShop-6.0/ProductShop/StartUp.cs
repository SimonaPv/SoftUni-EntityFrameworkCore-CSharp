using AutoMapper;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            string inputXml = File.ReadAllText(@"../../../Datasets/users.xml");
            string result = ImportUsers(context, inputXml);

            Console.WriteLine(result);
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var productDtos = Deserialize<ImportProductDto[]>(inputXml, "Products");

            Product[] products = productDtos
                .Select(p => new Product()
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerId = p.BuyerId == 0 ? null : p.BuyerId,
                    SellerId = p.SellerId
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var userDtos = Deserialize<ImportUserDto[]>(inputXml, "Users");
            var users = mapper.Map<User[]>(userDtos);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);

            using StringReader reader = new StringReader(inputXml);

            T dtos = (T)serializer.Deserialize(reader);
            return dtos;
        }
    }
}