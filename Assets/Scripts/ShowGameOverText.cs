using UnityEngine;

public class ShowGameOverText : MonoBehaviour
{

    void Awake()
    {
        EventHandler.Instance.hub.Subscribe<GameOver>(g => this.gameObject.SetActive(true));
        EventHandler.Instance.hub.Subscribe<Restart>(b => this.gameObject.SetActive(false));
        this.gameObject.SetActive(false);
    }
}
