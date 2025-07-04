using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTheDore : MonoBehaviour
{

    
    bool isNearDore = false;
    public string scene = "Pēteris_gulamistaba";

    void Start()
    {
       

    }

    void Update()
    {
        

        if (isNearDore)
        {

            if (Input.GetKeyDown(KeyCode.E) )
            {   
                SceneManager.LoadScene(scene);
            }else if (Input.GetKeyDown(KeyCode.E))
            {
                
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isNearDore = true;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isNearDore = false;
            
        }
    }


}
