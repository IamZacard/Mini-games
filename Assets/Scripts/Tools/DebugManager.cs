using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Close all dialogs first
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
        }

    }
}
