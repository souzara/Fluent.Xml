using Fluent.Xml.Interfaces;

namespace Fluent.Xml.Configurations
{
    internal class AttributeConfiguration : IPropertyConfiguration
    {
        public string AttributeName { get; private set; }
        public string PropertyName { get; private set; }
        public AttributeConfiguration(string attributeName, string propertyName)
        {
            AttributeName = attributeName;
            PropertyName = propertyName;
        }
    }
}
