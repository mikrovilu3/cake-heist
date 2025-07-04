using UnityEngine;

public class killBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Application.Quit();
    }
}
