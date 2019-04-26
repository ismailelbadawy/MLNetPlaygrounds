

using System;
using System.IO;
using Microsoft.ML;
using Microsoft.Data.DataView;
using Recommender.Structures;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.Recommender;

namespace Recommender.MachineLearning
{
    public class Trainer
    {
        private string trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data\\recommendation-ratings-train.csv");
        private string testDataPath = Path.Combine(Environment.CurrentDirectory, "Data\\recommendation-ratings-test.csv");
        
        private MLContext MLContext { get; set; }

        private IDataView TrainingDataView { get; set; }
        private IDataView TestDataView { get; set; }

        private TransformerChain<MatrixFactorizationPredictionTransformer> Model { get; set;}

        private PredictionEngine<MovieRating, MovieRatingPrediction> PredictionEngine { get; set; }

        /// <summary>
        /// Creates the trainer, loads the data from the files.
        /// </summary>
        public Trainer()
        {
            var _mlContext = new MLContext();
            TrainingDataView = _mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader : true, separatorChar: ',');
            TestDataView = _mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader : true, separatorChar : ',');

            MLContext = _mlContext;
        }

        /// <summary>
        /// Trains the model on the loaded data set.
        /// </summary>
        public void Train()
        {
            /// Here we use matrix factorization trainer
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "movieIdEncoded",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var pipeline = MLContext.Transforms.Conversion.MapValueToKey(
                inputColumnName : "userId",
                outputColumnName : "userIdEncoded"
            ).Append(MLContext.Transforms.Conversion.MapValueToKey(
                inputColumnName : "movieId",
                outputColumnName : "movieIdEncoded"
            )).Append(MLContext.Recommendation().Trainers.MatrixFactorization(options));


            // Train the model.
            Model = pipeline.Fit(TrainingDataView);
            
            PredictionEngine = Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(MLContext);
            
        }

        public void Evaluate()
        {
            var predictions = Model.Transform(TestDataView);
            var metrics = MLContext.Regression.Evaluate(predictions, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);
            Console.WriteLine($"RMSE : {metrics.Rms}");
            Console.WriteLine($"L1 : {metrics.L1}");
            Console.WriteLine($"L2 : {metrics.L2}");
            
        }

        public MovieRatingPrediction Predict(MovieRating rating)
        {
            var prediction = PredictionEngine.Predict(rating);
            return prediction;
        }
    }
}