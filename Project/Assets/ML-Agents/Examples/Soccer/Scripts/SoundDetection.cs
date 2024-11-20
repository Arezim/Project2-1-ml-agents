using System;
using System.Collections;
using System.Collections.Generic;
using ML_Agents.Examples.Soccer.Scripts;
using UnityEngine;

public class SoundDetection : MonoBehaviour
{
    // List to keep track of objects within the sphere
    private List<GameObject> objectsInSphere = new List<GameObject>();
    private Dictionary<ValueTuple<GameObject, GameObject>, Edge> adjMap;

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

    void Update()
        {
            // Call the helper method every frame or as needed
            CheckPositions();
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
                        Edge tempEdge = new Edge(obj1, obj2, obj1.transform.position, obj2.transform.position);
                        this.adjMap.Add(new ValueTuple<GameObject, GameObject>(obj1, obj2), tempEdge);
                        this.adjMap.Add(new ValueTuple<GameObject, GameObject>(obj2, obj1), tempEdge.getReversed());
                    }

                }
            }
        }

    // public List<Edge> getNearby(float threshold)
    // {
    //
    // }

}
