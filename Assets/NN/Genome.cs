using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NN
{
    public class Genome
    {
        private Dictionary<string, Connection> _genes = new Dictionary<string, Connection>();
        private List<Node> _nodes = new List<Node>();
        private List<Node> _inputs = new List<Node>();
        private List<Node> _outputs = new List<Node>();

        private Node _biasNode = null;

        private void PopulateGenes()
        {
            int id = 0;

            // Inputs
            var inputAltitude = new Node(id++, 0);
            _nodes.Add(inputAltitude);
            _inputs.Add(inputAltitude);

            var inputDistance = new Node(id++, 0);
            _nodes.Add(inputDistance);
            _inputs.Add(inputDistance);

            var inputObsAltitude = new Node(id++, 0);
            _nodes.Add(inputObsAltitude);
            _inputs.Add(inputObsAltitude);

            // Outputs
            var output = new Node(id++, 1);
            _nodes.Add(output);
            _outputs.Add(output);

            // Bias
            _biasNode = new Node(id++, 0);
            _nodes.Add(_biasNode);

            foreach (var inp in _inputs)
            {
                foreach (var outp in _outputs)
                {
                    _genes.Add(Connection.Hash(inp, outp), new Connection(inp, outp, UnityEngine.Random.Range(-1f, 1f)));
                }
            }

            _genes.Add(Connection.Hash(_biasNode, _outputs[0]), new Connection(_biasNode, _outputs[0], UnityEngine.Random.Range(-1f, 1f)));
        }

        public Genome()
        {
            PopulateGenes();
            //Debug.Log(ToString());
            //Debug.Log(_outputs[0].ID);
        }

        public override string ToString()
        {
            string s = "digraph g {\n";
            foreach (var connection in _genes.Values)
            {
                s += connection.ToString();
            }
            s += "}";
            return s;
        }

        public List<float> FeedForward(List<float> inputValues)
        {
            //set the outputs of the input this.nodes
            for (var i = 0; i < _inputs.Count; i++)
            {
                _inputs[i].outputValue = inputValues[i];
            }
            _biasNode.outputValue = 1; //output of bias is 1

            for (var i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Engage();
            }

            var outs = new List<float>();
            for (var i = 0; i < _outputs.Count; i++)
            {
                outs.Add(_outputs[i].outputValue);
            }

            for (var i = 0; i < _nodes.Count; i++)
            { //reset all the this.nodes for the next feed forward
                _nodes[i].inputSum = 0;
            }

            return outs;
        }

        public Connection GetConnection(string hash)
        {
            return _genes[hash];
        }

        public void CloneFrom(Genome genome)
        {
            Debug.Assert(genome != null);
            foreach (var connection in _genes.Keys)
            {
                _genes[connection].weight = genome.GetConnection(connection).weight;
            }
        }

        public void Crossover(Genome genome1, Genome genome2)
        {
            if (genome1 == null || genome2 == null)
                return;
            foreach (var connection in _genes.Keys)
            {
                float rand = UnityEngine.Random.value;
                if (rand < 0.5f)
                {
                    _genes[connection].weight = genome1._genes[connection].weight;
                }
                else
                {
                    _genes[connection].weight = genome2._genes[connection].weight;
                }
            }
        }

        public void Mutate()
        {
            foreach (var gene in _genes.Values)
            {
                gene.MutateWeight();
            }
        }
    }

    public class Node
    {
        public int ID = -1;
        public int layer = 0;
        public float outputValue = 0;
        public float inputSum = 0;
        public List<Connection> outputConnections = new List<Connection>();

        public Node(int i, int layer)
        {
            ID = i;
            this.layer = layer;
        }

        public void Engage()
        {
            if (layer != 0)
            {
                outputValue = sigmoid(inputSum);
            }

            foreach (var connections in outputConnections)
            {
                connections.to.inputSum += outputValue * connections.weight;
            }
        }

        public static float sigmoid(float x)
        {
            return 1.0f / (1.0f + Mathf.Pow(Mathf.Exp(1), -4.9f * x)); // TODO magic number
        }
    }

    public class Connection
    {
        public Node from;
        public Node to;
        public float weight;

        public override string ToString()
        {
            return $"n{from.ID} -> n{to.ID} [label = \"{weight}\"]\n";
        }

        public Connection(Node from, Node to, float weight)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
            from.outputConnections.Add(this);
        }

        public static string Hash(Node from, Node to)
        {
            return $"{from.ID}-{to.ID}";
        }

        public void MutateWeight()
        {
            float rand = UnityEngine.Random.value;
            if (rand < 0.1f)
            { //10% of the time completely change the this.weight
                this.weight = UnityEngine.Random.Range(-1f, 1f);
            }
            else
            { //otherwise slightly change it
                this.weight += UnityEngine.Random.Range(-GameManager.MutationForce, GameManager.MutationForce);
                this.weight = Mathf.Clamp(weight, -1f, 1f);
            }
        }
    }
}