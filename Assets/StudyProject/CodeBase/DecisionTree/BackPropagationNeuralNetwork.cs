using System;

namespace StudyProject.CodeBase.DecisionTree
{
    public class BackPropagationNeuralNetwork
    {
        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        private double SigmoidDerivative(double x)
        {
            return x * (1.0 - x);
        }

        public double Predict(double[] inputs, double[] weights, double bias)
        {
            double weightedSum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                weightedSum += inputs[i] * weights[i];
            }

            weightedSum += bias;
            return Sigmoid(weightedSum);
        }

        public void Train(double[][] inputs, double[] outputs, double[] weights, ref double bias, double learningRate,
            int epochs)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    double prediction = Predict(inputs[i], weights, bias);

                    double error = outputs[i] - prediction;

                    for (int j = 0; j < weights.Length; j++)
                    {
                        weights[j] += learningRate * error * SigmoidDerivative(prediction) * inputs[i][j];
                    }

                    bias += learningRate * error * SigmoidDerivative(prediction);
                }
            }
        }

        public void Normalize(double[] data, double maxValue)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] /= maxValue;
            }
        }

        public double Denormalize(double value, double maxValue)
        {
            return value * maxValue;
        }
        public double MeanSquaredError(double[] realValues, double[] predictedValues)
        {
            double sum = 0;
            for (int i = 0; i < realValues.Length; i++)
            {
                double error = realValues[i] - predictedValues[i];
                sum += error * error;
            }
            return sum / realValues.Length;
        }
    }
}