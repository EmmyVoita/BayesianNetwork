

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ProbTable 
{
    public SerializableDictionary<SerializableDictionary<Node, bool>, float> probTable = new SerializableDictionary<SerializableDictionary<Node, bool>, float>(new NodeDictionaryComparer());

    public ProbTable()
    {
    }
    public ProbTable(List<Node> nodeList)
    {
        DefineDefaultProbTable(nodeList);
    }
   
    public void DefineDefaultProbTable(List<Node> nodeList)
    {
        int numParents = nodeList.Count;
        int numCombinations = (int)Mathf.Pow(2, numParents);

        for (int i = 0; i < numCombinations; i++)
        {
            SerializableDictionary<Node, bool> parentStates = new SerializableDictionary<Node, bool>();
            for (int j = 0; j < numParents; j++)
            {
                bool state = ((i >> j) & 1) == 1;
                parentStates.Add(nodeList[j], state);
            }
            probTable.Add(parentStates, 0.0f);
        }
    }
    public void PrintProbTable()
    {
        string probTableString = "Probability Table:\n";

        foreach (var entry in probTable)
        {
            var parentStates = entry.Key;
            var probability = entry.Value;

            // concatenate parentStates and probability into probEntryString
            string probEntryString = "";
            foreach (var state in parentStates)
            {
                probEntryString += state.Key.nodeName + " : " + state.Value + ", ";
            }
            probEntryString = probEntryString.TrimEnd(',', ' ') + " => " + probability + "\n";

            probTableString += probEntryString;
        }

        Debug.Log(probTableString);
    }
    public ProbTable Join(HashSet<ProbTable> conditional_probability_tables)
    {
        Debug.Log("Joining tables");

        

        List<Node> allNodes = new List<Node>();
        foreach (ProbTable conditional_probability_table in conditional_probability_tables)
        {
            foreach (var key in conditional_probability_table.probTable.Keys)
            {
                foreach (var node in key.Keys)
                {
                    if (!allNodes.Contains(node))
                    {
                        allNodes.Add(node);
                    }
                }
            }
        }


        ProbTable joint_probability_table = new ProbTable();
        List<SerializableDictionary<Node, bool>> node_state_combinations = GenerateAllCombinations(allNodes);

        foreach (SerializableDictionary<Node, bool> node_state_combination in node_state_combinations)
        {
            Debug.Log(node_state_combination.ToString());

            Node head_node = FindHeadNode(allNodes);
            //Debug.Log("HeadNode = " + head_node.name);

            //recursive function to iterate through all parents to find the joint probability
            joint_probability_table.probTable[node_state_combination] = CalculateJointProbability(node_state_combination,  head_node);
        }

        return joint_probability_table;
    }
    public Node FindHeadNode(List<Node> allNodes)
    {
        // Find the head node of the node list: the head node should not have any parent node in the list
        foreach (Node node in allNodes)
        {
            bool head_node = true;
            foreach (Node otherNode in allNodes)
            {
                if (otherNode.parentNodes.Contains(node) && node != otherNode)
                {
                    head_node = false;
                    break;
                }
            }
            if (head_node)
            {
                return node;
            }
        }
        Debug.Log("No head node");
        // If no node was found to be the head node, return null
        return null;
    }
    public float CalculateJointProbability(SerializableDictionary<Node, bool> node_state_combination, Node currentNode)
    {
        float probability = 1.0f;

        //Debug.Log("Current Node" + currentNode.name);

        SerializableDictionary<Node, bool> current_node_environment = new SerializableDictionary<Node, bool>();
        
        // add the current node + all parent nodes to the node a dictionary to get the conditional probability from the current nodes table 
        foreach (Node parent_node in currentNode.parentNodes)
        {
            current_node_environment.Add(node_state_combination.FirstOrDefault(x => x.Key == parent_node).Key, node_state_combination.FirstOrDefault(x => x.Key == parent_node).Value);
        }

        //Debug.Log("Here is the current node environment: " + current_node_environment.ToString());


        if (currentNode.parentNodes.Count == 1)
        {
            //Debug.Log("Current Node" + currentNode.name + " is root node");
            probability *= GetConditionalProbability(current_node_environment, currentNode);
        }
        else
        {
            foreach (Node parent_node in currentNode.parentNodes)
            {
                if (parent_node == currentNode)
                    continue;
                //Debug.Log("Looping through parent nodes");
                probability *= CalculateJointProbability(node_state_combination, parent_node);
            }

            probability *= GetConditionalProbability(current_node_environment, currentNode);
            
        }

        return probability;
    }
    public float GetConditionalProbability(SerializableDictionary<Node, bool> node_state_combination, Node node)
    {
        //get the conditional probability for a node given the a dicitonary containing the node state combinations
        if(node.conditionalProbTable.probTable.TryGetValue(node_state_combination, out float out_probability))
            return out_probability;
        else
            return 0.0f;
    }
    // Generates all possible combinations of states for the given nodes
    public List<SerializableDictionary<Node, bool>> GenerateAllCombinations(List<Node> nodes)
    {
        List<SerializableDictionary<Node, bool>> allCombinations = new List<SerializableDictionary<Node, bool>>();
        int totalCombinations = (int)Mathf.Pow(2, nodes.Count);

        for (int i = 0; i < totalCombinations; i++)
        {
            SerializableDictionary<Node, bool> combination = new SerializableDictionary<Node, bool>();
            for (int j = 0; j < nodes.Count; j++)
            {
                combination.Add(nodes[j], ((i >> j) & 1) == 1 ? true : false);
            }
            allCombinations.Add(combination);
        }

        return allCombinations;
    }
    public float GetParentProbability(Node node, SerializableDictionary<Node, bool> node_state_combination)
    {
        float probability = 1.0f;
        if (node.parentNodes != null && node.parentNodes.Count > 0)
        {
            foreach (Node parent in node.parentNodes)
            {
                if (parent == node) continue;

                SerializableDictionary<Node, bool> parentValues = new SerializableDictionary<Node, bool>();
                if (node_state_combination.TryGetValue(parent, out bool parent_state))
                {
                    parentValues[parent] = parent_state;
                }

                if (parent.conditionalProbTable.probTable.TryGetValue(parentValues, out float parent_probability))
                {
                     Debug.Log("Parent probability: " + parent_probability);
                    probability *= parent_probability * GetParentProbability(parent, node_state_combination);
                }
            }
        }
        return probability;
        
    }
    public void EliminateHiddenVariable(Node hiddenVariable)
    {


        // Create a list of all variables except the hidden one
        List<Node> visibleNodes = probTable.Keys.First().Keys.Where(node => node != hiddenVariable).ToList();


        //remove the invisible node from the parents
        foreach(Node visible_node in visibleNodes)
        {
            visible_node.parentNodes.Remove(hiddenVariable);
        }

        // Compute the probability of each combination of states of visible variables,
        // by summing probabilities of all states of the hidden variable

        ProbTable visible_probability_table = new ProbTable();

        foreach (var visible_node_state_combination in GenerateAllCombinations(visibleNodes))
        {
            float sum = 0f;
            foreach (var hidden_state in GenerateAllCombinations(new List<Node> { hiddenVariable }))
            {
                
                // add the hidden varaible onto the end of the visible combination
                var key = new Dictionary<Node, bool>(visible_node_state_combination);
                key.Add(hiddenVariable, hidden_state[hiddenVariable]);
                
                foreach(KeyValuePair<SerializableDictionary<Node, bool>, float> keyValuePair in probTable)
                {
                    if (AreDictionariesEqual(keyValuePair.Key, key))
                    {
                        // keyValuePair.Key is equal to key
                        sum += keyValuePair.Value;
                    }
                }
            }
           visible_probability_table.probTable.Add(visible_node_state_combination, sum);
        }

        // Replace the old probability table with the new one
        probTable = visible_probability_table.probTable;
    }
    public void Normalize()
    {
        // create a copy of the dictionary
        var copy = new SerializableDictionary<SerializableDictionary<Node, bool>, float>(probTable);

        // calculate the sum of probabilities
        float sum = 0f;
        foreach (KeyValuePair<SerializableDictionary<Node, bool>, float> kvp in copy)
        {
            sum += kvp.Value;
        }
        Debug.Log("Sum: " + sum);
        // divide each probability by the sum
        foreach (KeyValuePair<SerializableDictionary<Node, bool>, float>  kvp in copy)
        {
            probTable[kvp.Key] = kvp.Value / sum;
        }

        
    }
    public bool AreDictionariesEqual<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
    {
        if (dict1.Count != dict2.Count)
        {
            return false;
        }

        foreach (var kvp in dict1)
        {
            if (!dict2.TryGetValue(kvp.Key, out TValue value) || !kvp.Value.Equals(value))
            {
                return false;
            }
        }

        return true;
    }




}