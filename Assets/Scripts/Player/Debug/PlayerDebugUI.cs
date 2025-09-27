using Mono.Cecil;
using TMPro;
using UnityEngine;
public class PlayerDebugUI : MonoBehaviour
{
    [SerializeField] private PlayerManager player;
    //[SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshProUGUI debugText;
    private void Update()
    {
        if (player == null || debugText == null) return;
        debugText.text =
        $"PLAYER DEBUG\n" +
        $"IsPaused: {player.IsPaused}\n" +
        $"Grounded: {player.MovementController.IsGrounded}\n" +
        $"Jumping: {player.MovementController.IsJumping}\n" +
        $"Dashing: {player.MovementController.IsDashing}\n" +
        $"Velocity: {player.GetComponent<Rigidbody2D>().linearVelocity}\n" +
        $"Last Grounded Time: {Time.time - player.MovementController.GetLastGroundedTime():F0}s ago\n" +
        $"Last Dash Time: {Time.time - player.MovementController.LastDashTime:F0}s ago\n";
        //$"Money: {inventory.Get(ResourceType.Gold)}\n";
    }
}