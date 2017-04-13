using System;
using Xunit;
using Fluent.Xml;

namespace Fluent.Xml.Tests
{
    public class FluentXmlTests
    {
        public FluentXmlTests()
        {

            FluentXml.RegisterMap<FluentXmlMappingConfiguration>();
        }
        [Fact]
        public void Should_Deserialize_Xml_into_a_object_based_in_type_definition()
        {
            var obj = FluentXml.Deserialize<Models.FluentXmlModel>(Resources.Xmls.fluentxml_xml);

            Assert.NotNull(obj);
            Assert.Equal("Ricardo Alves", obj.Author);
            Assert.Equal("FluentXml", obj.PackageName);
            Assert.Equal("This library is awesome =D", obj.Description);
        }

        [Fact]
        public void Should_Serialize_object_to_xml_based_in_type_definition()
        {
            var obj = new Models.FluentXmlModel
            {
                Author = "Ricardo Alves",
                PackageName = "FluentXml",
                Description = "This library is awesome =D"
            };
            var fluentXml = FluentXml.Serialize(obj);
            Assert.NotNull(fluentXml);
            Assert.True(string.Equals(Resources.Xmls.fluentxml_xml, fluentXml, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class FluentXmlMappingConfiguration : Fluent.Xml.XmlMappingConfiguration<Models.FluentXmlModel>
    {
        public FluentXmlMappingConfiguration() : base("FluentXml")
        {
            //WithName configuration
            HasElement(x => x.Author).WithName("Author");
            //Without WithName, the property name will be used with element name
            HasElement(x => x.PackageName);
            HasElement(x => x.Description).WithName("Description");
        }
    }
}
