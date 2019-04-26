using System;
using System.IO;
using System.Collections.Generic;
using Recommender.MachineLearning;
using Recommender.Structures;
using System.Linq;

namespace Recommender
{
    class Program
    {
        static void Main(string[] args)
        {
            Trainer trainer = new Trainer();
            trainer.Train();
            Console.WriteLine("Trained the model successfully!");
            trainer.Evaluate();
            

            // Let's make a prediction.
            // We want to predict how user 5 would rate the movie 4
            var movieRating = new MovieRating()
            {
                userId = 5,
                movieId = 4
            };

            var prediction = trainer.Predict(movieRating);
            Console.WriteLine($"User {movieRating.userId} rates {movieRating.movieId} with {prediction.Score}");

            // But that is not what we want to do, is it?
            // We want to recommend some movies to user 5, or virtually any user.
            
            // To do so, we need to get the maximum predicted rating that user 5 will make.
            

            // Get all the movies: 
            var loaded = new MovieFileLoader(Path.Combine(Environment.CurrentDirectory, "Data\\recommendation-movies.csv"));
            List<Movie> Movies = loaded.Movies;


            // Get the top 5 recommendations
            var top5 = (from m in Movies 
                let p = trainer.Predict(new MovieRating()
                    {
                        userId = 6,
                        movieId = m.Id
                    })
                orderby p.Score descending 
                select (MovieId : m.Id, Score : p.Score)).Take(5);

            // This loop will output the recommended movies for user 6
            foreach (var t in top5)
            {
                Console.WriteLine($"  Score:{t.Score}\tMovie: {Movies.Single(p => p.Id == t.MovieId)?.Title}");
            }

        }
    }
}