using Microsoft.ML.Data;

namespace HonestPlates.Data.Models
{
    // One line of training data, or one review the model will label
    // Came from Microsoft Learn: "Tutorial: Analyze sentiment of website comments with binary classification in ML.NET" (SentimentData class).
    // https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis

    public class SentimentData
    {
        // Column 0 of yelp_labelled.txt: the review sentence
        [LoadColumn(0)]
        public string SentimentText { get; set; }

        // Column 1: 1 = positive, 0 = negative
        // ML.Net will look for a column named "Label" to learn from.
        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment { get; set; }
    }

    // The result the model will output back after reading a review's text
    // Microsoft Learn: SentimentPrediction class
    
    public class SentimentPrediction : SentimentData
    {
        // Model's answer: true = positive, false = negative
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        // How model confirms positive from 0.0 to 1.0
        public float Probability { get; set; }

        // Raw number model uses before turning it into probability
        public float Score { get; set; }
    }
}