using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Interfaces
{
    public interface IXmlMappingConfiguration
    {

    }
    public interface IXmlMappingConfiguration<TObject> : IXmlMappingConfiguration where TObject : class, new()
    {
        IList<XmlElementConfiguration<TObject>> Configurations { get; }
        string RootElementName { get; }
    }
}
