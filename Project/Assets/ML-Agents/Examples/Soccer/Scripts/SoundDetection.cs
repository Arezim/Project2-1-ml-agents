using System;
using System.Collections;
using System.Collections.Generic;
using ML_Agents.Examples.Soccer.Scripts;
using UnityEngine;

public class SoundDetection : MonoBehaviour
{
    // List to keep track of objects within the sphere
    private List<GameObject> objectsInSphere = new List<GameObject>();
    // private Dictionary<ValueTuple<GameObject, GameObject>, Edge> adjMap;
    private Edge[][] adjMatrix;

    private void Start()
    {
        this.adjMatrix = new Edge[5][];
        for (int i = 0; i < adjMatrix.Length; i++)
        {
            adjMatrix[i] = new Edge[5];
        }
    }

    void OnTriggerEnter(Collider other)
        {
            // Add the object to the list if it's not already in it
                    if (!objectsInSphere.Contains(other.gameObject) && (other.CompareTag("blueAgent") || other.CompareTag("purpleAgent") || other.CompareTag("ball")))
                    {
                        objectsInSphere.Add(other.gameObject);
                        Debug.Log(other.name + " entered the detection sphere.");
                    }
        }

    void OnTriggerExit(Collider other)
        {
            // Remove the object from the list
                    if (!objectsInSphere.Contains(other.gameObject) && (other.CompareTag("blueAgent") || other.CompareTag("purpleAgent") || other.CompareTag("ball")))
                    {
                        objectsInSphere.Remove(other.gameObject);
                        Debug.Log(other.name + " exited the detection sphere.");
                    }
        }

    void FixedUpdate()
        {
            // Call the helper method every frame or as needed
            CheckPositions();
        }

    private void Update()
    {
        Debug.Log(this.getNearby(this.objectsInSphere[0], 1000000));
    }

    void CheckPositions()
        {
            for (int i = 0; i < objectsInSphere.Count; i++)
            {
                GameObject obj1 = objectsInSphere[i];

                for (int j = 0; j < objectsInSphere.Count; j++)
                {
                    GameObject obj2 = objectsInSphere[j];

                    if (i!=j)
                    {
                        // Edge tempEdge = new Edge(obj1, obj2, obj1.transform.position, obj2.transform.position);
                        // this.adjMap.Add(new ValueTuple<GameObject, GameObject>(obj1, obj2), tempEdge);
                        // this.adjMap.Add(new ValueTuple<GameObject, GameObject>(obj2, obj1), tempEdge.getReversed());
                        Edge newEdge = new Edge(obj1, obj2, obj1.transform.position, obj2.transform.position);
                        this.adjMatrix[i][j] = newEdge;
                    }
                    else
                    {
                        this.adjMatrix[i][j] = null;
                    }

                }
            }
        }

    public List<Edge> getNearby(GameObject gameObject, float threshold)
    {
        List<Edge> nearby = new List<Edge>();
        List<ValueTuple<int, int>> pairs =
            this.GenerateIndexPairs(this.objectsInSphere.IndexOf(gameObject), this.objectsInSphere.Count);
        foreach (ValueTuple<int, int> pair in pairs)
        {
            Edge edge = this.adjMatrix[pair.Item1][pair.Item2];
            if (edge != null && edge.getDistance() <= threshold)
            {
                nearby.Add(edge);
            }
        }

        return nearby;
    }

    /// <summary>
    /// Generates a list of value tuples pairing the given index with every other element.
    /// </summary>
    /// <param name="index">The index to pair with other elements.</param>
    /// <param name="totalElements">The total number of elements.</param>
    /// <returns>A list of (int, int) tuples.</returns>
    private List<(int, int)> GenerateIndexPairs(int index, int totalElements)
    {
        var pairs = new List<(int, int)>();

        for (int i = 0; i < totalElements; i++)
        {
            if (i != index)
            {
                pairs.Add((index, i));
            }
        }

        return pairs;
    }

}
