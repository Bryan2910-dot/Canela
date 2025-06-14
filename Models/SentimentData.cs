using Microsoft.ML.Data;

namespace Canela.Models
{
    public class SentimentData
    {
        [LoadColumn(1)] public string? Comment;
        [LoadColumn(7)] public bool IsNegative; // 1 = negativo, 0 = positivo
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsNegative { get; set; }
        public float Probability { get; set; }
    }
}