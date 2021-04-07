using UnityEngine;

public class ShowReleaseText : MonoBehaviour
{
    void Awake()
    {
        EventHandler.Instance.hub.Subscribe<GameStart>(g => this.gameObject.SetActive(true));
        EventHandler.Instance.hub.Subscribe<BallReleased>(b => this.gameObject.SetActive(false));
    }
}
