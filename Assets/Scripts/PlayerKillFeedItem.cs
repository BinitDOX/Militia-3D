using UnityEngine;
using UnityEngine.UI;

public class PlayerKillFeedItem : MonoBehaviour
{
    [SerializeField]
    Text sourceText;
    [SerializeField]
    Text targetText;

    public void Setup(string player, string source)
    {
        sourceText.text = source;
        targetText.text = player;
    }
}
