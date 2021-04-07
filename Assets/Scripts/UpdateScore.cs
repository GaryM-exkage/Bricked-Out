using UnityEngine;
using TMPro;

public class UpdateScore : MonoBehaviour
{
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        EventHandler.Instance.hub.Subscribe<ScoreTotal>(s => text.text = $"Score: {s.score}");
    }
}
