using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public LayerMask targetLayer; // ✅ Define which layers should destroy the bullet
    public GameObject impact;
    void OnCollisionEnter(Collision other)
    {
        
            Instantiate(impact);
            Destroy(gameObject); // ✅ Bullet disappears only when hitting valid layers
        
    }
}