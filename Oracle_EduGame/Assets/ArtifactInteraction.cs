using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    public GameObject muralToShow;
    public GameObject boneCharacter;
    private bool isPlayerInRange;
    public GameObject interactPrompt;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // if player collides with mural and presses E, show the mural
            ToggleMural();
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if player collides with the mural, set var to true
            isPlayerInRange = true;
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // once they leave, stop showing the mural
            isPlayerInRange = false;
            muralToShow.SetActive(false);
            interactPrompt.SetActive(false);
        }
    }

    void ToggleMural ()
    {
        if (muralToShow.activeSelf)
        {
            muralToShow.SetActive(false);
        } else
        {
            muralToShow.SetActive(true);
        }
    }
}
