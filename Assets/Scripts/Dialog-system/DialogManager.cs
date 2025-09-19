using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogPanelPrefab;
    [SerializeField] private Canvas dialogCanvas;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Pause Integration")]
    [SerializeField] private PauseCoordinator coordinator;  // Assign in inspector
    [SerializeField] private string pauseGroup = "MovementOnly";  // Data-driven: Configurable group

    private GameObject dialogPanel;
    private Image avatarImage;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogText;
    private GameObject choicePanel;
    private Button[] choiceButtons;

    private Dialog currentDialog;
    private int currentLineIndex;
    private bool isDialogActive;
    private bool isTyping;
    private Action<int> onChoiceSelected;
    private Action onDialogEnd;
    private Dialog nextDialog;
    private Action<int> nextChoiceSelected;
    private Action nextDialogEnd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeUI();
        if (coordinator == null)
            coordinator = FindAnyObjectByType<PauseCoordinator>(FindObjectsInactive.Include);
    }

    private void InitializeUI()
    {
        if (dialogCanvas == null)
            dialogCanvas = GameObject.Find("DialogCanvas")?.GetComponent<Canvas>();

        dialogPanel = Instantiate(dialogPanelPrefab, dialogCanvas.transform);
        DontDestroyOnLoad(dialogCanvas.gameObject);

        Transform dialogBoard = dialogPanel.transform.Find("DialogBoard");
        avatarImage = dialogBoard.transform.Find("Avatar_Image")?.GetComponent<Image>();
        nameText = dialogBoard.transform.Find("NamePanel/NameText")?.GetComponent<TextMeshProUGUI>();
        dialogText = dialogBoard.transform.Find("DialogTextPanel/DialogText")?.GetComponent<TextMeshProUGUI>();
        choicePanel = dialogBoard.transform.Find("ChoicePanel")?.gameObject;

        if (choicePanel != null)
            choiceButtons = choicePanel.GetComponentsInChildren<Button>();
        else
            choiceButtons = new Button[0];

        dialogPanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogActive && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = ProcessDialogText(currentDialog.lines[currentLineIndex].text, currentDialog.lines[currentLineIndex].vendorData);
                isTyping = false;
            }
            else if (!choicePanel.activeSelf)
            {
                AdvanceDialog();
            }
        }
    }

    public void StartDialog(Dialog dialog, Action<int> onChoiceSelected = null, Action onDialogEnd = null)
    {
        if (isDialogActive)
        {
            nextDialog = dialog;
            nextChoiceSelected = onChoiceSelected;
            nextDialogEnd = onDialogEnd;
            return;
        }

        Debug.Log("DialogManager: Starting dialog, attempting pause...");  // NEW: Entry point log

        // Pause movement via coordinator
        if (coordinator == null)
        {
            Debug.LogError("DialogManager: Coordinator is NULL! Assign in Inspector.");  // NEW: Null check
            return;
        }
        coordinator.PauseGroup(pauseGroup);
        Debug.Log($"DialogManager: Paused group '{pauseGroup}' via coordinator.");  // NEW: Confirm call
          
        isDialogActive = true;
        currentDialog = dialog;
        currentLineIndex = 0;
        this.onChoiceSelected = onChoiceSelected;
        this.onDialogEnd = onDialogEnd;
        dialogPanel.SetActive(true);
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentLineIndex < currentDialog.lines.Count)
        {
            Dialog.DialogLine line = currentDialog.lines[currentLineIndex];
            avatarImage.sprite = line.speakerAvatar;
            nameText.text = line.speakerName;

            string processedText = ProcessDialogText(line.text, line.vendorData);
            StartCoroutine(TypeSentence(processedText));

            if (line.choices.Count > 0)
            {
                choicePanel.SetActive(true);
                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    if (i < line.choices.Count)
                    {
                        choiceButtons[i].gameObject.SetActive(true);
                        TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                        buttonText.text = ProcessDialogText(line.choices[i], line.vendorData);

                        int choiceIndex = i;
                        choiceButtons[i].onClick.RemoveAllListeners();
                        choiceButtons[i].onClick.AddListener(() => SelectChoice(choiceIndex));
                    }
                    else
                    {
                        choiceButtons[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                choicePanel.SetActive(false);
            }
        }
        else
        {
            EndDialog();
        }
    }

    private string ProcessDialogText(string originalText, VendorData vendorData)
    {
        if (vendorData == null) return originalText;
        return originalText.Replace("{entryCost}", vendorData.entryCost.ToString());
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void AdvanceDialog()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogText.text = ProcessDialogText(currentDialog.lines[currentLineIndex].text, currentDialog.lines[currentLineIndex].vendorData);
            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            DisplayCurrentLine();
        }
    }

    private void SelectChoice(int choiceIndex)
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogText.text = ProcessDialogText(currentDialog.lines[currentLineIndex].text, currentDialog.lines[currentLineIndex].vendorData);
            isTyping = false;
        }
        onChoiceSelected?.Invoke(choiceIndex);
        EndDialog();
    }

    private void EndDialog()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
        }
        isDialogActive = false;
        dialogPanel.SetActive(false);
        choicePanel.SetActive(false);
        onDialogEnd?.Invoke();

        // Resume movement via coordinator
        coordinator?.ResumeGroup(pauseGroup);

        if (nextDialog != null)
        {
            Dialog queuedDialog = nextDialog;
            Action<int> queuedChoiceSelected = nextChoiceSelected;
            Action queuedDialogEnd = nextDialogEnd;

            nextDialog = null;
            nextChoiceSelected = null;
            nextDialogEnd = null;

            StartDialog(queuedDialog, queuedChoiceSelected, queuedDialogEnd);
        }
    }

    public void ForceEndDialog()
    {
        if (isDialogActive)
        {
            StopAllCoroutines();
            isTyping = false;
            isDialogActive = false;
            dialogPanel.SetActive(false);
            choicePanel.SetActive(false);
            nextDialog = null;
            nextChoiceSelected = null;
            nextDialogEnd = null;

            // Force resume
            coordinator?.ResumeGroup(pauseGroup);
        }
    }

    public bool IsDialogActive() => isDialogActive;
}