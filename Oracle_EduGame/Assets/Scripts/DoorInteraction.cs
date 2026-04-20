using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class DoorInteraction : MonoBehaviour
{
    public QuizManager quizScript;
    public int numOfArtifacts; // this will be 1 for room 0, and 5 for room 1
    public GameObject doorLight; // make door glow once they finish interacting with all objects
    public GameObject doorObject;
    public int roomNum; // 0 for room 0, 1 for room 1 - assign in inspector
    public GameObject fillInTheBlankPanel;
    public GameObject interactPrompt; // press E
    //public Image displayImage;
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
            ShowQuiz();
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

void ShowQuiz() 
{
    // Toggle the panel
    bool openingNow = !fillInTheBlankPanel.activeSelf;
    fillInTheBlankPanel.SetActive(openingNow);

    if (openingNow)
    {
        fillInTheBlankPanel.SetActive(true);
        LockPlayer(true);
        // Tell the quiz manager to set up the correct room!
        quizScript.SetupRoom(roomNum); 
    } 
    else 
    {
        LockPlayer(false);
        fillInTheBlankPanel.SetActive(false);
        // We only check for success if the QuizManager says isFinished is true
        if (quizScript.isRoomComplete) 
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
