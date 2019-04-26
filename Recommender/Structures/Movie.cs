using System.Collections.Generic;
using System.IO;

namespace Recommender.Structures
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class MovieFileLoader
    {
        public List<Movie> Movies { get; set; }
        public MovieFileLoader(string filename, char seperatorChar = ',')
        {
            Movies = new List<Movie>();
            using(var reader = new StreamReader(filename))
            {
                reader.ReadLine();
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var split = line.Split(seperatorChar);
                    
                    Movies.Add(new Movie()
                    {
                        Id = int.Parse(split[0]),
                        Title = split[1]
                    });
                }
                reader.Close();
            }
        }
    }
}