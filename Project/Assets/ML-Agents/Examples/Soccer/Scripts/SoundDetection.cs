using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDetection : MonoBehaviour
{
    // List to keep track of objects within the sphere
    private List<GameObject> objectsInSphere = new List<GameObject>();

    void OnTriggerEnter(Collider other)
        {
            // Add the object to the list if it's not already in it
                    if (!objectsInSphere.Contains(other.gameObject) && (other.CompareTag("blueAgent") || other.CompareTag("purpleAgent")))
                    {
                        objectsInSphere.Add(other.gameObject);
                        Debug.Log(other.name + " entered the detection sphere.");
                    }
        }

    void OnTriggerExit(Collider other)
        {
            // Remove the object from the list
                    if (!objectsInSphere.Contains(other.gameObject) && (other.CompareTag("blueAgent") || other.CompareTag("purpleAgent")))
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
            foreach (GameObject obj in objectsInSphere)
            {
                // Ensure the object still exists (in case it's destroyed elsewhere)
                if (obj != null)
                {
                    Vector3 position = obj.transform.position;
                    // Perform your logic with the position data
                    Debug.Log(obj.name + " is at position " + position);
                }
            }
        }
}
