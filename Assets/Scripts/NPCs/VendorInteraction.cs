using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class VendorInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private InteractionPrompt interactionPrompt;

    [Header("Game Data")]
    [SerializeField] private VendorData vendorData;
    //[SerializeField] private PlayerStats playerStats;
    //[SerializeField] private Inventory inventory;

    [Header("Dialogs")]
    [SerializeField] private Dialog firstDialog;
    [SerializeField] private Dialog repeatDialog;
    [SerializeField] private Dialog enoughMoneyDialog;
    [SerializeField] private Dialog notEnoughMoneyDialog;
    [SerializeField] private Dialog declineDialog;

    [Header("Events")]
    public UnityEvent OnStartMiniGame;

    private bool playerInRange;
    private bool firstTime = true;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            interactionPrompt?.StopAllFeedback();
            StartDialogFlow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        interactionPrompt?.Show();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        interactionPrompt?.Hide();
    }

    private void StartDialogFlow()
    {
        var dialog = firstTime && firstDialog != null ? firstDialog : repeatDialog;
        if (firstTime) firstTime = false;

        if (dialog != null)
            DialogManager.Instance.StartDialog(dialog, HandleChoice);
        else
            Debug.LogWarning($"No dialog assigned for vendor: {name}");
    }

    private void HandleChoice(int choiceIndex)
    {
        if (choiceIndex == 0) // Accept
            HandleAccept();
        else // Decline
            HandleDecline();
    }

    private void HandleAccept()
    {
        /*if (inventory.Spend(ResourceType.Gold, vendorData.entryCost))
        {
            if (enoughMoneyDialog != null)
                DialogManager.Instance.StartDialog(enoughMoneyDialog, null, TryStartMiniGame);
            else
                TryStartMiniGame();
        }
        else
        {
            if (notEnoughMoneyDialog != null)
                DialogManager.Instance.StartDialog(notEnoughMoneyDialog);
        }*/
    }

    private void HandleDecline()
    {
        if (declineDialog != null)
            DialogManager.Instance.StartDialog(declineDialog);
    }

    private void TryStartMiniGame()
    {
        OnStartMiniGame?.Invoke();
    }

    public int GetEntryCost() => vendorData?.entryCost ?? 0;
}