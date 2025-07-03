using UnityEngine;

public class destroyaftertime : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(detroy),3f) ;
    }

    // Update is called once per frame
    void detroy()
    {
        Destroy(gameObject);
    }
}
