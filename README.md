# Nuget Packges
```
Install-Package Fluent.Xml
```

# Introduction
Library to make easy manipulation of xml files, this version does not support deep deserialization, it will be implemented soon.

# Getting Started - Usage
``` C#
using Fluent.Xml;

namespace FluentXmlSample
{
    class Program
    {
        private static string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                   @"<FluentXml>
                                      <Author>Ricardo Alves</Author>
                                      <PackageName>FluentXml</PackageName>
                                      <Description>This library is awesome = D </Description>
                                    </FluentXml>";
        static Program()
        {
            FluentXml.RegisterMap<FluentXmlMappingConfiguration>();
        }
        static void Main(string[] args)
        {
            //Deserialize
            var obj = FluentXml.Deserialize<FluentXmlModel>(xml);

            //Serialize
          
            var fluentXml = FluentXml.Serialize(obj);
        }
    }


    public class FluentXmlModel
    {
        public string Author { get; set; }
        public string PackageName { get; set; }
        public string Description { get; set; }
    }


    public class FluentXmlMappingConfiguration : Fluent.Xml.XmlMappingConfiguration<FluentXmlModel>
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

```


# Contribute
This version was developed for a simple serialization and deserialization project, contributions and suggestions are welcome.
