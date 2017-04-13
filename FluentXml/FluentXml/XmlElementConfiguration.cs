using System;
using System.Collections.Generic;
using Fluent.Xml.Interfaces;
using System.Linq.Expressions;

namespace Fluent.Xml
{
    public class XmlElementConfiguration<TObject> : IXmlElementConfiguration<TObject> where TObject : class, new()
    {
        private readonly Dictionary<string, string> configurations = new Dictionary<string, string>();
        internal readonly Dictionary<string, AttributeConfiguration> attributesConfigurations = new Dictionary<string, AttributeConfiguration>();
        private readonly string propertyName;

        public string PropertyName { get { return propertyName; } }

        public Dictionary<string, string> Configurations { get { return configurations; } }

        public XmlElementConfiguration(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public IXmlElementConfiguration<TObject> WithName(string name)
        {
            Configurations.Add(nameof(WithName), name);
            return this;
        }

        public IXmlElementConfiguration<TObject> AddAttribute<TPropertyType>(string attributeName, Expression<Func<TObject, TPropertyType>> property)
        {
            string propName = GetPropertyName(property);
            attributesConfigurations.Add("attribute", new AttributeConfiguration
            {
                AttributeName = attributeName,
                PropertyName = propName
            });
            return this;
        }

        private static string GetPropertyName<TPropertyType>(Expression<Func<TObject, TPropertyType>> property)
        {
            string propName;
            if (property.Body is UnaryExpression)
                propName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
            else
                propName = ((MemberExpression)property.Body).Member.Name;
            return propName;
        }

        internal struct AttributeConfiguration
        {
            public string AttributeName { get; set; }
            public string PropertyName { get; set; }
        }
    }
}