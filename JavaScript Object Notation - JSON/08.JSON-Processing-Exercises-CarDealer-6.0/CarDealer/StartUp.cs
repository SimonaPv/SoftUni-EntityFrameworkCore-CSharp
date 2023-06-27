using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using System.Globalization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //string inputJson = File.ReadAllText(@"../../../Datasets/cars-and-parts.json");
            //string result = ImportSales(context, inputJson);
            //Console.WriteLine(result);

            string jsonOutput = GetSalesWithAppliedDiscount(context);
            string outputFilePath = @"../../../Results/sales-discounts.json";

            File.WriteAllText(outputFilePath, jsonOutput);
        }

        //--19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesWithDiscount = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartsCars.Sum(p => p.Part.Price):f2}",
                    priceWithDiscount = $"{s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - s.Discount / 100):f2}"
                })
                .ToArray();

            return JsonConvert.SerializeObject(salesWithDiscount, Formatting.Indented);
        }

        //--18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customerSales = context.Customers
               .Where(c => c.Sales.Any())
               .Select(c => new
               {
                   fullName = c.Name,
                   boughtCars = c.Sales.Count(),
                   salePrices = c.Sales.SelectMany(x => x.Car.PartsCars.Select(x => x.Part.Price))
               })
               .ToArray();

            var totalSalesByCustomer = customerSales.Select(t => new
            {
                t.fullName,
                t.boughtCars,
                spentMoney = t.salePrices.Sum()
            })
            .OrderByDescending(t => t.spentMoney)
            .ThenByDescending(t => t.boughtCars)
            .ToArray();

            return JsonConvert.SerializeObject(totalSalesByCustomer, Formatting.Indented);
        }

        //--17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsAndParts = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                        .Select(p => new
                        {
                            Name = p.Part.Name,
                            Price = $"{p.Part.Price:f2}"
                        })
                })
                .ToArray();

            return JsonConvert.SerializeObject(carsAndParts, Formatting.Indented);
        }

        //--16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
               .Where(s => !s.IsImporter)
               .Select(s => new
               {
                   Id = s.Id,
                   Name = s.Name,
                   PartsCount = s.Parts.Count()
               })
               .ToArray();

            return JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
        }

        //--15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsFromMakeToyota = context.Cars
               .Where(c => c.Make == "Toyota")
               .OrderBy(c => c.Model)
               .ThenByDescending(c => c.TraveledDistance)
               .Select(c => new
               {
                   Id = c.Id,
                   Make = c.Make,
                   Model = c.Model,
                   TraveledDistance = c.TraveledDistance
               })
               .ToArray();

            return JsonConvert.SerializeObject(carsFromMakeToyota, Formatting.Indented);
        }

        //--14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //--13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            var saleDtos = JsonConvert.DeserializeObject<ImportSalesDto[]>(inputJson);
            var sales = mapper.Map<Sale[]>(saleDtos);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        //--12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            var customersDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);
            var customers = mapper.Map<Customer[]>(customersDtos);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        //--11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            var carDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            ICollection<Car> cars = new HashSet<Car>();
            ICollection<PartCar> parts = new HashSet<PartCar>();

            foreach (var carDto in carDtos)
            {
                Car car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TravelledDistance
                };
                cars.Add(car);

                foreach (var partId in carDto.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    };
                    parts.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //--10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            var partsDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<Part> parts = new HashSet<Part>();

            foreach (var partsDto in partsDtos)
            {
                if (!context.Suppliers.Any(x => x.Id == partsDto.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partsDto);
                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        //--9
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            var suppliersDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);
            var suppliers = mapper.Map<Supplier[]>(suppliersDtos);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }
    }
}