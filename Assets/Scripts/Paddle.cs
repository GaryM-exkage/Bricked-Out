using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] Ball ball; // Keep a reference to the ball for parenting/unparenting.
    [SerializeField] float ballOffset = 0.75f; // Offset of the ball to the paddle.
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float speed = 10.0f;
    [SerializeField] AudioClip paddleClip;
    AudioSource audioSource;

    bool hasBall = false;

    bool isGameOver = false;

    bool isPressing = false;

    Camera mainCam;
    Rigidbody rb;

    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        EventHandler.Instance.hub.Subscribe<GameStart>(g => {
            AttachBall();
        });

        EventHandler.Instance.hub.Subscribe<GameOver>(g => {
            isGameOver = true;
            AttachBall();
        });
    }

    void AttachBall()
    {
        ball.transform.SetParent(this.transform);
        ball.transform.position = transform.position + (Vector3.up * ballOffset);
        ball.Sleep();
        hasBall = true;
    }

    void Update()
    {
        if(hasBall && (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            if(isGameOver)
            {
                isGameOver = false;
                EventHandler.Instance.hub.Publish<Restart>(new Restart());
                return;
            }
            ball.transform.SetParent(null, true);
            ball.Launch();
            EventHandler.Instance.hub.Publish<BallReleased>(new BallReleased());
            hasBall = false;
        }
        // Cute ternary operator for getting pressed state outside of FixedUpdate
        isPressing = (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)) ? true : false;
        
    }

    void FixedUpdate()
    {

        if(isPressing)
        {
            // This move style is lerping towards the input position rather than treating input as an axis
            // so that it will always follow the mouse/finger position.
            var projectedMousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, transform.position.y, -mainCam.transform.position.z)) * sensitivity;
            var newPos = Vector3.MoveTowards(transform.position, new Vector3(Mathf.Clamp(projectedMousePos.x, -7.5f, 7.5f), transform.position.y, transform.position.z), speed);
            rb.MovePosition(newPos);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Slightly alleviate ear fatigue with randomized pitch
        audioSource.pitch = Random.Range(0.8f, 1.2f);

        // Prevent double triggers
        audioSource.Stop();
        audioSource.PlayOneShot(paddleClip);
    }
}
