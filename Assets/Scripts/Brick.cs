using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brick : MonoBehaviour
{
    [SerializeField] GameObject destructiblePrefab;
    [SerializeField] int hitScore = 10;
    [SerializeField] int destroyScore = 50;
    [SerializeField] float maxDamageMapIntensity = 5f;
    [SerializeField] AudioClip hitClip;
    [SerializeField] List<Color> colors;

    int health = 1;
    int maxHealth = 0;
    float damageMapIntensity = 0f;

    int instantiatedHealth = 0;

    Renderer renderComponent;
    AudioSource audioSource;
    bool isColliding = false;

    MaterialPropertyBlock propertyBlock;

    public void SetHealth(int health)
    {
        if(instantiatedHealth == 0)
        {
            instantiatedHealth = health;
        }
        health = Mathf.Clamp(health, 0, maxHealth);
        this.health = health;
        if(health > 0)
        {
            propertyBlock.SetColor("_Color", colors[health - 1]);
            if(health < instantiatedHealth)
            {
                damageMapIntensity = (Mathf.Pow(1 - Mathf.InverseLerp(1, instantiatedHealth, health), 2)) * maxDamageMapIntensity;
                propertyBlock.SetFloat("_BumpScale", damageMapIntensity);
                propertyBlock.SetFloat("_OcclusionStrength", 1 - Mathf.InverseLerp(1, instantiatedHealth, health));
            }
            renderComponent.SetPropertyBlock(propertyBlock);
        }
    }

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        audioSource = GetComponent<AudioSource>();
        renderComponent = GetComponent<Renderer>();
        maxHealth = colors.Count;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Early return to prevent multiple collisions
        if(isColliding) return;

        // health -= 1;
        SetHealth(health - 1);

        audioSource.pitch = Random.Range(0.8f, 1.2f);

        audioSource.Stop();
        if(health > 0)
        {
            EventHandler.Instance.hub.Publish(new Score(hitScore));
            audioSource.PlayOneShot(hitClip);
        }
        else
        {
            EventHandler.Instance.hub.Publish(new Score(destroyScore));
            EventHandler.Instance.hub.Publish(new BrickDestroyed());
            var contact = collision.GetContact(0);
            // Slightly delay destruction to let ball bounce first
            StartCoroutine(DestroySelf(collision.relativeVelocity.magnitude * 0.4f, contact.point));
        }
        isColliding = true;
    }

    IEnumerator DestroySelf(float force, Vector3 point)
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Instantiate(destructiblePrefab, transform.position, transform.rotation)
            .GetComponent<DestroyedBrick>().SetExplosionParameters(force, point, 3f, 0f);
        Destroy(gameObject);
    }

    void OnCollisionExit(Collision other)
    {
        isColliding = false;
    }
}
