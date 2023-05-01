using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class Bayesian : MonoBehaviour
{




    public float probability;

    public List<Node> nodeList;
    //public Node nodeA;
    //public Node nodeB;
    //public Node nodeC;

    //public Node nodeD;

    public SerializableDictionary<Node, bool> query;

    private SerializableDictionary<Node, bool> query_copy;

    public SerializableDictionary<Node, bool> evidence;

    private SerializableDictionary<Node, bool> evidence_copy;


    private void Start()
    {
        //List<Node> node_list_copy = new List<Node>(nodeList);
        query_copy = new SerializableDictionary<Node, bool>();
        evidence_copy = new SerializableDictionary<Node, bool>();

        foreach(KeyValuePair<Node, bool> query_node_state_pair in query)
        {
            Node copy_node = Instantiate(query_node_state_pair.Key);
            query_copy.Add(copy_node, query_node_state_pair.Value);
        }

        foreach(KeyValuePair<Node, bool>  evidence_node_state_pair in evidence)
        {
            Node copy_node = Instantiate( evidence_node_state_pair.Key);
            evidence_copy.Add(copy_node,  evidence_node_state_pair.Value);
        }

        probability = Node.VariableElimination( query_copy, evidence_copy);
    }


    private void Update()
    {
        
        /*if (previousNodeAState != nodeAstate || previousNodeBState != nodeBstate || previousQueryAorD != QueryAorD)
        {

            evidence = new Dictionary<Node, bool>();
            evidence.Add(nodeB, nodeBstate);

            query = new Dictionary<Node, bool>();
            if(QueryAorD)
                query.Add(nodeA, nodeAstate);
            else
                query.Add(nodeD, nodeDstate);
            
            

            probability = nodeA.VariableElimination(new List<Node> { nodeA, nodeB, nodeC ,nodeD }, query, evidence);
            previousNodeAState = nodeAstate; // update previous value
            previousNodeBState = nodeBstate;
            previousQueryAorD = QueryAorD;
        }*/
        
    }

}

