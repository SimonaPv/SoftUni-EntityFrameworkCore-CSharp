namespace Trucks.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchers = context.Despatchers
                .Where(d => d.Trucks.Any())
                .Select(d => new ExportDespatcherDto()
                {
                    TrucksCount = d.Trucks.Count(),
                    DespatcherName = d.Name,
                    Trucks = d.Trucks.Select(t => new ExpTruckDto()
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString()
                    })
                    .OrderBy(t => t.RegistrationNumber)
                    .ToArray()
                })
                .OrderByDescending(t => t.Trucks.Count())
                .ThenBy(t => t.DespatcherName)
                .ToArray();

            return Serialize<ExportDespatcherDto[]>(despatchers, "Despatchers");
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Where(c => c.ClientsTrucks.Any(x => x.Truck.TankCapacity >= capacity))
                .ToArray()
                .Select(c => new
                {
                    Name = c.Name,
                    Trucks = c.ClientsTrucks
                             .Where(x => x.Truck.TankCapacity >=   capacity)
                             .Select(t => new
                             {
                                 TruckRegistrationNumber = t.Truck.RegistrationNumber,
                                 VinNumber = t.Truck.VinNumber,
                                 TankCapacity = t.Truck.TankCapacity,
                                 CargoCapacity = t.Truck.CargoCapacity,
                                 CategoryType = t.Truck.CategoryType.ToString(),
                                 MakeType = t.Truck.MakeType.ToString(),
                             })
                             .OrderBy(t => t.MakeType)
                             .ThenByDescending(t => t.CargoCapacity)
                             .ToArray()
                })
                .OrderByDescending(c => c.Trucks.Count())
                .ThenBy(c => c.Name)
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }

        private static string Serialize<T>(T dataTransferObjects, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            StringBuilder sb = new StringBuilder();
            using var writer = new StringWriter(sb);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, dataTransferObjects, xmlNamespaces);

            return sb.ToString();
        }
    }
}
