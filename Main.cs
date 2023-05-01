using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Bayesian bn = new Bayesian();

        // Add nodes to the network
        /*bn.AddNode("A", null);
        bn.AddNode("B", new List<string> { "A" });
        bn.AddNode("C", new List<string> { "B" });

        // Set conditional probabilities for each node
        bn.SetPro=-babilities("A", 0.3f);
        bn.SetProbabilities("B", new Dictionary<string, float>{
            {"A=true", 0.8f},
            {"A=false", 0.2f}
        });
        bn.SetProbabilities("C", new Dictionary<string, float>{
            {"B=true", 0.9f},
            {"B=false", 0.1f}
        });*/

        // Calculate probability of node A being true given evidence
        /*float pA = bn.CalculateProbability("A", new Dictionary<string, bool> { { "B", true }, { "C", false } });

        Debug.Log("Probability of A=true given B=true and C=false: " + pA);

        // Calculate probability of node A being true given evidence
        float pB = bn.CalculateProbability("A", new Dictionary<string, bool> { { "A", false }, { "C", true } });

        Debug.Log("Probability of B=true given A=true and C=false: " + pB);*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}
