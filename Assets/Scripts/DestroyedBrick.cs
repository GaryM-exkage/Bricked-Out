using System.Collections;
using UnityEngine;

public class DestroyedBrick : MonoBehaviour
{
    // [SerializeField] float DeathTimer = 2f;
    [SerializeField] AudioClip destroyClip;
    AudioSource audioSource;
    Vector3 explosionPoint;
    float force = 1;
    float radius = 1;
    float upwards = 1;

    Renderer[] allChildren;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(destroyClip);

        foreach (var shard in GetComponentsInChildren<Rigidbody>())
        {
            shard.AddExplosionForce(force, explosionPoint, radius, upwards, ForceMode.Impulse);
            
        }

        // Invoke("DestroySelf", DeathTimer);
        StartCoroutine(DestroyChildren());
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    IEnumerator DestroyChildren()
    {
        allChildren = GetComponentsInChildren<Renderer>();

        yield return new WaitForSeconds(0.3f);
        foreach (var child in allChildren)
        {
            child.gameObject.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void SetExplosionParameters(float force, Vector3 point, float radius, float upwards)
    {
        this.force = force;
        this.explosionPoint = point;
        this.radius = radius;
        this.upwards = upwards;
    }
}
