using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluent.Xml.Interfaces;
using System.Linq.Expressions;

namespace Fluent.Xml
{
    public class XmlMappingConfiguration<TObject> : Interfaces.IXmlMappingConfiguration<TObject> where TObject : class, new()
    {
        private List<XmlElementConfiguration<TObject>> configurations = new List<XmlElementConfiguration<TObject>>();
        private readonly string rootElementName;

        public IList<XmlElementConfiguration<TObject>> Configurations { get { return configurations; } }
        public string RootElementName { get { return rootElementName; } }
        public XmlMappingConfiguration() : this(null) { }
        public XmlMappingConfiguration(string rootElementName)
        {
            this.rootElementName = rootElementName ?? typeof(TObject).Name;
        }

        protected IXmlElementConfiguration<TObject> HasElement<TPropertyType>(Expression<Func<TObject, TPropertyType>> property)
        {
            string propName;
            if (property.Body is UnaryExpression)
                propName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
            else
                propName = ((MemberExpression)property.Body).Member.Name;

            var xmlElementConfiguration = new XmlElementConfiguration<TObject>(propName);
            configurations.Add(xmlElementConfiguration);
            return xmlElementConfiguration;
        }

    }
}
