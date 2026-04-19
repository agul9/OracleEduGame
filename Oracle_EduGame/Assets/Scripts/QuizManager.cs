using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RoomData { 
    public Sprite[] choiceSprites; // All available images for this room
    public Sprite[] blankSprites;  // The specific "Oracle Bone" images that float above the blanks
}

public class QuizManager : MonoBehaviour
{
    public Sprite selectedSprite;
    public GameObject selectedButton;
    public int selectedID = -1;

    public RoomData[] rooms; // Set size to 2 in the Inspector
    public Image[] slotCharacterImages; // Drag the Image components that sit ABOVE the blank buttons here
    public GameObject[] choiceButtons; // Drag your 16 choice buttons here
    public GameObject[] blankSlots;    // Drag your 4 blank buttons here
    int currentRoom = 1;

    void Start()
{
    // This forces the game to set up Room 0 (the 4-button version) 
    // the moment you hit Play.
    SetupRoom(currentRoom);
}

    public void SelectChoice(GameObject clickedBtn)
    {
        // Reset old button opacity
        if (selectedButton != null) SetOpacity(selectedButton, 1.0f);

        selectedButton = clickedBtn;
        
        // TRICK: Get the ID from the name (e.g., "Choice_3" becomes 3)
        string[] nameParts = clickedBtn.name.Split('_');
        selectedID = int.Parse(nameParts[1]);

        selectedSprite = clickedBtn.GetComponent<Image>().sprite;

        // Visual feedback
        SetOpacity(selectedButton, 0.5f); 
    }

    public void PlaceInBlank(GameObject blankSlot)
    {
        if (selectedButton != null)
        {
            // Get the ID from the blank's name (e.g., "Blank_3")
            string[] nameParts = blankSlot.name.Split('_');
            int slotID = int.Parse(nameParts[1]);

            // Place the image
            Image slotImage = blankSlot.GetComponent<Image>();
            slotImage.sprite = selectedSprite;
            slotImage.color = Color.white; 

            // Feedback
            SetOpacity(selectedButton, 0.2f);
            selectedButton.GetComponent<Button>().interactable = false;

            // Check if it's the right spot
            if (selectedID == slotID) {
                Debug.Log("Correct placement for artifact " + slotID);
            } else {
                Debug.Log("Wrong spot!");
            }

            // Reset selection
            selectedButton = null;
            selectedID = -1;
        }
    }

    void SetOpacity(GameObject obj, float alpha)
    {
        Image img = obj.GetComponent<Image>();
        if (img != null) {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }

    public void SetupRoom(int roomIndex)
    {
        currentRoom = roomIndex;
        RoomData data = rooms[roomIndex];

        // 1. Setup Choices
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < data.choiceSprites.Length)
            {
                choiceButtons[i].SetActive(true);
                choiceButtons[i].GetComponent<Image>().sprite = data.choiceSprites[i];
                choiceButtons[i].GetComponent<Button>().interactable = true;
                SetOpacity(choiceButtons[i], 1.0f); // Reset visibility
            }
            else
            {
                choiceButtons[i].SetActive(false);
            }
        }

        // 2. Setup Blanks & Floating Characters
for (int i = 0; i < blankSlots.Length; i++)
{
    if (blankSlots[i] == null) continue; // Safety skip

    if (i < data.blankSprites.Length)
    {
        blankSlots[i].SetActive(true);
        
        // Safety check for the character image frames
        if (i < slotCharacterImages.Length && slotCharacterImages[i] != null)
        {
            slotCharacterImages[i].gameObject.SetActive(true);
            slotCharacterImages[i].sprite = data.blankSprites[i];
            slotCharacterImages[i].color = Color.white;
        }

        // Reset Blank Slot
        Image blankImg = blankSlots[i].GetComponent<Image>();
        if (blankImg != null) {
            blankImg.sprite = null; 
            blankImg.color = new Color(1, 1, 1, 0.1f);
        }
    }
    else
    {
        blankSlots[i].SetActive(false);
        if (i < slotCharacterImages.Length && slotCharacterImages[i] != null)
            slotCharacterImages[i].gameObject.SetActive(false);
    }
}
    }
}