using UnityEngine;

public class ObjectSizeChecker : MonoBehaviour
{
    public GameObject targetObject;

    void Start()
    {
        if (targetObject != null)
        {
            Renderer renderer = targetObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                Vector3 objectSize = renderer.bounds.size;

                Debug.Log("Object Width (X): " + objectSize.x + " meters");
                Debug.Log("Object Height (Y): " + objectSize.y + " meters");
                Debug.Log("Object Depth (Z): " + objectSize.z + " meters");
            }
            else
            {
                Debug.LogError("Renderer component not found on the target object.");
            }
        }
        else
        {
            Debug.LogError("Target object is not assigned.");
        }
    }
}
