public class BinomialResult
{
    // American prices - account for early exercise
    public double AmericanCallPrice { get; set; }
    public double AmericanPutPrice { get; set; }

    // European prices - for direct comparison
    public double EuropeanCallPrice { get; set; }
    public double EuropeanPutPrice { get; set; }

    // Early exercise premium - the value of American over European
    public double EarlyExercisePremiumCall { get; set; }
    public double EarlyExercisePremiumPut { get; set; }

    // Tree parameters
    public double UpFactor { get; set; }
    public double DownFactor { get; set; }
    public double RiskNeutralProbability { get; set; }
    public double TimeStepSize { get; set; }

    // First order risk sensitivity
    public double CallDelta { get; set; }
    public double PutDelta { get; set; }
}