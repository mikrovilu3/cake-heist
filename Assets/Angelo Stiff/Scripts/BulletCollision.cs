using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public LayerMask targetLayer; // ✅ Define which layers should destroy the bullet

    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject); // ✅ Bullet disappears only when hitting valid layers
    }
}
