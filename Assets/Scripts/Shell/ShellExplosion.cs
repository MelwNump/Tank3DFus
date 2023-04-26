using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask tankMask;
    public ParticleSystem explosionParticles;
    public AudioSource explosionAudio;
    public float maxDamage = 100f;
    public float explosionForce = 1000f;
    public float maxLifeTime = 2f;
    public float explosionRadius = 5f;

    public void Inti(Vector3 shellInstance)
    {
        GetComponent<Rigidbody>().velocity = shellInstance;
    }
    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRb = colliders[i].GetComponent<Rigidbody>();

            if (!targetRb)
                continue;

            targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            TankHealth targetHealth = targetRb.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRb.position);
            targetHealth.TakeDamage(damage);

            explosionParticles.transform.parent = null;
            explosionParticles.Play();
            explosionAudio.Play();

            Destroy(explosionParticles.gameObject, 3f);
            Destroy(this.gameObject);
        }

        explosionParticles.transform.parent = null;
        explosionParticles.Play();
        explosionAudio.Play();

        Destroy(explosionParticles.gameObject, 3f);
        Destroy(this.gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;
        float damage = relativeDistance * maxDamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }
}