using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSAnomalyDetection_Console;
using ScottPlot;

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
        
        var cPlusMean = new List<double>[3];
        var cMinusMean = new List<double>[3];
        var cPlusDev  = new List<double>[3];
        var cMinusDev = new List<double>[3];
        for (int i = 0; i < 3; i++)
        {
            cPlusMean[i]  = new List<double>();
            cMinusMean[i] = new List<double>();
            cPlusDev[i]   = new List<double>();
            cMinusDev[i]  = new List<double>();
        }
        
        for (int i = 0; i < liveStep1.Count; i++)
        {
            double[] values = { liveStep1[i], liveStep2[i], liveStep3[i] };
            
            for (int step = 0; step < 3; step++)
            {
                var m = step == 0 ? cusumDetectorByMean1 :
                        step == 1 ? cusumDetectorByMean2 :
                                    cusumDetectorByMean3;
                var mC = m.AnomalyDetectionByMean(values[step]);
                cPlusMean[step].Add(mC.cPlus);
                cMinusMean[step].Add(mC.cMinus);
                
                var d = step == 0 ? cusumDetectorByDeviation1 :
                        step == 1 ? cusumDetectorByDeviation2 :
                                    cusumDetectorByDeviation3;
                var dC = d.AnomalyDetectionByDeviation(values[step]);
                cPlusDev[step].Add(dC.cPlus);
                cMinusDev[step].Add(dC.cMinus);
            }
        }
        
        void SaveCusumPlot(
            double[] xs, List<double> cPlus, List<double> cMinus,
            string title, string fileName, double threshold)
        {
            var plt = new Plot();
            plt.Title(title);
            plt.XLabel("Index");
            plt.YLabel("C-value");
            plt.Add.ScatterLine(xs, cPlus.ToArray());
            //plt.Add.ScatterLine(xs, cMinus.ToArray());
            plt.Add.ScatterLine(xs ,Enumerable.Repeat(threshold, xs.Length).ToArray());
            //plt.Add.ScatterLine(xs ,Enumerable.Repeat(-threshold, xs.Length).ToArray());
            plt.ShowLegend(Alignment.LowerRight);
            plt.SavePng(fileName, 1000, 400);
            Console.WriteLine($"Saved {fileName}");
        }
        
        double[] indices = Enumerable.Range(0, liveStep1.Count).Select(i => (double)i).ToArray();
        
        for (int step = 0; step < 3; step++)
        {
            double meanThreshold =0, devThreshold = 0;
            switch (step)
            {
                case 0:
                {
                    meanThreshold = cusumDetectorByMean1.threshold;
                    devThreshold = cusumDetectorByDeviation1.threshold;
                    break;
                }
                case 1:
                {
                    meanThreshold = cusumDetectorByMean2.threshold;
                    devThreshold = cusumDetectorByDeviation2.threshold;
                    break;
                }
                case 2:
                {
                    meanThreshold = cusumDetectorByMean3.threshold;
                    devThreshold = cusumDetectorByDeviation3.threshold;
                    break;
                }
            }
            
            string stepName = $"STEP_{step + 1}";
            SaveCusumPlot(
                indices,
                cPlusMean[step],
                cMinusMean[step],
                $"{stepName} – CUSUM by Mean",
                $"{stepName.ToLower()}_cusum_mean.png",
                meanThreshold
            );
            SaveCusumPlot(
                indices,
                cPlusDev[step],
                cMinusDev[step],
                $"{stepName} – CUSUM by Deviation",
                $"{stepName.ToLower()}_cusum_dev.png",
                devThreshold
            );
        }
    }
}
