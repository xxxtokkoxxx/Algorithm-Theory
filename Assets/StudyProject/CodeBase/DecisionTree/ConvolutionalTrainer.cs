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
        [SerializeField] private List<Texture2D> _inputs;
        [SerializeField] private List<Texture2D> _testInputs;
        [SerializeField] private float[] _targets;
        private FullyConnectedLayer _fullyConnectedLayer;

        [Button]
        public void TrainTest()
        {
            Prepare(_inputs.Count, 2);
            Train(0.1f);
            Test();
        }

        public void Train(float learningRate)
        {
            for (var i = 0; i < _inputs.Count; i++)
            {
                float[,] inputImage = _network.ConvertImage(_inputs[i]);
                float[] outputs = FeedForward(inputImage);

                float[] target = _targets[i] == 0 ? new float[] { 1, 0 } : new float[] { 0, 1 };
                float[] errors = new float[outputs.Length];

                for (int j = 0; j < outputs.Length; j++)
                {
                    errors[j] = target[j] - outputs[j];
                }

                _fullyConnectedLayer.Backpropagate(errors, learningRate);
            }
        }

        private void Test()
        {
            int correctPredictions = 0;
            int totalTestImages = _testInputs.Count;

            for (var i = 0; i < _testInputs.Count; i++)
            {
                var testTargets = _testInputs[i];
                float[] output = FeedForward(_network.ConvertImage(testTargets));
                float[] target = _targets[i + _inputs.Count] == 0 ? new float[] { 1, 0 } : new float[] { 0, 1 };

                int predictedClass = Array.IndexOf(output, output.Max());
                int actualClass = Array.IndexOf(target, target.Max());

                if (predictedClass == actualClass)
                {
                    correctPredictions++;
                    Debug.Log("correct prediction " + correctPredictions);
                }
            }

            float accuracy = (float) correctPredictions / totalTestImages * 100;
            Debug.Log(accuracy);
        }

        private void Prepare(int fcInputSize, int fcOutputSize)
        {
            _fullyConnectedLayer = new FullyConnectedLayer(fcInputSize, fcOutputSize);
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