using Fluent.Xml.Interfaces;

namespace Fluent.Xml.Configurations
{
    internal class WithNameConfiguration : IPropertyConfiguration
    {
        public string Name { get; private set; }
        public WithNameConfiguration(string name)
        {
            Name = name;
        }
    }
}
