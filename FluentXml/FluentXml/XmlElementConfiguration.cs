using System;
using System.Collections.Generic;
using Fluent.Xml.Interfaces;
using System.Linq.Expressions;
using Fluent.Xml.Configurations;

namespace Fluent.Xml
{
    public class XmlElementConfiguration<TObject> : IXmlElementConfiguration<TObject> where TObject : class, new()
    {
        private readonly IList<IPropertyConfiguration> configurations = new List<IPropertyConfiguration>();

        private readonly string propertyName;

        public string PropertyName { get { return propertyName; } }

        public IList<IPropertyConfiguration> Configurations { get { return configurations; } }

        public XmlElementConfiguration(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public IXmlElementConfiguration<TObject> WithName(string name)
        {
            Configurations.Add(new WithNameConfiguration(name));
            return this;
        }

        public IXmlElementConfiguration<TObject> AddAttribute<TPropertyType>(string attributeName, Expression<Func<TObject, TPropertyType>> property)
        {
            string propertyName = GetPropertyName(property);
            Configurations.Add(new AttributeConfiguration(attributeName, propertyName));
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

    }
}