using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSAnomalyDetection_Console;

class Program
{
    static void Main(string[] args)
    {
        var trainingLines = File.ReadAllLines("/Users/paulberta/Downloads/step_data.csv")
                                .Skip(1)
                                .Select(line => line.Split(','))
                                .ToList();

        var trainStep1 = new List<double>();
        var trainStep2 = new List<double>();
        var trainStep3 = new List<double>();

        foreach (var parts in trainingLines)
        {
            trainStep1.Add(double.Parse(parts[0]));
            trainStep2.Add(double.Parse(parts[1]));
            trainStep3.Add(double.Parse(parts[2]));
        }
        
        double toleranceFactor = 0.5;
        double thresholdFactor = 4.0;
        int deviationWindowSize = 5;
        double deviationToleranceFactor = 0.5;
        double deviationThresholdFactor = 4.0;
        
        var cusumDetectorByMean1 = new CUSUMDetectorByMean(trainStep1, toleranceFactor, thresholdFactor);
        var cusumDetectorByMean2 = new CUSUMDetectorByMean(trainStep2, toleranceFactor, thresholdFactor);
        var cusumDetectorByMean3 = new CUSUMDetectorByMean(trainStep3, toleranceFactor, thresholdFactor);

        var cusumDetectorByDeviation1 = new CUSUMDetectorByDeviation(trainStep1, deviationWindowSize, deviationToleranceFactor, deviationThresholdFactor);
        var cusumDetectorByDeviation2 = new CUSUMDetectorByDeviation(trainStep2, deviationWindowSize, deviationToleranceFactor, deviationThresholdFactor);
        var cusumDetectorByDeviation3 = new CUSUMDetectorByDeviation(trainStep3, deviationWindowSize, deviationToleranceFactor, deviationThresholdFactor);
        
        var attackLines = File.ReadAllLines("/Users/paulberta/Downloads/attack.csv")
                              .Skip(1)
                              .Select(line => line.Split(','))
                              .ToList();

        var liveStep1 = attackLines.Select(p => double.Parse(p[0])).ToList();
        var liveStep2 = attackLines.Select(p => double.Parse(p[1])).ToList();
        var liveStep3 = attackLines.Select(p => double.Parse(p[2])).ToList();
        
        int count = liveStep1.Count;
        for (int i = 0; i < count; i++)
        {
            double v1 = liveStep1[i], v2 = liveStep2[i], v3 = liveStep3[i];
            
            if (cusumDetectorByMean1.AnomalyDetectionByMean(v1))
                Console.WriteLine($"CUSUMDetectorByMean: [Index {i}] STEP_1 anomaly: {v1} ms");
            if (cusumDetectorByMean2.AnomalyDetectionByMean(v2))
                Console.WriteLine($"CUSUMDetectorByMean: [Index {i}] STEP_2 anomaly: {v2} ms");
            if (cusumDetectorByMean3.AnomalyDetectionByMean(v3))
                Console.WriteLine($"CUSUMDetectorByMean: [Index {i}] STEP_3 anomaly: {v3} ms");
            
            if (cusumDetectorByDeviation1.AnomalyDetectionByDeviation(v1))
                Console.WriteLine($"CUSUMDetectorByDeviation: [Index {i}] STEP_1 anomaly: {v1} ms");
            if (cusumDetectorByDeviation2.AnomalyDetectionByDeviation(v2))
                Console.WriteLine($"CUSUMDetectorByDeviation: [Index {i}] STEP_2 anomaly: {v2} ms");
            if (cusumDetectorByDeviation3.AnomalyDetectionByDeviation(v3))
                Console.WriteLine($"CUSUMDetectorByDeviation: [Index {i}] STEP_3 anomaly: {v3} ms");
        }
    }
}
