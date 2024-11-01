using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace StudyProject.CodeBase.DecisionTree
{
    public class ConvolutionalNetwork : SerializedMonoBehaviour
    {
        public RawImage OutputImage;
        public Texture2D InputImage;
        public int Stride = 1;
        public int PoolingStride = 2;
        public RawImage[] RawImages;

        private float[,] _filter1 =
        {
            {0, 0, 0},
            {0, 1, 0},
            {0, 0, 0}
        };

        private float[,] _filter2 =
        {
            {1, 0, -1},
            {0, 0, 0},
            {-1, 0, 1}
        };

        private float[,] _filter3 =
        {
            {0, 1, 0},
            {1, -4, 1},
            {0, 1, 0}
        };

        private float[,] _filter4 =
        {
            {-1, -1, -1},
            {-1, 8, -1},
            {-1, -1, -1}
        };

        private float[,] _filter5 =
        {
            {-0, -1, 0},
            {-1, 8, -1},
            {0, -1, 0}
        };

        private float[,] _filter6 =
        {
            {-0, -1, 0},
            {-1, 5, -1},
            {0, -1, 0}
        };

        private float[,] _filter7 =
        {
            {-1, -1, -1},
            {-1, 5, -1},
            {-1, -1, -1}
        };

        private float[,] _customFilter =
        {
            {1, 1, 1},
            {1, 4, 1},
            {1, 1, 1}
        };

        private float[,] _redFilter =
        {
            {1, 0, -1},
            {1, 0, -1},
            {1, 0, -1}
        };

        float[,] _greenFilter =
        {
            {0, 1, 0},
            {1, -4, 1},
            {0, 1, 0}
        };

        float[,] _blueFilter =
        {
            {-1, -1, -1},
            {0, 0, 0},
            {1, 1, 1}
        };

        private List<FilterContainer> _filters = new();
        [SerializeField] private Texture2D _coloredTexture;
        [SerializeField] private RawImage _coloredRawImage;
        [SerializeField] private RawImage _graduallyFilteredRawImage;

        [Button]
        private void Test()
        {
            _filters = new List<FilterContainer>();
            _filters.Add(new FilterContainer(_filter1, 1, RawImages[0]));
            _filters.Add(new FilterContainer(_filter1, 2, RawImages[1]));
            _filters.Add(new FilterContainer(_filter1, 3, RawImages[2]));

            _filters.Add(new FilterContainer(_filter2, 1, RawImages[3]));
            _filters.Add(new FilterContainer(_filter2, 2, RawImages[4]));
            _filters.Add(new FilterContainer(_filter2, 3, RawImages[5]));

            _filters.Add(new FilterContainer(_filter3, 1, RawImages[6]));
            _filters.Add(new FilterContainer(_filter3, 2, RawImages[7]));
            _filters.Add(new FilterContainer(_filter3, 3, RawImages[8]));

            _filters.Add(new FilterContainer(_filter4, 1, RawImages[9]));
            _filters.Add(new FilterContainer(_filter4, 2, RawImages[10]));
            _filters.Add(new FilterContainer(_filter4, 3, RawImages[11]));

            _filters.Add(new FilterContainer(_filter5, 1, RawImages[12]));

            _filters.Add(new FilterContainer(_filter6, 1, RawImages[13]));

            _filters.Add(new FilterContainer(_filter7, 1, RawImages[14]));
            _filters.Add(new FilterContainer(_customFilter, 1, RawImages[15]));

            foreach (FilterContainer filter in _filters)
            {
                CalculateConvolutional(filter);
            }

            CalculateConvolutionalGradually();
            Color[,] colors = ApplyColorConvolution(GetColorMatrix(_coloredTexture), _redFilter, _blueFilter,
                _greenFilter, 1);
            _coloredRawImage.texture = CreateTextureFromColorMatrix(colors);
            Save();
        }

        private void CalculateConvolutional(FilterContainer filter)
        {
            float[,] image = ConvertImage(InputImage);
            float[,] convResult = ApplyConvolution(image, filter.Filter, filter.Stride);
            float[,] pooledResult = ApplyMaxPooling(convResult, PoolingStride);

            Texture2D texture = ConvertConvolutionResultToTexture(pooledResult);
            filter.RawImage.texture = texture;
        }

        private void CalculateConvolutionalGradually()
        {
            float[,] image = ConvertImage(InputImage);
            float[,] image2 = ApplyConvolution(image, _filter1, 1);
            float[,] image3 = ApplyConvolution(image2, _filter2, 1);
            float[,] convResult = ApplyConvolution(image3, _filter3, 1);

            float[,] pooledResult = ApplyMaxPooling(convResult, PoolingStride);
            Texture2D texture = ConvertConvolutionResultToTexture(pooledResult);
            _graduallyFilteredRawImage.texture = texture;
        }

        public float[,] ApplyConvolution(float[,] matrix, float[,] filter, int stride)
        {
            int filterSize = filter.GetLength(0);
            int outputSize = (matrix.GetLength(0) - filterSize) / stride + 1;
            float[,] output = new float[outputSize, outputSize];

            for (int y = 0; y < outputSize; y++)
            {
                for (int x = 0; x < outputSize; x++)
                {
                    float sum = 0f;
                    for (int fy = 0; fy < filterSize; fy++)
                    {
                        for (int fx = 0; fx < filterSize; fx++)
                        {
                            int imageX = x * stride + fx;
                            int imageY = y * stride + fy;
                            sum += matrix[imageY, imageX] * filter[fy, fx];
                        }
                    }

                    output[y, x] = sum;
                }
            }

            return output;
        }

        public void ApplyReLU(float[,] matrix)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[y, x] = Mathf.Max(0, matrix[y, x]);
                }
            }
        }

        public float[,] ApplyMaxPooling(float[,] matrix, int stride)
        {
            int pooledHeight = matrix.GetLength(0) / stride;
            int pooledWidth = matrix.GetLength(1) / stride;
            float[,] pooled = new float[pooledHeight, pooledWidth];

            for (int y = 0; y < pooledHeight; y++)
            {
                for (int x = 0; x < pooledWidth; x++)
                {
                    float maxVal = float.MinValue;
                    for (int dy = 0; dy < stride; dy++)
                    {
                        for (int dx = 0; dx < stride; dx++)
                        {
                            int matrixX = x * stride + dx;
                            int matrixY = y * stride + dy;
                            maxVal = Mathf.Max(maxVal, matrix[matrixY, matrixX]);
                        }
                    }

                    pooled[y, x] = maxVal;
                }
            }

            return pooled;
        }

        public float[] Flatten(float[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);
            float[] flattened = new float[height * width];
            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    flattened[index++] = matrix[y, x];
                }
            }

            return flattened;
        }

        public float[,] ConvertImage(Texture2D texture)
        {
            int width = texture.width;
            int height = texture.height;
            float[,] matrix = new float[height, width];

            Color[] pixels = texture.GetPixels();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = pixels[y * width + x];
                    float brightness = (pixel.r + pixel.g + pixel.b) / 3f;
                    matrix[y, x] = brightness;
                }
            }

            return matrix;
        }

        private Texture2D ConvertConvolutionResultToTexture(float[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            Texture2D texture = new Texture2D(width, height);

            float minVal = float.MaxValue;
            float maxVal = float.MinValue;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    minVal = Mathf.Min(minVal, matrix[y, x]);
                    maxVal = Mathf.Max(maxVal, matrix[y, x]);
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalizedValue = (matrix[y, x] - minVal) / (maxVal - minVal);
                    Color color = new Color(normalizedValue, normalizedValue, normalizedValue);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        private Color[,] GetColorMatrix(Texture2D image)
        {
            int width = image.width;
            int height = image.height;
            Color[,] matrix = new Color[height, width];

            Color[] pixels = image.GetPixels();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    matrix[y, x] = pixels[y * width + x];
                }
            }

            return matrix;
        }

        private Color[,] ApplyColorConvolution(Color[,] colorMatrix, float[,] redFilter, float[,] greenFilter,
            float[,] blueFilter, int stride)
        {
            int width = (colorMatrix.GetLength(1) - redFilter.GetLength(0)) / stride + 1;
            int height = (colorMatrix.GetLength(0) - redFilter.GetLength(1)) / stride + 1;
            Color[,] output = new Color[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float redSum = 0, greenSum = 0, blueSum = 0;

                    for (int fy = 0; fy < redFilter.GetLength(0); fy++)
                    {
                        for (int fx = 0; fx < redFilter.GetLength(1); fx++)
                        {
                            int imageX = x * stride + fx;
                            int imageY = y * stride + fy;

                            Color pixel = colorMatrix[imageY, imageX];
                            redSum += pixel.r * redFilter[fy, fx];
                            greenSum += pixel.g * greenFilter[fy, fx];
                            blueSum += pixel.b * blueFilter[fy, fx];
                        }
                    }

                    output[y, x] = new Color(Mathf.Clamp01(redSum), Mathf.Clamp01(greenSum), Mathf.Clamp01(blueSum));
                }
            }

            return output;
        }

        private Texture2D CreateTextureFromColorMatrix(Color[,] matrix)
        {
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(0);
            Texture2D texture = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, matrix[y, x]);
                }
            }

            texture.Apply();
            return texture;
        }

        private void Save()
        {
            foreach (RawImage image in RawImages)
            {
                var filepath = Path.Combine(Application.persistentDataPath, image.gameObject.name + ".png");
                byte[] bytes = (image.texture as Texture2D).EncodeToPNG();

                File.WriteAllBytes(filepath, bytes);

                Debug.Log("Filtered texture saved to: " + filepath);
            }
        }
    }

    public class FilterContainer
    {
        public float[,] Filter;
        public int Stride;
        public RawImage RawImage;

        public FilterContainer(float[,] filter, int stride, RawImage rawImage)
        {
            Filter = filter;
            Stride = stride;
            RawImage = rawImage;
        }
    }

    public class FullyConnectedLayer
    {
        private int _inputSize;
        private int _outputSize;
        private float[] _weights;
        private float[] _biases;
        private float[] _outputs;
        private float[] _inputs;
        private System.Random _random = new();

        public FullyConnectedLayer(int inputSize, int outputSize)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;
            _weights = new float[inputSize * outputSize];
            _biases = new float[outputSize];
            _outputs = new float[outputSize];

            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = (float) (_random.NextDouble() * 2 - 1);
            }

            for (int i = 0; i < _biases.Length; i++)
            {
                _biases[i] = (float) (_random.NextDouble() * 2 - 1);
            }
        }

        public float[] FeedForward(float[] input)
        {
            _inputs = input;
            for (int i = 0; i < _outputSize; i++)
            {
                float sum = _biases[i];
                for (int j = 0; j < _inputSize; j++)
                {
                    sum += input[j] * _weights[i * _inputSize + j];
                }

                _outputs[i] = Sigmoid(sum);
            }

            return _outputs;
        }

        public float[] Backpropagate(float[] errors, float learningRate)
        {
            float[] newErrors = new float[_inputSize];

            for (int i = 0; i < _outputSize; i++)
            {
                float delta = errors[i] * SigmoidDerivative(_outputs[i]);
                _biases[i] += delta * learningRate;

                for (int j = 0; j < _inputSize; j++)
                {
                    newErrors[j] += _weights[i * _inputSize + j] * delta;
                    _weights[i * _inputSize + j] += delta * _inputs[j] * learningRate;
                }
            }

            return newErrors;
        }

        private float Sigmoid(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }

        private float SigmoidDerivative(float x)
        {
            return x * (1 - x);
        }
    }
}