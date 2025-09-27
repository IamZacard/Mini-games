using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerManager player;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.ForceEndDialog();
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.L) && player != null)
        {
            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            player.transform.position = mouseWorld;

            // Reset velocity after teleporting
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        // Additional debug keys
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}