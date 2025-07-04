using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTheDore : MonoBehaviour
{
    public float sizeNeededToExit = 1;
    public MovementScaler scaler;
    bool isNearDore = false;
    private float scale1;
    private tooltipwarden Textuw;
    public string scene = "Pēteris_gulamistaba";

    void Start()
    {
        scaler = GameObject.Find("First Person Controller").GetComponent<MovementScaler>();
        Textuw = GameObject.Find("uii/tooltip").GetComponent<tooltipwarden>();
    }

    void Update()
    {
        scale1 = scaler.scale;

        if (isNearDore)
        {

            if (Input.GetKeyDown(KeyCode.E) && scale1 >= sizeNeededToExit)
            {   
                SceneManager.LoadScene(scene);
            }else if (Input.GetKeyDown(KeyCode.E))
            {
                Textuw.avalableExit = -1;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == scaler.gameObject)
        {
            isNearDore = true;
            Textuw.avalableExit=1;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == scaler.gameObject)
        {
            isNearDore = false;
            Textuw.avalableExit=0;
        }
    }


}
