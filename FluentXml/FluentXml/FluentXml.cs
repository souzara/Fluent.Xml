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
            return Deserialize(xml, typeof(TResult)) as TResult;
        }

        public static object Deserialize(string xml, Type type)
        {
            var xmlMappingConfiguration = GetXmlMappingConfiguration(type);

            var result = type.CreateInstance();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var xmlDocument = XDocument.Load(stream);
                DeserializeSimpleProperty(xmlMappingConfiguration, result, xmlDocument);

                DeserializeComplexProperty(xmlMappingConfiguration, result, xmlDocument);
            }

            return result;
        }

        private static void DeserializeComplexProperty(IXmlMappingConfiguration xmlMappingConfiguration, object result, XDocument xmlDocument)
        {
            var resultType = result.GetType();

            foreach (var elementConfiguration in xmlMappingConfiguration.Configurations.Where(x => x.IsComplex))
            {
                var elementType = resultType.GetProperty(elementConfiguration.PropertyName);
                var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x is WithNameConfiguration) as WithNameConfiguration ?? new WithNameConfiguration(elementConfiguration.PropertyName);

                var elementValue = (from el in xmlDocument.Descendants()
                                    where string.Equals(elementName.Name, el.Name.LocalName, StringComparison.OrdinalIgnoreCase)
                                    select el).FirstOrDefault();

                object value;
                if (elementConfiguration.PropertyType.IsEnumerable())
                    value = DeserializeEnumerable(elementValue.ToString(), elementConfiguration.PropertyType.GetGenericArgument(), elementName.Name);
                else
                    value = Deserialize(elementValue.ToString(), elementConfiguration.PropertyType);

                elementType.SetValue(result, value);
            }

        }

        private static void DeserializeSimpleProperty(IXmlMappingConfiguration xmlMappingConfiguration, object result, XDocument xmlDocument)
        {
            var resultType = result.GetType();
            foreach (var elementConfiguration in xmlMappingConfiguration.Configurations.Where(x => !x.IsComplex))
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

        public static IEnumerable<object> DeserializeEnumerable(string xml, Type type, string rootElementName)
        {
            var xmlMappingConfiguration = GetXmlMappingConfiguration(type);
            var result = type.CreateEnumerable();
            var resultType = result.GetType();
            var addMethodInfo = resultType.GetMethod("Add");
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var xmlDocument = XDocument.Load(stream);

                var rootElements = (from el in xmlDocument.Descendants()
                                    where string.Equals(rootElementName, el.Name.LocalName, StringComparison.OrdinalIgnoreCase)
                                    select el);

                foreach (var rootElement in rootElements)
                {
                    var elements = (from el in rootElement.Descendants()
                                    where string.Equals(xmlMappingConfiguration.RootElementName, el.Name.LocalName, StringComparison.OrdinalIgnoreCase)
                                    select el);
                    foreach (var element in elements)
                    {
                        var item = Deserialize(element.ToString(), type);

                        addMethodInfo.Invoke(result, new object[] { item });
                    }
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

            var descendantsElements = new List<XElement>();

            descendantsElements.AddRange(SerializeSimpleProperty(obj, xmlMappingConfiguration, objType));
            descendantsElements.AddRange(SerializeComplexProperty(obj, xmlMappingConfiguration, objType));

            var rootElement = new XElement(XName.Get(xmlMappingConfiguration.RootElementName));
            var xDocument = new XDocument();
            rootElement.Add(descendantsElements);
            xDocument.Add(rootElement);
            using (var stream = new MemoryStream())
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                xDocument.Save(sw, SaveOptions.None);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        private static List<XElement> SerializeSimpleProperty<TObject>(TObject obj, IXmlMappingConfiguration xmlMappingConfiguration, Type objType) where TObject : class, new()
        {
            var descendantsElements = new List<XElement>();
            foreach (var elementConfiguration in xmlMappingConfiguration.Configurations.Where(x => !x.IsComplex))
            {
                var propertyInfo = objType.GetProperty(elementConfiguration.PropertyName);
                var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x is WithNameConfiguration) as WithNameConfiguration ?? new WithNameConfiguration(elementConfiguration.PropertyName);
                var value = propertyInfo.GetValue(obj);
                var element = new XElement(XName.Get(elementName.Name), value);

                foreach (AttributeConfiguration attribute in elementConfiguration.Configurations.Where(x => x is AttributeConfiguration))
                {
                    var property = objType.GetProperty(attribute.PropertyName);
                    var propertyValue = property.GetValue(obj);
                    element.Add(new XAttribute(XName.Get(attribute.AttributeName), propertyValue ?? string.Empty));
                }

                descendantsElements.Add(element);
            }
            return descendantsElements;
        }
        private static List<XElement> SerializeComplexProperty<TObject>(TObject obj, IXmlMappingConfiguration xmlMappingConfiguration, Type objType) where TObject : class, new()
        {
            var descendantsElements = new List<XElement>();
            foreach (var elementConfiguration in xmlMappingConfiguration.Configurations.Where(x => x.IsComplex))
            {
                var propertyInfo = objType.GetProperty(elementConfiguration.PropertyName);
                var elementName = elementConfiguration.Configurations.FirstOrDefault(x => x is WithNameConfiguration) as WithNameConfiguration ?? new WithNameConfiguration(elementConfiguration.PropertyName);
                var value = propertyInfo.GetValue(obj);
                List<XElement> descendants;
                if (propertyInfo.PropertyType.IsEnumerable())
                    descendants = SerializeEnumerableComplexProperty(propertyInfo, elementName, value as IEnumerable<object>);
                else
                    descendants = SerializeSingleComplexProperty(propertyInfo.PropertyType, value);
                descendantsElements.AddRange(descendants);
            }
            return descendantsElements;
        }
        private static List<XElement> SerializeSingleComplexProperty(Type propertyType, object value)
        {
            var descendantsElements = new List<XElement>();
            var xmlMappingConfiguration = GetXmlMappingConfiguration(propertyType);
            var descendant = new List<XElement>();
            foreach (var xmlElementConfiguration in xmlMappingConfiguration.Configurations)
            {
                var property = value.GetType().GetProperty(xmlElementConfiguration.PropertyName);
                var elementName = xmlElementConfiguration.Configurations.FirstOrDefault(x => x is WithNameConfiguration) as WithNameConfiguration ?? new WithNameConfiguration(xmlElementConfiguration.PropertyName);
                var propertyValue = property.GetValue(value);
                descendant.Add(new XElement(XName.Get(elementName.Name), propertyValue));
            }
            descendantsElements.Add(new XElement(XName.Get(xmlMappingConfiguration.RootElementName), descendant));
            return descendantsElements;
        }
        private static List<XElement> SerializeEnumerableComplexProperty(System.Reflection.PropertyInfo propertyInfo, WithNameConfiguration descendantName, IEnumerable<object> values)
        {
            var descendantsElements = new List<XElement>();
            var xmlMappingConfiguration = GetXmlMappingConfiguration(propertyInfo.PropertyType.GetGenericArgument());
            var descendant = new List<XElement>();
            foreach (var value in values)
            {
                var elements = new List<XElement>();
                var objectType = value.GetType();
                elements.AddRange(SerializeSimpleProperty(value, GetXmlMappingConfiguration(objectType), objectType));
                elements.AddRange(SerializeComplexProperty(value, GetXmlMappingConfiguration(objectType), objectType));

                descendant.Add(new XElement(XName.Get(xmlMappingConfiguration.RootElementName), elements));
            }
            descendantsElements.Add(new XElement(XName.Get(descendantName.Name), descendant));

            return descendantsElements;
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
