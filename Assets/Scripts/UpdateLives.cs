using UnityEngine;
using TMPro;

public class UpdateLives : MonoBehaviour
{
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        EventHandler.Instance.hub.Subscribe<Lives>(l => text.text = $"Lives: {l.lives}");
    }
}
