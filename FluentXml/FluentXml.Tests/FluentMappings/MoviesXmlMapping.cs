﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Tests.FluentMappings
{
    public class MoviesXmlMapping : Fluent.Xml.XmlMappingConfiguration<Models.MoviesXmlModel>
    {
        public MoviesXmlMapping() : base("Movies")
        {
            HasElement(x => x.Total);
            HasElement(x => x.Year);
            HasElement(x => x.Movies).Complex().WithName("MoviesList");
            HasElement(x => x.Author).Complex();
        }
    }

    public class MovieXmlMapping : Fluent.Xml.XmlMappingConfiguration<Models.MovieXmlModel>
    {
        public MovieXmlMapping() : base("Movie")
        {
            HasElement(x => x.Name);
            HasElement(x => x.Description);
        }
    }

    public class AuthorXmlMapping : Fluent.Xml.XmlMappingConfiguration<Models.AuthorXmlModel>
    {
        public AuthorXmlMapping() : base("Author")
        {
            HasElement(x => x.Name);
        }
    }
}