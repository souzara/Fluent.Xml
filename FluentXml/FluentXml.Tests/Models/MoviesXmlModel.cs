using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Tests.Models
{
    public class MoviesXmlModel
    {
        public int Total { get; set; }
        public int Year { get; set; }
        public AuthorXmlModel Author { get; set; }
        public MoviesList MoviesList { get; set; }

        public MoviesXmlModel()
        {
            
        }
    }

    public class MoviesList
    {
        public IEnumerable<MovieXmlModel> Movie { get; set; }
    }

    public class MovieXmlModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Reviews Reviews { get; set; }
    }

    public class AuthorXmlModel
    {
        public string Name { get; set; }
        public IEnumerable<MovieXmlModel> Movies { get; set; }
    }
    public class Reviews
    {
        public IEnumerable<Review> Review { get; set; }
    }
    public class Review
    {
        public string Name { get; set; }
        public short Stars { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
