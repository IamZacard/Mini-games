using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshProUGUI debugText;

    private void Update()
    {
        if (player == null || debugText == null) return;

        debugText.text =
            $"<b>PLAYER DEBUG</b>\n" +
            $"IsPaused: {player.IsPaused}\n" +            
            $"Grounded: {player.IsGrounded()}\n" +
            $"Jumping: {player.IsJumping()}\n" +
            $"Dashing: {player.IsDashing()}\n" +
            $"Velocity: {player.GetComponent<Rigidbody2D>().linearVelocity}\n" +
            $"Last Grounded Time: {Time.time - player.GetLastGroundedTime():F0}s ago\n" +
            $"Last Dash Time: {Time.time - player.GetLastDashTime():F0}s ago\n" +
            $"Money: {inventory.Get(ResourceType.Gold)}\n";
    }
}
