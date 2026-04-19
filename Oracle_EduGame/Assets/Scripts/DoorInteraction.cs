using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class DoorInteraction : MonoBehaviour
{
    public int numOfArtifacts; // this will be 1 for room 0, and 5 for room 1
    public GameObject doorLight; // make door glow once they finish interacting with all objects
    public GameObject doorObject;
    public GameObject fillInTheBlankPanel;
    public GameObject interactPrompt; // press E
    public Image displayImage;
    public Sprite quizPreviewImage;
    private bool isPlayerInRange;
    private bool isDoorOpen = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpen)
        {
            ShowFillInTheBlank();
        }

        if (ArtifactInteraction.artifactsDecoded >= numOfArtifacts)
        {
            if (doorLight != null) {
                doorLight.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactPrompt.SetActive(false);
        }
    }

void ShowFillInTheBlank() 
{
    // 1. Figure out what we are about to do
    // If the panel is NOT active, it means we are OPENING it now.
    bool openingNow = !fillInTheBlankPanel.activeSelf;
    
    // 2. Set the panel to that new state
    fillInTheBlankPanel.SetActive(openingNow);

    if (openingNow)
    {
        // --- OPENING SEQUENCE ---
        LockPlayer(true); // Stop player and show mouse
        displayImage.sprite = quizPreviewImage;
    } 
    else 
    {
        // --- CLOSING SEQUENCE ---
        LockPlayer(false); // Resume player and hide mouse

        // Only check for the door success AFTER the panel is closed
        if (ArtifactInteraction.artifactsDecoded >= numOfArtifacts)
        {
            OpenDoor();
        }
    }
}

    void OpenDoor()
    {
        isDoorOpen = true;
        
        ArtifactInteraction.artifactsDecoded = 0; // reset for next room
        ArtifactInteraction.touchedOracleBone = false;
        
        doorObject.SetActive(false); 
        if (doorLight != null) {
            doorLight.SetActive(false);
        }
    }

    void LockPlayer(bool lockIt)
    {
        Cursor.lockState = lockIt ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockIt;
        
        var controller = FindFirstObjectByType<FirstPersonController>();
        if(controller != null) controller.enabled = !lockIt;
    }
}
