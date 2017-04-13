using System;
using System.Collections.Generic;
using Fluent.Xml.Interfaces;

namespace Fluent.Xml
{
    internal class XmlElementConfiguration : IXmlElementConfiguration
    {
        private readonly Dictionary<string, string> configurations = new Dictionary<string, string>();
        private readonly string propertyName;

        public string PropertyName { get { return propertyName; } }

        public Dictionary<string, string> Configurations { get { return configurations; } }

        public XmlElementConfiguration(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public IXmlElementConfiguration WithName(string name)
        {
            Configurations.Add(nameof(WithName), name);
            return this;
        }
    }
}