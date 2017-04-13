using Fluent.Xml.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fluent.Xml.Extensions;
using Fluent.Xml.Handlers;
using Fluent.Xml.Configurations;

namespace Fluent.Xml
{
    public static class FluentXml
    {
        #region Fields
        public static IList<IXmlMappingConfiguration> xmlMappingsConfigurations = new List<IXmlMappingConfiguration>();
        #endregion
     
        #region Properties
        public static event ResolveUnconvertibleTypeHandler ResolveUnconvertibleType
        {
            add
            {
                ObjectConvertExtensions.resolveUnconvertibleType += value;
            }
            remove
            {
                ObjectConvertExtensions.resolveUnconvertibleType -= value;
            }
        }
        #endregion

        #region RegisterMap
        public static void RegisterMap<TXmlMappingConfiguration>() where TXmlMappingConfiguration : IXmlMappingConfiguration, new()
        {
            
            xmlMappingsConfigurations.Add(new TXmlMappingConfiguration());
        }
        #endregion
        
        #region Deserialize
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
                    var elementType = resultType.GetProperty(elementConfiguration.PropertyName);

                    var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x is WithNameConfiguration) as WithNameConfiguration ?? new WithNameConfiguration(elementConfiguration.PropertyName);

                    var elementValue = (from el in xmlDocument.Descendants()
                                        where string.Equals(elementName.Name, el.Name.LocalName, StringComparison.OrdinalIgnoreCase)
                                        select el).FirstOrDefault();

                    foreach (AttributeConfiguration attribute in elementConfiguration.Configurations.Where(x => x is AttributeConfiguration))
                    {
                        var attributeType = resultType.GetProperty(attribute.PropertyName);
                        var atttributeValue = (from at in elementValue.Attributes()
                                               where string.Equals(at.Name.LocalName, attribute.AttributeName, StringComparison.OrdinalIgnoreCase)
                                               select at).FirstOrDefault();

                        attributeType.SetValue(result, atttributeValue?.Value.ChangeType(attributeType.PropertyType));


                    }

                    elementType.SetValue(result, elementValue?.Value.ChangeType(elementType.PropertyType));
                }
            }

            return result;
        }
        #endregion
        
        #region Serialize
        public static string Serialize<TObject>(TObject obj) where TObject : class, new()
        {
            var xmlMappingConfiguration = GetXmlMappingConfiguration<TObject>();
            var objType = obj.GetType();

            var decendantsElements = new List<XElement>();

            foreach (var elementConfiguration in xmlMappingConfiguration.Configurations)
            {
                var propertyType = objType.GetProperty(elementConfiguration.PropertyName);
                var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x is WithNameConfiguration) as WithNameConfiguration ?? new WithNameConfiguration(elementConfiguration.PropertyName);
                var value = propertyType.GetValue(obj);
                var element = new XElement(XName.Get(elementName.Name), value);

                foreach (AttributeConfiguration attribute in elementConfiguration.Configurations.Where(x => x is AttributeConfiguration))
                {
                    var property = objType.GetProperty(attribute.PropertyName);
                    var propertyValue = property.GetValue(obj);
                    element.Add(new XAttribute(XName.Get(attribute.AttributeName), propertyValue ?? string.Empty));
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
        #endregion

        #region Commom
        private static IXmlMappingConfiguration GetXmlMappingConfiguration(Type type)
        {
            if (!TypeIsMapped(type))
                throw new InvalidOperationException($"XmlMappingConfiguration not found for type {type.FullName}");
            var xmlMappingConfiguration = xmlMappingsConfigurations.FirstOrDefault(x => x.ObjectType == type);

            return xmlMappingConfiguration;
        }
        private static IXmlMappingConfiguration GetXmlMappingConfiguration<TResult>()
        {
            return GetXmlMappingConfiguration(typeof(TResult));
        }

        private static bool TypeIsMapped(Type type)
        {
            if (type.IsEnumerable())
            {
                if (type.GetGenericArguments().Count() == 0)
                    return false;
                return xmlMappingsConfigurations.Count(x => x.ObjectType == type.GetGenericArgument()) > 0;
            }
            return xmlMappingsConfigurations.Count(x => x.ObjectType == type) > 0;
        }
        #endregion
    }
}
