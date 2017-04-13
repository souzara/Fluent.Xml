using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Fluent.Xml.Extensions;

namespace Fluent.Xml.Tests.Extensions
{
    public class ObjectConvertExtensionsTests
    {
        [Fact]
        public void Should_Convert_string_to_int()
        {
            var value = "10";

            var valueConverted = value.ChangeType(typeof(int));

            Assert.NotNull(valueConverted);
            Assert.IsType<int>(valueConverted);
            Assert.Equal(10, valueConverted);
        }

        [Fact]
        public void Should_Convert_string_to_int_with_resolveUnConvertibleTypeEvent()
        {
            FluentXml.ResolveUnconvertibleType += FluentXml_ResolveUnconvertibleType;

            var value = "Dez";

            var valueConverted = value.ChangeType(typeof(int));
            Assert.NotNull(valueConverted);
            Assert.IsType<int>(valueConverted);
            Assert.Equal(10, valueConverted);
            FluentXml.ResolveUnconvertibleType -= FluentXml_ResolveUnconvertibleType;
        }

        private object FluentXml_ResolveUnconvertibleType(Handlers.ResolveUnconvertibleTypeArgs e)
        {
            if (e.OriginalObject.ToString() == "Dez" && e.DestinationType == typeof(int))
                return 10;
            return e.DestinationType.IsValueType ? Activator.CreateInstance(e.DestinationType) : null;
        }
    }
}
