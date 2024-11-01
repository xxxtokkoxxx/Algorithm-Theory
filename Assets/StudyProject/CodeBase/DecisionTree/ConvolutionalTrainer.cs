using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace StudyProject.CodeBase.DecisionTree
{
    public class ConvolutionalTrainer : SerializedMonoBehaviour
    {
        [SerializeField] ConvolutionalNetwork _network;
        [OdinSerialize] private Dictionary<Texture2D, float[]> _inputs;
        [OdinSerialize] private Dictionary<Texture2D, float[]> _testTextures;
        private FullyConnectedLayer _fullyConnectedLayer;


        [Button]
        public void TrainTest()
        {
            foreach (KeyValuePair<Texture2D, float[]> input in _inputs)
            {
                Train(_network.ConvertImage(input.Key), input.Value, 0.1f);
            }
        }

        private void Prepare(int fcInputSize, int fcOutputSize)
        {
            _fullyConnectedLayer = new FullyConnectedLayer(fcInputSize, fcOutputSize);
        }

        public void Train(float[,] inputImage, float[] target, float learningRate)
        {
            Prepare(_inputs.Count, _inputs.Count);
            float[] outputs = FeedForward(inputImage);
            float[] errors = new float[outputs.Length];

            for (int i = 0; i < outputs.Length; i++)
            {
                errors[i] = target[i] - outputs[i];
            }

            float[] fcErrors = _fullyConnectedLayer.Backpropagate(errors, learningRate);

            int correctPredictions = 0;
            int totalTestImages = _testTextures.Count;

            foreach (KeyValuePair<Texture2D, float[]> testTargets in _testTextures)
            {
                float[] output = FeedForward(_network.ConvertImage(testTargets.Key));

                int predictedClass = Array.IndexOf(output, output.Max());
                int actualClass = Array.IndexOf(testTargets.Value, testTargets.Value.Max());

                if (predictedClass == actualClass)
                {
                    correctPredictions++;
                }
            }

            float accuracy = (float) correctPredictions / totalTestImages * 100;
            Debug.Log(accuracy);
        }

        private float[] FeedForward(float[,] inputImage)
        {
            float[,] filter =
            {
                {0, 0, 0},
                {0, 1, 0},
                {0, 0, 0}
            };

            float[,] convOutput = _network.ApplyConvolution(inputImage, filter, 1);
            float[,] poolOutput = _network.ApplyMaxPooling(convOutput, 2);
            float[] flattened = Flatten(poolOutput);

            return _fullyConnectedLayer.FeedForward(flattened);
        }

        private float[] Flatten(float[,] input)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            float[] flattened = new float[rows * cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flattened[i * cols + j] = input[i, j];
                }
            }

            return flattened;
        }
    }
}