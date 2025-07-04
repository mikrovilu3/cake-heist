using ASD;
using UnityEngine;
using UnityEngine.AI;

public class BulletCollision : MonoBehaviour
{
    public LayerMask targetLayer; // ✅ Define which layers should destroy the bullet
    public GameObject impact;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {

            other.gameObject.transform.Rotate(new Vector3(-90,0,0));
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            other.gameObject.GetComponent<EnemyAI>().enabled = false;
        }
            Instantiate(impact,transform.position,new Quaternion(0,0,0,0));
            Destroy(gameObject); // ✅ Bullet disappears only when hitting valid layersa
        
    }
}