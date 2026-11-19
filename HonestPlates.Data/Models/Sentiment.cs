using Microsoft.ML;

namespace HonestPlates.Data.Models
{
    // Labels any review text "Positive" or "Negative" using the trained model.

    public class Sentiment
    {
        // Shared by every request. Loads ONCE from the zip.
        private static MLContext MLContext = new MLContext();
        private static ITransformer model;

        // Return Positive, Negative, or Blank if empty text or model cant be used
        public static string Predict (string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }

            try
            {
                if(model == null)
                {
                    string modelPath = Path.Combine(AppContext.BaseDirectory, "MLModels", "SentimentModel.zip");
                    DataViewSchema inputSchema;
                    model = MLContext.Model.Load(modelPath, out inputSchema);
                }

                // Makes one guess at a time: Data goes in, Prediction output
                PredictionEngine<SentimentData, SentimentPrediction> engine = 
                    MLContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

                SentimentData input = new SentimentData();
                input.SentimentText = text;
                SentimentPrediction result = engine.Predict(input);

                if (result.Prediction)
                {
                    return "Positive";
                }
                return "Negative";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
