using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArtifactInteraction : MonoBehaviour
{
    public bool needsOracleBoneFirst = true; // false for room 0 bc we don't need it then
    public static bool touchedOracleBone = false; // meaning, touched the oracle bone in room 1
    public static int artifactsDecoded = 0;
    private bool hasBeenCounted = false;
    public CanvasGroup muralGroup;
    public GameObject interactPrompt; // 'press E'
    public float fadeSpeed = 1.5f;
    private bool isPlayerInRange;
    private bool isMuralVisible = false;
    public Image charUI; // the image of the character

    void Start()
    {
        muralGroup.alpha = 0;
        muralGroup.gameObject.SetActive(false);
    }

    void Update()
    {
        if ((!needsOracleBoneFirst || touchedOracleBone) && isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            interactPrompt.SetActive(false);
            if (!isMuralVisible)
            {
                StopAllCoroutines(); // in case they spam E
                StartCoroutine(FadeMural(0, 1));
                isMuralVisible = true;
                charUI.gameObject.SetActive(true);

                if (!hasBeenCounted) 
                {
                    artifactsDecoded++;
                    hasBeenCounted = true; 
                    Debug.Log("Artifacts found: " + artifactsDecoded);
                }
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FadeMural(1, 0));
                isMuralVisible = false;
                charUI.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator FadeMural(float startAlpha, float endAlpha)
    {
        if (endAlpha > 0) muralGroup.gameObject.SetActive(true);

        float timer = 0;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime * fadeSpeed;
            muralGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer);
            yield return null;
        }

        if (endAlpha <= 0) muralGroup.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!needsOracleBoneFirst || touchedOracleBone)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactPrompt.SetActive(false);
            charUI.gameObject.SetActive(false);
            
            // hide mural if they walk away
            if (isMuralVisible)
            {
                StartCoroutine(FadeMural(1, 0));
                isMuralVisible = false;
            }
        }
    }
}