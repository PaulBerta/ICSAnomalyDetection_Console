# Introduction

A .NET console app that uses two CUSUM(cumulative sum) methods (mean & deviation) to spot anomalies in three data streams and save comparison plots.

# How It Works

CUSUM by Mean: Tracks small shifts in the process and fires an anomaly when the cumulative sum of the deviations exceeds the threshold.

CUSUM by Deviation: Tracks the process variability and fires an anomaly when the cumulative sum of the deviations of the observed window exceeds the threshold

The program reads training data, initializes the CUSUM detectors (mean & deviation), processes live data, and produces six PNG plots:

- step_1_cusum_mean.png
- step_1_cusum_dev.png
- step_2_cusum_mean.png
- step_2_cusum_dev.png
- step_3_cusum_mean.png
- step_3_cusum_dev.png

# Results

![STEP 3: CUSUM(mean) - C+](./Resources/step_2_cusum_mean.png)
![STEP 2: CUSUM(deviation) - C+](./Resources/step_2_cusum_dev.png)
![STEP 3: CUSUM(mean) - C+](./Resources/step_3_cusum_mean.png)
![STEP 3: CUSUM(deviation) - C-](./Resources/step_3_cusum_dev.png)

# References

[1] Bolboacă, Roland, Béla Genge, and Piroska Haller. "Using Side-Channels to Detect Abnormal Behavior in Industrial Control Systems." In 2019 IEEE 15th International Conference on Intelligent Computer Communication and Processing (ICCP), pp. 435-441. IEEE, 2019.
