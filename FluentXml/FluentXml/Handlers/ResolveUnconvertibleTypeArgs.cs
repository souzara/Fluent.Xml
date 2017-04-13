using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Handlers
{
    public class ResolveUnconvertibleTypeArgs : EventArgs
    {
        public object OriginalObject { get; private set; }

        public Type DestinationType { get; private set; }

        public ResolveUnconvertibleTypeArgs(object originalObject, Type destinationType)
        {
            OriginalObject = originalObject;
            DestinationType = destinationType;
        }
    }
}
