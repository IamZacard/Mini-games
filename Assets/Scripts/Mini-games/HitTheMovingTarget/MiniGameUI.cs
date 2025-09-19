using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MiniGameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TextMeshProUGUI hitsText;
    [SerializeField] private TextMeshProUGUI missesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFillBar;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button closeWinButton;
    [SerializeField] private Button closeLoseButton;

    [Header("Animation Settings")]
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float punchDuration = 0.3f;
}