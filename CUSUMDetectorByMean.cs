/*
This class implements the Cumulative Sum (CUSUM) algorithm for anomaly detection.
The algorithm should determine if an input value is an anomaly or not.
More specifically, the algorithm detects gradual changes, that stack up, and are harder to detect individually.
In the context of this project, the input value is the communication time(request-response) between two machines in an industrial network.
The tolerance and threshold factors are chosen arbitrarily, depending on the specific application.
*/

namespace ICSAnomalyDetection_Console;

public class CUSUMDetectorByMean
{
    private double cPlus { get; set; }
    private double cMinus { get; set; }
    private double mean { get; set; }
    private double stdDev { get; set; }
    private double tolerance { get; set; }
    private double threshold { get; set; }

    public CUSUMDetectorByMean(List<double> trainingData, double toleranceFactor, double thresholdFactor)
    {
        UpdateModel(trainingData);
        this.tolerance = toleranceFactor * stdDev;
        this.threshold = thresholdFactor * stdDev;
        this.cPlus = 0;
        this.cMinus = 0;
    }

    public void UpdateModel(List<double> values)
    {
        mean = calculateMean(values);
        stdDev = calculateStdDev(values);
    }

    public double calculateMean(List<double> values)
    {
        return values.Average();
    }

    public double calculateStdDev(List<double> values)
    {
        double sum = values.Sum(d => Math.Pow(d - mean, 2));
        return Math.Sqrt(sum / (values.Count() - 1));
    }
    public bool AnomalyDetectionByMean(double value)
    {
        cPlus = Math.Max(0, value - (mean + tolerance) + cPlus);
        cMinus = Math.Min(0, value - (mean - tolerance) + cMinus);

        if (cPlus > threshold || cMinus < -threshold)
        {
            cPlus = 0;
            cMinus = 0;
            return true;
        }
        
        return false;
    }
}