/*
This class implements the Cumulative Sum (CUSUM) algorithm for anomaly detection.
The algorithm should determine if an input value is an anomaly or not.
More specifically, the algorithm detects gradual changes, that stack up, and are harder to detect individually.
In the context of this project, the input value is the communication time(request-response) between two machines in an industrial network.
The tolerance and threshold factors are chosen arbitrarily, depending on the specific application.
*/
namespace ICSAnomalyDetection_Console;

public class CUSUMDetectorByDeviation
{
    private double cPlus { get; set; }
    private double cMinus { get; set; }
    private double meanDeviation { get; set; }
    private double stdDevDeviation { get; set; }
    private double tolerance { get; set; }
    private double threshold { get; set; }
    private int windowSize { get; set; }
    private Queue<double> window;

    public CUSUMDetectorByDeviation(List<double> trainingData, int windowSize, double toleranceFactor, double thresholdFactor)
    {
        this.windowSize = windowSize;
        this.window = new Queue<double>();

        var stdDevs = CalculateTrainingDeviations(trainingData);
        meanDeviation = stdDevs.Average();
        stdDevDeviation = CalculateStdDev(stdDevs, meanDeviation);

        tolerance = toleranceFactor * stdDevDeviation;
        threshold = thresholdFactor * stdDevDeviation;

        cPlus = 0;
        cMinus = 0;
    }

    private List<double> CalculateTrainingDeviations(List<double> data)
    {
        var deviations = new List<double>();

        for (int i = 0; i <= data.Count - windowSize; i++)
        {
            var windowSlice = data.Skip(i).Take(windowSize).ToList();
            double mean = windowSlice.Average();
            double std = Math.Sqrt(windowSlice.Sum(x => Math.Pow(x - mean, 2)) / (windowSize - 1));
            deviations.Add(std);
        }

        return deviations;
    }

    private double CalculateStdDev(List<double> values, double mean)
    {
        double sum = values.Sum(x => Math.Pow(x - mean, 2));
        return Math.Sqrt(sum / (values.Count - 1));
    }

    public bool AnomalyDetectionByDeviation(double newValue)
    {
        window.Enqueue(newValue);
        if (window.Count < windowSize)
            return false;

        if (window.Count > windowSize)
            window.Dequeue();

        var list = window.ToList();
        double mean = list.Average();
        double currentStd = Math.Sqrt(list.Sum(x => Math.Pow(x - mean, 2)) / (windowSize - 1));

        cPlus = Math.Max(0, currentStd - (meanDeviation + tolerance) + cPlus);
        cMinus = Math.Min(0, currentStd - (meanDeviation - tolerance) + cMinus);

        return cPlus > threshold || cMinus < -threshold;
    }
}