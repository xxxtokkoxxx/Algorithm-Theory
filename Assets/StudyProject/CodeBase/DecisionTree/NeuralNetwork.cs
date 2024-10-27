namespace StudyProject.CodeBase.DecisionTree
{
    public class NeturalNetwork_AND
    {
        private float _T = 1.5f;

        private int[] _weights =
        {
            1, 1
        };

        double ActivationFunction(double x)
        {
            return x >= _T ? 1.0 : 0.0;
        }

        public double Predict(int x1, int x2)
        {
            double sum = x1 * _weights[0] + x2 * _weights[1];
            return ActivationFunction(sum);
        }
    }

    public class NeturalNetwork_OR
    {
        private float _T = 0.5f;

        private int[] _weights =
        {
            1, 1
        };

        double ActivationFunction(double x)
        {
            return x >= _T ? 1.0 : 0.0;
        }

        public double Predict(int x1, int x2)
        {
            double sum = x1 * _weights[0] + x2 * _weights[1];
            return ActivationFunction(sum);
        }
    }

    public class NeturalNetwork_NOT
    {
        private float _weight = -1.5f;
        private int _T = -1;

        double ActivationFunction(double x)
        {
            return x >= _T ? 1.0 : 0.0;
        }

        public double Predict(int x)
        {
            double sum = x * _weight;
            return ActivationFunction(sum);
        }
    }

    public class NeturalNetwork_XOR
    {
        double[,] weights1 = new double[2, 2];
        double[] weights2 = new double[2];

        public NeturalNetwork_XOR()
        {
            weights1[0, 0] = 1.0;
            weights1[0, 1] = 1.0;
            weights1[1, 0] = 1.0;
            weights1[1, 1] = 1.0;

            weights2[0] = 1.0;
            weights2[1] = -1.0;
        }

        double ActivationFunction(double x)
        {
            return x >= 0.5 ? 1.0 : 0.0;
        }

        public double Predict(double x1, double x2)
        {
            double hidden1 = ActivationFunction(x1 * weights1[0, 0] + x2 * weights1[0, 1]);
            double hidden2 = ActivationFunction(x1 * weights1[1, 0] + x2 * weights1[1, 1]);

            double output = hidden1 * weights2[0] + hidden2 * weights2[1];

            return ActivationFunction(output);
        }
    }
}