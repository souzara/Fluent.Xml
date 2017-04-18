using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Tests.FluentMappings
{
    public class FluentXmlMappingConfiguration : Fluent.Xml.XmlMappingConfiguration<Models.FluentXmlModel>
    {
        public FluentXmlMappingConfiguration() : base("FluentXml")
        {
            //WithName configuration
            HasElement(x => x.Author).WithName("Author").AddAttribute("Id", x => x.Id);
            //Without WithName, the property name will be used with element name
            HasElement(x => x.PackageName);
            HasElement(x => x.Description).WithName("Description");
        }
    }

}
