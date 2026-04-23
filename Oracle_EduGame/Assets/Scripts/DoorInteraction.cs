using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public GameObject fadePanel;
    public GameObject endingUI;
    public AudioSource doorOpenSound;
    private bool waitingForRestart = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (waitingForRestart && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

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
        bool openingNow = !fillInTheBlankPanel.activeSelf;

        if (openingNow)
        {
            // 1. THE SAFETY CHECK: Did the timer finish while we were walking around?
            if (quizScript.isLockedOut && Time.unscaledTime >= quizScript.timerEndTime)
            {
                // If yes, we force the lockout to end before opening the panel
                quizScript.isLockedOut = false;
                quizScript.failedAttempts = 0;
                quizScript.quizCanvasGroup.interactable = true;
                quizScript.quizCanvasGroup.blocksRaycasts = true;
            }

            // 2. Open the main panel
            fillInTheBlankPanel.SetActive(true);
            
            // 3. Decide if the RED lockout screen should be visible
            quizScript.lockOutUI.SetActive(quizScript.isLockedOut);

            LockPlayer(true);
            quizScript.SetupRoom(roomNum); 
        } 
        else 
        {
            // Closing logic
            quizScript.CloseAllSuccessPopups();
            LockPlayer(false);
            fillInTheBlankPanel.SetActive(false);
            quizScript.lockOutUI.SetActive(false); // Hide red screen on exit
            
            if (quizScript.isRoomComplete) 
            {
                OpenDoor();
            }
        }
    }

    void OpenDoor()
    {
        isDoorOpen = true;
        
        if (roomNum == 1)
        {
            ShowEndingUI();
        } else
        {
            ArtifactInteraction.artifactsDecoded = 0; // reset for next room
            ArtifactInteraction.touchedOracleBone = false;

            // Door gets disabled immediately, so OnTriggerExit may never fire.
            // Explicitly clear prompt/range state to avoid a stuck "Press E".
            isPlayerInRange = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        
            doorObject.SetActive(false);
            doorOpenSound.Play();
            if (doorLight != null) {
                doorLight.SetActive(false);
            }
        }
    }

    public static void LockPlayer(bool lockIt)
    {
        Cursor.lockState = lockIt ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockIt;
        
        var controller = FindFirstObjectByType<FirstPersonController>();
        if(controller != null) controller.enabled = !lockIt;
    }

    void ShowEndingUI()
    {
        // Don't just pop things on/off here anymore. 
        // Start the Coroutine to handle the timing!
        StartCoroutine(FadeAndEnd());
    }

    IEnumerator FadeAndEnd()
    {
        // 1. Prepare the Fade Panel
        fadePanel.SetActive(true);
        Image panelImage = fadePanel.GetComponent<Image>();
        float alpha = 0;

        // 2. Fade to Black (The dramatic transition)
        while (alpha < 1)
        {
            // Use unscaledDeltaTime in case you have time paused
            alpha += Time.unscaledDeltaTime; 
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 3. While the screen is black, swap the UI
        yield return new WaitForSecondsRealtime(0.5f);
        
        if (endingUI != null)
        {
            endingUI.SetActive(true); // Show the Acknowledgement Screen
            waitingForRestart = true;
            LockPlayer(true);
        }

        // 4. (Optional) Fade the black panel back out so they can see the credits
        // If your Acknowledgement screen is its own full-screen image, 
        // you can just leave the black panel on or fade it out slowly:
        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime;
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        fadePanel.SetActive(false);
    }
}
