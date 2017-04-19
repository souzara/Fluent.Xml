using System;
using Xunit;
using Fluent.Xml;
using System.Collections.Generic;
using System.Linq;

namespace Fluent.Xml.Tests
{
    public class FluentXmlTests
    {
        public FluentXmlTests()
        {
            FluentXml.RegisterMap<FluentMappings.FluentXmlMappingConfiguration>();
            FluentXml.RegisterMap<FluentMappings.MoviesXmlMapping>();
            FluentXml.RegisterMap<FluentMappings.MovieXmlMapping>();
            FluentXml.RegisterMap<FluentMappings.AuthorXmlMapping>();
            FluentXml.RegisterMap<FluentMappings.ReviewXmlMapping>();
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
                Description = "This library is awesome =D",
                Id = 10
            };
            var fluentXml = FluentXml.Serialize(obj);
            Assert.NotNull(fluentXml);
            Assert.Equal(Resources.Xmls.fluentxml_xml, fluentXml, true, true, true);
        }

        [Fact]
        public void ShouldDeserialize_Complex_Xml_into_a_object_based_in_type_definition()
        {
            var obj = FluentXml.Deserialize<Models.MoviesXmlModel>(Resources.Xmls.movies_2016_xml);

            Assert.NotNull(obj);
            Assert.Equal(3, obj.Total);
            Assert.Equal(2016, obj.Year);
            Assert.NotNull(obj.Movies);
            Assert.Equal(3, obj.Movies.Count());
            Assert.NotNull(obj.Author);
            Assert.Equal(obj.Author.Name, "Ricardo Alves");
        }

        [Fact]
        public void Sould_Serialize_Complex_Xml_to_xml_based_in_type_definition()
        {
            var obj = FluentXml.Deserialize<Models.MoviesXmlModel>(Resources.Xmls.movies_2016_xml);

            var xml = FluentXml.Serialize(obj);

            Assert.Equal(Resources.Xmls.movies_2016_xml, xml, true, true, true);
        }

    }

}
