using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace StudyProject.CodeBase.DecisionTree
{
    public class DecisionTree : MonoBehaviour
    {
        private void Start()
        {
            SaveTreeIntoDotFile();
        }

        private List<Dictionary<string, object>> _data = new()
        {
            new() {{"Tank", "Full"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Far"}, {"Result", "Yes"}},
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Bad"}, {"Money", 0}, {"Distance", "Short"}, {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Full"}, {"Car conditions", "Bad"}, {"Money", 1000}, {"Distance", "Average"}, {"Result", "No"}
            },
            new()
            {
                {"Tank", "Empty"}, {"Car conditions", "Good"}, {"Money", 100}, {"Distance", "Short"}, {"Result", "Yes"}
            },
            new() {{"Tank", "Full"}, {"Car conditions", "Bad"}, {"Money", 0}, {"Distance", "Far"}, {"Result", "No"}},
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Far"}, {"Result", "No"}
            },
            new() {{"Tank", "Full"}, {"Car conditions", "Bad"}, {"Money", 0}, {"Distance", "Short"}, {"Result", "Yes"}},
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Average"},
                {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Empty"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Short"}, {"Result", "No"}
            },
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Good"}, {"Money", 500}, {"Distance", "Far"},
                {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Bad"}, {"Money", 0}, {"Distance", "Average"},
                {"Result", "No"}
            },
            new()
            {
                {"Tank", "Empty"}, {"Car conditions", "Bad"}, {"Money", 500}, {"Distance", "Average"}, {"Result", "No"}
            },
            new()
            {
                {"Tank", "Empty"}, {"Car conditions", "Good"}, {"Money", 500}, {"Distance", "Average"},
                {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Far"}, {"Result", "No"}
            },
            new()
            {
                {"Tank", "Empty"}, {"Car conditions", "Good"}, {"Money", 1000}, {"Distance", "Far"}, {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Average"},
                {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Empty"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Average"}, {"Result", "No"}
            },
            new() {{"Tank", "Full"}, {"Car conditions", "Bad"}, {"Money", 0}, {"Distance", "Short"}, {"Result", "Yes"}},
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Good"}, {"Money", 0}, {"Distance", "Average"},
                {"Result", "Yes"}
            },
            new()
            {
                {"Tank", "Half Full"}, {"Car conditions", "Bad"}, {"Money", 0}, {"Distance", "Average"},
                {"Result", "No"}
            }
        };

        private List<string> _attributes = new() {"Tank", "Car conditions", "Money", "Distance"};
        private string _target = "Result";

        [Button]
        public void TestLables()
        {
            List<Dictionary<string, object>> data = _data.GetRange(0, 14);
            Dictionary<string, List<string>> labels = new Dictionary<string, List<string>>();

            foreach (var d in data)
            {
                foreach (var k in d)
                {
                    if (!labels.ContainsKey(k.Key))
                    {
                        labels.Add(k.Key, new List<string>());
                    }

                    labels[k.Key].Add(k.Value.ToString());
                }
            }

            foreach (KeyValuePair<string, List<string>> l in labels)
            {
            }

            foreach (KeyValuePair<string, List<string>> l in labels)
            {
            }

            foreach (var a in labels)
            {
                var res = InformationGain(data, a.Key, "Result");
                Debug.Log("Gain " + a.Key + " " + res);
            }
        }

        double CalculateEntropy(List<string> values)
        {
            var counts = values.GroupBy(v => v)
                .Select(g => (double) g.Count() / values.Count)
                .ToList();

            double entropy = 0;
            foreach (var p in counts)
            {
                entropy -= p * math.log2(p);
            }

            return entropy;
        }

        public double Entropy(List<string> labels)
        {
            List<int> labelCounts = labels.GroupBy(x => x)
                .Select(group => group.Count())
                .ToList();

            double total = labels.Count;
            double entropy = 0.0;

            foreach (int count in labelCounts)
            {
                double probability = count / total;
                entropy -= probability * math.log2(probability);
            }

            return entropy;
        }

        public double InformationGain(List<Dictionary<string, object>> data, string attribute, string target)
        {
            double totalEntropy = Entropy(data.Select(d => d[target].ToString()).ToList());
            IEnumerable<string> attributeValues = data.Select(d => d[attribute].ToString()).Distinct();
            double subsetEntropy = 0.0;

            foreach (string value in attributeValues)
            {
                List<Dictionary<string, object>> subset = data.Where(d => d[attribute].ToString() == value).ToList();
                double weight = (double) subset.Count / data.Count;
                subsetEntropy += weight * Entropy(subset.Select(d => d[target].ToString()).ToList());
            }

            return totalEntropy - subsetEntropy;
        }

        public string BestAttribute(List<Dictionary<string, object>> data, List<string> attributes, string target)
        {
            Dictionary<string, double> gains = new Dictionary<string, double>();

            foreach (string attribute in attributes)
            {
                double gain = InformationGain(data, attribute, target);
                gains[attribute] = gain;
            }

            return gains.OrderByDescending(g => g.Value).First().Key;
        }

        public object BuildTree(List<Dictionary<string, object>> data, List<string> attributes, string target)
        {
            List<string> labels = data.Select(d => d[target].ToString()).ToList();

            if (labels.Distinct().Count() == 1)
            {
                return labels[0];
            }

            if (!attributes.Any())
            {
                return labels.GroupBy(x => x)
                    .OrderByDescending(g => g.Count())
                    .First().Key;
            }

            string bestAttribute = BestAttribute(data, attributes, target);
            Dictionary<string, Dictionary<string, object>> tree = new Dictionary<string, Dictionary<string, object>>();

            IEnumerable<string> attributeValues = data.Select(d => d[bestAttribute].ToString()).Distinct();
            tree[bestAttribute] = new Dictionary<string, object>();

            foreach (string value in attributeValues)
            {
                List<Dictionary<string, object>>
                    subset = data.Where(d => d[bestAttribute].ToString() == value).ToList();
                List<string> remainingAttributes = attributes.Where(attr => attr != bestAttribute).ToList();

                object subtree = BuildTree(subset, remainingAttributes, target);
                tree[bestAttribute][value] = subtree;
            }

            return tree;
        }

        public double TestTree(object tree, List<Dictionary<string, object>> testData, string target)
        {
            int correct = 0;

            foreach (Dictionary<string, object> instance in testData)
            {
                string predicted = Classify(instance, tree);

                if (predicted == instance[target].ToString())
                {
                    correct++;
                }
            }

            return (double) correct / testData.Count * 100;
        }

        public string Classify(Dictionary<string, object> target, object tree)
        {
            if (tree is string label)
            {
                return label;
            }

            Dictionary<string, Dictionary<string, object>> decisionTree =
                (Dictionary<string, Dictionary<string, object>>) tree;

            string attribute = decisionTree.Keys.First();
            string attributeValue = target[attribute].ToString();

            if (decisionTree[attribute].ContainsKey(attributeValue))
            {
                return Classify(target, decisionTree[attribute][attributeValue]);
            }

            return "Undetermined";
        }

        [Button]
        public void Main()
        {
            _target = "Result";

            List<Dictionary<string, object>> data = _data.GetRange(0, 14);
            List<Dictionary<string, object>> testData = _data.GetRange(14, _data.Count - 14);

            object decisionTree = BuildTree(data, _attributes, _target);
            double accuracy = TestTree(decisionTree, testData, _target);

            // Debug.Log(accuracy);
            // PrintTree(decisionTree, "");
            foreach (var VARIABLE in _data)
            {
            }
        }

        public void PrintTree(object tree, string indent)
        {
            if (tree is Dictionary<string, Dictionary<string, object>> attributeTree)
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> node in attributeTree)
                {
                    Debug.Log($"{indent}{node.Key}:");
                    foreach (KeyValuePair<string, object> valueNode in node.Value)
                    {
                        Debug.Log($"{indent}  {valueNode.Key} ->");
                        PrintTree(valueNode.Value, indent + "    ");
                    }
                }
            }
            else
            {
                Debug.Log($"{indent}{tree}");
            }
        }

        [Button]
        public void SaveTreeIntoDotFile()
        {
            List<Dictionary<string, object>> data = _data.GetRange(0, 14);

            Dictionary<string, Dictionary<string, object>> tree =
                BuildTree(data, _attributes, _target) as Dictionary<string, Dictionary<string, object>>;


            var path = Path.Combine(Application.persistentDataPath, "fil.dot");
            GenerateDotFile(tree, path);
        }

        public void GenerateDotFile(object tree, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("digraph DecisionTree {");
                writer.WriteLine("node [shape=box];");
                WriteNode(tree, writer, "root", 0);
                writer.WriteLine("}");
            }

            Debug.Log("shoud save " + filePath);
        }

        private void WriteNode(object node, StreamWriter writer, string nodeName, int level)
        {
            if (node is Dictionary<string, Dictionary<string, object>> res)
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> kvp in res)
                {
                    string attribute = kvp.Key;

                    foreach (KeyValuePair<string, object> branch in kvp.Value)
                    {
                        string branchName = branch.Key;
                        string childNode = $"node_{level}_{attribute}_{branchName}";
                        writer.WriteLine($"\"{nodeName}\" -> \"{childNode}\" [label=\"{attribute}: {branchName}\"];");

                        WriteNode(branch.Value, writer, childNode, level + 1);
                    }
                }
            }
            else
            {
                writer.WriteLine($"\"{nodeName}\" [label=\"{node}\", shape=ellipse];");
            }
        }
    }
}