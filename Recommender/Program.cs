using System;
using System.IO;
using Recommender.MachineLearning;

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
            
        }
    }
}