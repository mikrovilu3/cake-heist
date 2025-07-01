using TMPro;
using UnityEngine;

public class fpscheck : MonoBehaviour
{

    void Update()
    {
        Debug.Log("fps"+ (1.0 / Time.deltaTime));
    }

}
