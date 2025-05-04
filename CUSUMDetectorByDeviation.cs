namespace ICSAnomalyDetection_Console;
public class CUSUMDetectorByDeviation
{
    private double meanDeviation { get; set; }
    private double stdDevDeviation { get; set; }
    private double tolerance { get; set; }
    private double threshold { get; set; }
    private int windowSize { get; set; }
    private Queue<double> window;

    private CValue cValue;

    public CUSUMDetectorByDeviation(List<double> trainingData, int windowSize, double toleranceFactor, double thresholdFactor)
    {
        this.windowSize = windowSize;
        this.window = new Queue<double>();

        var stdDevs = CalculateTrainingDeviations(trainingData);
        meanDeviation = stdDevs.Average();
        stdDevDeviation = CalculateStdDev(stdDevs, meanDeviation);

        tolerance = toleranceFactor * stdDevDeviation;
        threshold = thresholdFactor * stdDevDeviation;
        
        cValue.cPlus = 0;
        cValue.cMinus = 0;
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

    public CValue AnomalyDetectionByDeviation(double newValue)
    {
        window.Enqueue(newValue);
        if (window.Count < windowSize)
        {
            cValue.Anomaly = false;
            return cValue; 
        }
        
        if (window.Count > windowSize)
            window.Dequeue();

        var list = window.ToList();
        double mean = list.Average();
        double currentStd = Math.Sqrt(list.Sum(x => Math.Pow(x - mean, 2)) / (windowSize - 1));

        cValue.cPlus = Math.Max(0, currentStd - (meanDeviation + tolerance) + cValue.cPlus);
        cValue.cMinus = Math.Min(0, currentStd - (meanDeviation - tolerance) + cValue.cMinus);
        
        cValue.Anomaly = cValue.cPlus > threshold || cValue.cMinus < -threshold ? true : false;
        
        return cValue;
    }
}