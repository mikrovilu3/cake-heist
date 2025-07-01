using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 10;
    public LayerMask targetLayer;
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 2f;

    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is on the target layer
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            HandleHit(other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object is on the target layer
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            HandleHit(collision.collider);
            Debug.Log("sukle my balls");
        }
    }

    private void HandleHit(Collider hitObject)
    {
        // Create hit effect
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectLifetime);
        }

        // Play hit sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Destroy the bullet
        Destroy(gameObject);
    }
}