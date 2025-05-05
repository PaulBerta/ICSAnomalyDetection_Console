namespace ICSAnomalyDetection_Console;

public class CUSUMDetectorByMean
{
    public struct CValue
    {
        public double cPlus;
        public double cMinus;
        public bool Anomaly;
    }
    private double mean { get; set; }
    private double stdDev { get; set; }
    private double tolerance { get; set; }
    public double threshold { get; set; }

    private CValue _cValue;

    public CUSUMDetectorByMean(List<double> trainingData, double toleranceFactor, double thresholdFactor)
    {
        UpdateModel(trainingData);
        this.tolerance = toleranceFactor * stdDev;
        this.threshold = thresholdFactor * stdDev;
        _cValue.cPlus = 0;
        _cValue.cMinus = 0;
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
    public CValue AnomalyDetectionByMean(double value)
    {
        _cValue.cPlus = Math.Max(0, value - (mean + tolerance) + _cValue.cPlus);
        _cValue.cMinus = Math.Min(0, value - (mean - tolerance) + _cValue.cMinus);
        
        var returnCValue = _cValue;
        
        if (_cValue.cPlus > threshold || _cValue.cMinus < -threshold)
        {
            _cValue.cPlus = 0;
            _cValue.cMinus = 0;
            returnCValue.Anomaly = true;
        }
        else
        {
            returnCValue.Anomaly = false;
        }

        return returnCValue;
    }
}