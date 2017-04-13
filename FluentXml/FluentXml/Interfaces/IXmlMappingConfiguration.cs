﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Interfaces
{
    public interface IXmlMappingConfiguration
    {
        IList<IXmlElementConfiguration> Configurations { get; }
        string RootElementName { get; }
    }
}
