using System;
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

        private List<FilterContainer> Filters = new();

        private void Start()
        {
            Filters = new List<FilterContainer>();
            Filters.Add(new FilterContainer(_filter1,1, RawImages[0]));
            Filters.Add(new FilterContainer(_filter1,2,RawImages[1]));
            Filters.Add(new FilterContainer(_filter1,3,RawImages[2]));

            Filters.Add(new FilterContainer(_filter2,1,RawImages[3]));
            Filters.Add(new FilterContainer(_filter2,2,RawImages[4]));
            Filters.Add(new FilterContainer(_filter2,3,RawImages[5]));

            Filters.Add(new FilterContainer(_filter3,1,RawImages[6]));
            Filters.Add(new FilterContainer(_filter3,2,RawImages[7]));
            Filters.Add(new FilterContainer(_filter3,3,RawImages[8]));

            Filters.Add(new FilterContainer(_filter4,1,RawImages[9]));
            Filters.Add(new FilterContainer(_filter4,2,RawImages[10]));
            Filters.Add(new FilterContainer(_filter4,3,RawImages[11]));

            Filters.Add(new FilterContainer(_filter5,1,RawImages[12]));

            Filters.Add(new FilterContainer(_filter6,1,RawImages[13]));

            Filters.Add(new FilterContainer(_filter7,1,RawImages[14]));
            Filters.Add(new FilterContainer(_customFilter,1,RawImages[15]));

            foreach (FilterContainer filter in Filters)
            {
                CalculateConvolutional(filter);
            }
        }

        private void CalculateConvolutional(FilterContainer filter)
        {
            // Перетворимо зображення в градації сірого і отримаємо матрицю яскравості
            float[,] image = GetImage(InputImage);
            // Виконаємо перший згортковий шар з фільтром
            float[,] convResult = ApplyConvolution(image, filter.Filter, filter.Stride);

            // Застосовуємо ReLU
            ApplyReLU(convResult);

            // Виконуємо max pooling
            float[,] pooledResult = ApplyMaxPooling(convResult, PoolingStride);

            // Повторити процес згортки, ReLU, pooling за потреби...

            // Згладити результат і подати на FC шар
            float[] flattened = Flatten(pooledResult);
            Texture2D texture = ConvertConvolutionResultToTexture(pooledResult);
            filter.RawImage.texture = texture;
            Save(texture);

            // Тут можна застосувати класифікацію з повністю пов'язаним шаром (нейромережею)
            Debug.Log("Готово до класифікації!");
        }

        private float[,] ApplyConvolution(float[,] matrix, float[,] filter, int stride)
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

        private void ApplyReLU(float[,] matrix)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[y, x] = Mathf.Max(0, matrix[y, x]);
                }
            }
        }

        private float[,] ApplyMaxPooling(float[,] matrix, int stride)
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

        private float[] Flatten(float[,] matrix)
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

        private float[,] GetImage(Texture2D texture)
        {
            int width = texture.width;
            int height = texture.height;
            float[,] matrix = new float[height, width];

            // Отримуємо всі пікселі з текстури
            Color[] pixels = texture.GetPixels();

            // Перетворюємо зображення в градації сірого та записуємо у матрицю
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = pixels[y * width + x];
                    // Обчислюємо яскравість (середнє значення RGB)
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
            // Знайдемо мінімальне і максимальне значення у матриці для нормалізації
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

            // Конвертуємо матрицю в текстуру з нормалізацією
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Нормалізуємо значення до діапазону [0, 1]
                    float normalizedValue = (matrix[y, x] - minVal) / (maxVal - minVal);
                    Color color = new Color(normalizedValue, normalizedValue, normalizedValue); // Градація сірого
                    texture.SetPixel(x, y, color);
                }
            }

            // Застосовуємо зміни до текстури
            texture.Apply();
            return texture;
        }

        public void Save(Texture2D texture)
        {
            foreach (RawImage image in RawImages)
            {
                var filepath = Path.Combine(Application.persistentDataPath, image.gameObject.name + ".png");
                byte[] bytes = texture.EncodeToPNG();

                // Визначаємо шлях для збереження файлу (наприклад, у папку проекту)

                // Записуємо масив байтів у файл
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
}