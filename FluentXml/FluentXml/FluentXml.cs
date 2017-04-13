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
            var xmlMappingConfiguration = GetXmlMappingConfiguration<TResult>();

            var result = new TResult();
            var resultType = result.GetType();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var xmlDocument = XDocument.Load(stream);
                foreach (var elementConfiguration in xmlMappingConfiguration.Configurations)
                {
                    var propertyType = resultType.GetProperty(elementConfiguration.PropertyName);
                    var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x.Key == "WithName").Value ?? elementConfiguration.PropertyName;

                    var element = (from el in xmlDocument.Descendants()
                                 where string.Equals(elementName, el.Name.LocalName, StringComparison.OrdinalIgnoreCase)
                                 select el).FirstOrDefault();

                    foreach (var attribute in elementConfiguration.attributesConfigurations)
                    {
                        var property = resultType.GetProperty(attribute.Value.PropertyName);
                        var attrValue = (from at in element.Attributes()
                                         where string.Equals(at.Name.LocalName, attribute.Value.AttributeName, StringComparison.OrdinalIgnoreCase)
                                         select at).FirstOrDefault();

                        property.SetValue(result, attrValue?.Value);


                    }

                    propertyType.SetValue(result, element?.Value);
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
                var element = new XElement(XName.Get(elementName), value);
                
                foreach (var attribute in elementConfiguration.attributesConfigurations)
                {
                    var property = objType.GetProperty(attribute.Value.PropertyName);
                    var propertyValue = property.GetValue(obj);
                    element.Add(new XAttribute(XName.Get(attribute.Value.AttributeName), propertyValue ?? string.Empty));
                }

                decendantsElements.Add(element);
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
        private static XmlMappingConfiguration<TResult> GetXmlMappingConfiguration<TResult>() where TResult : class, new()
        {
            var xmlMappingConfiguration = xmlMappingsConfigurations.FirstOrDefault(x => x.GetType().BaseType == typeof(XmlMappingConfiguration<TResult>));
            if (xmlMappingConfiguration == null)
                throw new InvalidOperationException($"XmlMappingConfiguration not found for type {typeof(TResult).FullName}");
            return xmlMappingConfiguration as XmlMappingConfiguration<TResult>;
        }
    }
}
