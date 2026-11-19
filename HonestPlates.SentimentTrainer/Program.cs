/* Thimmy's Learning Opportunity */

using HonestPlates.Data.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace HonestPlates.SentimentTrainer
{
    // train the review sentiment model once, tests it, then saves to
    // HonestPlates.Data\MLModels\SentimentModel.zip
    // Website only reads from the file, never trains it.

    internal class Program
    {
        static void Main(string[] args)
        {
            string runFolder = AppContext.BaseDirectory;
            string dataPath = Path.Combine(runFolder, "Data", "yelp_labelled.txt");

            // Save into the HonestPlates.Data project so the
            // website and both APIs can use the model.
            // Goes up net8.0 -> Debug -> Bin -> HonestPlates.SentimentTrainer
            // -> solution folder

            string modelFolder = Path.GetFullPath(Path.Combine(runFolder, "..", "..", "..", "..", "HonestPlates.Data", "MLModels"));
            string modelPath = Path.Combine(modelFolder, "SentimentModel.zip");

            // Starting point -> 1 makes 80/20 split the same on every run.
            MLContext mLContext = new MLContext(seed: 1);

            // Load the 1,000 yelp sentences (tab between text and label, no header row)
            IDataView allData = mLContext.Data.LoadFromTextFile<SentimentData>(dataPath, hasHeader: false);

            // 80% to learn, 20% reserved to test with
            DataOperationsCatalog.TrainTestData split = mLContext.Data.TrainTestSplit(allData, testFraction: 0.2);

            // Train on 1 CPU thread so the results are exactly same every run
            SdcaLogisticRegressionBinaryTrainer.Options trainerOptions = 
                new SdcaLogisticRegressionBinaryTrainer.Options();

            trainerOptions.LabelColumnName = "Label";
            trainerOptions.FeatureColumnName = "Features";
            trainerOptions.NumberOfThreads = 1;

            // Steps: turn text into numbers ("Features"), then train a positive/negative classifier on them.
            IEstimator<ITransformer> pipeline = mLContext.Transforms.Text.FeaturizeText(outputColumnName: "Features",
                inputColumnName: "SentimentText").Append(mLContext.BinaryClassification.Trainers.SdcaLogisticRegression(trainerOptions));

            Console.Write("Training on 80% of yelp_labelled.txt...");
            ITransformer model = pipeline.Fit(split.TrainSet);

            // Test: let model guess the 20%, then compare to real labels.
            // Microsoft Learn: Evaluate the model
            IDataView testPredictions = model.Transform(split.TestSet);
            CalibratedBinaryClassificationMetrics metrics = mLContext.BinaryClassification.Evaluate(testPredictions, "Label");

            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");

            // Save the model to a file
            Directory.CreateDirectory(modelFolder);
            mLContext.Model.Save(model, allData.Schema, modelPath);

            Console.WriteLine();
            Console.WriteLine("Model saved to " + modelPath);

            // Test sentiment class on reviews it has never seen
            Console.WriteLine();
            Console.WriteLine("Sample Reviews");
            string[] samples =
            {
                "The food was amazing and the staff are lovely.",
                "Cold fries, obnoxious waiter, never coming back.",
                "The food fine. It's okay.",
                "It took so long for the food to arrive, but food was worth it."
            };

            foreach (string sample in samples)
            {
                Console.WriteLine(Sentiment.Predict(sample) + " | " + sample);
            }

            // Label every review in the database that doesn't have a label yet
            Console.WriteLine();
            int labeled = Review.LabelMissingReviews();
            Console.WriteLine("Labeled " + labeled + " reviews in the database that had no sentiment yet.");

        }
    }
}
