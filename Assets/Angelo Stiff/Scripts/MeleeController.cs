using UnityEngine;
using System.Collections;

namespace Com.Kawaiisun.SimpleHostile
{
    public class MeleeController : MonoBehaviour
    {
        [Header("Melee Attack")]
        public KeyCode attackKey = KeyCode.Mouse0;
        public float attackDamage = 50f;
        public float attackRange = 2f;
        public LayerMask enemyLayers = -1;

        [Header("Attack Animation")]
        public Vector3 attackOffset = new Vector3(0, 0, 0.5f);
        public Vector3 attackRotation = new Vector3(-45f, 0, 0);
        public float attackSpeed = 8f;
        public float returnSpeed = 6f;

        [Header("Attack Timing")]
        public float attackCooldown = 1f;
        public float damageWindow = 0.3f; // When during attack animation damage is dealt

        [Header("Visual Effects")]
        public GameObject hitEffect;
        public AudioClip attackSound;
        public AudioClip hitSound;

        // Private variables
        private Vector3 originalPosition;
        private Vector3 originalRotation;
        private bool isAttacking = false;
        private bool canAttack = true;
        private bool hasDealDamage = false;
        private AudioSource audioSource;
        private Camera playerCamera;

        private void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localEulerAngles;

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            playerCamera = Camera.main;
            if (playerCamera == null)
                playerCamera = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(attackKey) && canAttack && !isAttacking)
            {
                StartCoroutine(PerformAttack());
            }
        }

        private IEnumerator PerformAttack()
        {
            isAttacking = true;
            canAttack = false;
            hasDealDamage = false;

            // Play attack sound
            if (attackSound != null && audioSource != null)
                audioSource.PlayOneShot(attackSound);

            float attackTimer = 0f;
            float attackDuration = 1f / attackSpeed;

            // Attack forward motion
            while (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
                float progress = attackTimer / attackDuration;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                // Animate position and rotation
                Vector3 targetPos = originalPosition + attackOffset;
                Vector3 targetRot = originalRotation + attackRotation;

                transform.localPosition = Vector3.Lerp(originalPosition, targetPos, smoothProgress);
                transform.localEulerAngles = Vector3.Lerp(originalRotation, targetRot, smoothProgress);

                // Deal damage at specific point in animation
                if (!hasDealDamage && progress >= damageWindow)
                {
                    DealDamage();
                    hasDealDamage = true;
                }

                yield return null;
            }

            // Return to original position
            attackTimer = 0f;
            float returnDuration = 1f / returnSpeed;

            while (attackTimer < returnDuration)
            {
                attackTimer += Time.deltaTime;
                float progress = attackTimer / returnDuration;
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                Vector3 currentPos = originalPosition + attackOffset;
                Vector3 currentRot = originalRotation + attackRotation;

                transform.localPosition = Vector3.Lerp(currentPos, originalPosition, smoothProgress);
                transform.localEulerAngles = Vector3.Lerp(currentRot, originalRotation, smoothProgress);

                yield return null;
            }

            // Ensure we're back to original transform
            transform.localPosition = originalPosition;
            transform.localEulerAngles = originalRotation;

            isAttacking = false;

            // Cooldown
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }

        private void DealDamage()
        {
            if (playerCamera == null) return;

            // Raycast from camera center
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, attackRange, enemyLayers))
            {
            }

            // Play hit sound
            if (hitSound != null && audioSource != null)
                audioSource.PlayOneShot(hitSound);

            // Spawn hit effect
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(effect, 2f);
            }

            Debug.Log($"Hit {hit.collider.name} for {attackDamage} damage!");

        }

        // Public methods for external control
        public void SetAttackEnabled(bool enabled)
        {
            canAttack = enabled;
            if (!enabled && isAttacking)
            {
                StopAllCoroutines();
                ResetWeaponTransform();
                isAttacking = false;
            }
        }

        public void ResetWeaponTransform()
        {
            transform.localPosition = originalPosition;
            transform.localEulerAngles = originalRotation;
        }

        public bool IsAttacking()
        {
            return isAttacking;
        }

        // Gizmo for visualizing attack range in editor
        private void OnDrawGizmosSelected()
        {
            if (playerCamera != null)
            {
                Gizmos.color = Color.red;
                Vector3 forward = playerCamera.transform.forward;
                Gizmos.DrawRay(playerCamera.transform.position, forward * attackRange);
                Gizmos.DrawWireSphere(playerCamera.transform.position + forward * attackRange, 0.1f);
            }
        }

    }
}