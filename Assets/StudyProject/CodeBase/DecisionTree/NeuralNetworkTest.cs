using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StudyProject.CodeBase.DecisionTree
{
    public class NeuralNetworkTest : MonoBehaviour
    {
        [Button]
        public void Train()
        {
            BackPropagationNeuralNetwork neuralNetwork = new BackPropagationNeuralNetwork();

            double[][] trainingInputs =
            {
                new double[] {1.69, 3.38, 1.40},
                new double[] {3.38, 1.40, 5.56},
                new double[] {1.40, 5.56, 1.86},
                new double[] {5.56, 1.86, 5.62},
                new double[] {1.86, 5.62, 0.46},
                new double[] {5.62, 0.46, 5.51},
                new double[] {0.46, 5.51, 0.26},
                new double[] {5.51, 0.26, 5.13},
                new double[] {0.26, 5.13, 1.18 },
                new double[] {5.13, 1.18, 5.98 },
            };
            double[] trainingOutputs = {5.56, 1.86, 5.62, 0.46, 5.51, 0.26, 5.13, 1.18, 5.98, 1.36};

            double[] weights = {0.1, -0.1, 0.1};
            double bias = 0.1;

            double maxInputValue = 10.0;
            double maxOutputValue = 10.0;

            foreach (double[] input in trainingInputs)
            {
                neuralNetwork.Normalize(input, maxInputValue);
            }

            neuralNetwork.Normalize(trainingOutputs, maxOutputValue);
            neuralNetwork.Train(trainingInputs, trainingOutputs, weights, ref bias, learningRate: 0.01, epochs: 1000000);

            double[][] testInputs =
            {
                new double[] {1.18, 5.98, 1.36},
                new double[] {5.98, 1.36, 5.09 },
            };

            double[] testOutputs = {5.09, 1.29};

            foreach (var input in testInputs)
            {
                neuralNetwork.Normalize(input, maxInputValue);
            }

            double[] predictedValues = new double[testInputs.Length];
            for (int i = 0; i < testInputs.Length; i++)
            {
                double normalizedPrediction = neuralNetwork.Predict(testInputs[i], weights, bias);
                predictedValues[i] = neuralNetwork.Denormalize(normalizedPrediction, maxOutputValue);
                Debug.Log($"Прогноз для тестових даних {i + 1}: {predictedValues[i]}");
            }

            double mse = neuralNetwork.MeanSquaredError(testOutputs, predictedValues);
            Debug.Log($"Середньоквадратична похибка (MSE) для тестових даних: {mse}");
        }
    }


}