using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DiaryInteraction : MonoBehaviour
{
    public GameObject pressEPrompt;
    public GameObject diaryOverlay;
    public Sprite diaryPage;
    public bool diaryOpen;
    private bool isPlayerInRange;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("DIARY Script is firing!");
            diaryOpen = !diaryOpen;
            if (diaryOpen)
            {
                Image display = diaryOverlay.transform.Find("Image").GetComponent<Image>();
                if (display != null) {
                    display.sprite = diaryPage;
                }

                DoorInteraction.LockPlayer(true);
                diaryOverlay.SetActive(true);
                pressEPrompt.SetActive(false);
            } else
            {
                diaryOverlay.SetActive(false);
                pressEPrompt.SetActive(true);
                DoorInteraction.LockPlayer(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            pressEPrompt.SetActive(true);
            Debug.Log("Something entered the Diary trigger: " + other.name);
        }
    }

    void OnTriggerExit (Collider other)
    {
        pressEPrompt.SetActive(false);
        isPlayerInRange = false;
    }
}
