using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float minSpeed = 5f;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float accelerationSpeed = 1.5f;
    [SerializeField] AudioClip bounceClip;
    [SerializeField] AudioClip failClip;
    [SerializeField] ParticleSystem sparks;
    AudioSource audioSource;
    Rigidbody rb;
    bool isColliding = false;
    float lastVelocity;

    float collisionCooldown = 0.1f;
    float collisionTimer = 0f;

    void EnableCollisions()
    {
        rb.detectCollisions = true;
    }

    public void Launch()
    {
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Invoke("EnableCollisions", 0.1f);
        rb.AddRelativeForce(Vector3.ClampMagnitude(new Vector3(Random.Range(-minSpeed, minSpeed), minSpeed, 0.0f), minSpeed), ForceMode.Impulse);
    }

    public void Sleep()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();

        EventHandler.Instance.hub.Subscribe<Fail>(f => audioSource.PlayOneShot(failClip));
    }


    void FixedUpdate()
    {
        // Clamp to min speed
        if(lastVelocity < minSpeed) rb.velocity = rb.velocity.normalized * minSpeed;
        // Linearly Accelerate
        rb.AddRelativeForce(rb.velocity.normalized * accelerationSpeed, ForceMode.Acceleration);
        // Clamp Max speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        lastVelocity = rb.velocity.magnitude;

        // Unity physics can be quite buggy for something as unrealistic as this.
        // Simply decrementing the timer back to 0 without doing a conditional.
        collisionTimer = Mathf.Max(collisionTimer - Time.deltaTime, 0);

    }

    void OnTriggerEnter(Collider other)
    {
        // Only one trigger in the game. The overlap for failing,
        // so do not query for components or tags.
        EventHandler.Instance.hub.Publish<Fail>(new Fail());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collisionTimer > 0 || isColliding) return; // Early return to prevent multiple collisions

        collisionTimer = collisionCooldown; // Reset the cooldown on a fresh collision

        // Use GetContact for this instead of contact[0] which allocates
        ContactPoint contact = collision.GetContact(0);

        // Slightly alleviate ear fatigue by randomizing pitch slightly.
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Stop();
        audioSource.PlayOneShot(bounceClip);

        // Caching the rigidbody for logic against the paddle
        // Because the Bricks do not have rigidbodies
        // Can also explicity GetComponent<Paddle>() however I would still
        // Need to query the Rigidbody afterwards anyways.
        // Avoiding checking tags here.
        var collrb = contact.otherCollider.GetComponent<Rigidbody>();
        if(collrb)
        {
            // Basically forcing the ball to move upwards after hitting the paddle if it is facing downwards.
            // if(contact.normal == Vector3.up && Vector3.SignedAngle(rb.velocity.normalized, Vector3.up, Vector3.up) > 90f)
            if(contact.normal == Vector3.up && Vector3.Dot(rb.velocity.normalized, Vector3.up) < 0)
            {
                rb.velocity = Vector3.Reflect(rb.velocity, contact.normal);
            }

            // Adding "spin" to the ball based on the velocity of the paddle
            // Then clamping for sane speeds. Game feel > Realism.
            rb.AddRelativeForce(new Vector3(Mathf.Min(-collrb.velocity.x * 0.7f, 7f), 0, 0), ForceMode.Impulse);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, lastVelocity);
        }

        // Prevent horizontal deadlock bouncing by very slightly pushing the ball downwards
        if(Vector3.SignedAngle(rb.velocity.normalized, Vector3.right, Vector3.up) <= 2f)
        {
            rb.AddRelativeForce(Vector3.down, ForceMode.Impulse);
        }

        // Reposition and Reorient the spark particle system to play off of the contact point
        sparks.transform.position = contact.point;
        sparks.transform.rotation = Quaternion.LookRotation(contact.normal, sparks.transform.forward);
        sparks.Play();

        isColliding = true;
    }

    void OnCollisionExit(Collision other)
    {
        isColliding = false;
    }
}
