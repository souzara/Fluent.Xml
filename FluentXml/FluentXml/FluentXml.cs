using Fluent.Xml.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fluent.Xml
{
    public static class FluentXml
    {
        public static IList<IXmlMappingConfiguration> xmlMappingsConfigurations = new List<IXmlMappingConfiguration>();
        public static void RegisterMap<TXmlMappingConfiguration>() where TXmlMappingConfiguration : IXmlMappingConfiguration, new()
        {
            xmlMappingsConfigurations.Add(new TXmlMappingConfiguration());
        }

        public static TResult Deserialize<TResult>(string xml) where TResult : class, new()
        {
            IXmlMappingConfiguration xmlMappingConfiguration = GetXmlMappingConfiguration<TResult>();

            var result = new TResult();
            var resultType = result.GetType();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var xmlDocument = XDocument.Load(stream);
                foreach (var elementConfiguration in xmlMappingConfiguration.Configurations)
                {
                    var propertyType = resultType.GetProperty(elementConfiguration.PropertyName);
                    var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x.Key == "WithName").Value ?? elementConfiguration.PropertyName;

                    var value = (from el in xmlDocument.Descendants()
                                 where string.Equals(elementName, el.Name.LocalName, StringComparison.OrdinalIgnoreCase)
                                 select el).FirstOrDefault()?.Value;

                    propertyType.SetValue(result, value);
                }
            }

            return result;
        }



        public static string Serialize<TObject>(TObject obj) where TObject : class, new()
        {
            var xmlMappingConfiguration = GetXmlMappingConfiguration<TObject>();
            var objType = obj.GetType();

            var decendantsElements = new List<XElement>();

            foreach (var elementConfiguration in xmlMappingConfiguration.Configurations)
            {
                var propertyType = objType.GetProperty(elementConfiguration.PropertyName);
                var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x.Key == "WithName").Value ?? elementConfiguration.PropertyName;
                var value = propertyType.GetValue(obj);
                decendantsElements.Add(new XElement(XName.Get(elementName), value));
            }

            var rootElement = new XElement(XName.Get(xmlMappingConfiguration.RootElementName));
            var xDocument = new XDocument();
            rootElement.Add(decendantsElements);
            xDocument.Add(rootElement);
            using (var stream = new MemoryStream())
            {
                xDocument.Save(stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        private static IXmlMappingConfiguration GetXmlMappingConfiguration<TResult>() where TResult : class, new()
        {
            var xmlMappingConfiguration = xmlMappingsConfigurations.FirstOrDefault(x => x.GetType().BaseType == typeof(XmlMappingConfiguration<TResult>));
            if (xmlMappingConfiguration == null)
                throw new InvalidOperationException($"XmlMappingConfiguration not found for type {typeof(TResult).FullName}");
            return xmlMappingConfiguration;
        }
    }
}
