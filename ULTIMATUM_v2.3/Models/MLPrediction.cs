namespace Ultimatum.Models
{
    /// <summary>
    /// Defines the possible directions a model can predict.
    /// </summary>
    public enum PredictedDirection
    {
        Neutral = 0,
        Bullish = 1,
        Bearish = -1
    }

    /// <summary>
    /// Represents the output of a machine learning model's prediction,
    /// including direction, confidence, and other metadata.
    /// </summary>
    public class MLPrediction
    {
        /// <summary>
        /// The predicted market direction (Bullish, Bearish, or Neutral).
        /// </summary>
        public PredictedDirection Direction { get; set; }

        /// <summary>
        /// The confidence score of the prediction, typically between 0 and 1.
        /// A higher value indicates greater certainty from the model.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Optional: The name of the pattern recognized by the model, if applicable.
        /// </summary>
        public string RecognizedPattern { get; set; }

        /// <summary>
        /// A flag indicating if the model successfully produced a valid prediction.
        /// </summary>
        public bool HasPrediction { get; set; }

        /// <summary>
        /// A static factory method to create a "no prediction" result.
        /// </summary>
        public static MLPrediction NoPrediction()
        {
            return new MLPrediction { HasPrediction = false };
        }
    }
}
