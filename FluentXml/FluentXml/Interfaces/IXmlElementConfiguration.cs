using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Interfaces
{
    public interface IXmlElementConfiguration<TObject> where TObject : class , new()
    {
        Dictionary<string, string> Configurations { get; }
        string PropertyName { get; }
        IXmlElementConfiguration<TObject> WithName(string name);
        IXmlElementConfiguration<TObject> AddAttribute<TPropertyType>(string attributeName, Expression<Func<TObject, TPropertyType>> property);
    }
}
