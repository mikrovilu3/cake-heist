using UnityEngine;

public class FollowParentPosition : MonoBehaviour
{
    public Transform parentObject;

    void Update()
    {
        if (parentObject != null)
        {
            transform.position = parentObject.position; // ✅ Only copies position
        }
    }
}
