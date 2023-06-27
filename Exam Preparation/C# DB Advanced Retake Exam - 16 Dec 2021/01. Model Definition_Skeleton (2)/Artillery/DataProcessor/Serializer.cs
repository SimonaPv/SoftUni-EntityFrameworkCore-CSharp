
namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            throw new NotImplementedException();
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            throw new NotImplementedException();
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
