using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public LayerMask targetLayer; // ✅ Define which layers should destroy the bullet

    void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0) // ✅ Only destroy bullet on correct collision
        {
            Destroy(gameObject); // ✅ Bullet disappears only when hitting valid layers
        }
    }
}