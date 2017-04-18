using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Fluent.Xml.Interfaces
{
    public interface IXmlElementConfiguration
    {
        IList<IPropertyConfiguration> Configurations { get; }
        string PropertyName { get; }
        bool IsComplex { get; }
        Type PropertyType { get; }
        
    }
    public interface IXmlElementConfiguration<TObject> : IXmlElementConfiguration where TObject : class, new()
    {
        IXmlElementConfiguration<TObject> WithName(string name);
        IXmlElementConfiguration<TObject> AddAttribute<TPropertyType>(string attributeName, Expression<Func<TObject, TPropertyType>> property);
        IXmlElementConfiguration<TObject> Complex();
    }
}
