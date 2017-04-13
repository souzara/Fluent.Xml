using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Interfaces
{
    public interface IXmlElementConfiguration
    {
        Dictionary<string, string> Configurations { get; }
        string PropertyName { get; }

        IXmlElementConfiguration WithName(string name);
    }
}
