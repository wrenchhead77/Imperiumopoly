using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Environments : MonoBehaviour
{
    public soEnvironData so_EnvironData; // Reference to the ScriptableObject
    public Image wLargeImage; // The large image display
    public Transform grp_thumbnailParent; // Parent of the thumbnail buttons in the prefab
    public Sprite highlightSprite; // Shared highlight sprite for all buttons

    private int selectedEnvironmentIndex = 0; // Track the currently selected environment
    private List<Button> thumbnailButtons = new List<Button>(); // Store button references
    private List<Image> highlightImages = new List<Image>(); // Store highlight layer references

    private void Start()
    {
        ShowButtons();
    }

    public void ShowButtons()
    {
        thumbnailButtons.Clear();
        highlightImages.Clear();

        // Populate buttons and highlight images
        foreach (Transform child in grp_thumbnailParent)
        {
            Button button = child.GetComponent<Button>();
            Image highlightImage = child.Find("Highlight")?.GetComponent<Image>();

            if (button != null)
            {
                thumbnailButtons.Add(button);
                highlightImages.Add(highlightImage);

                // Assign the highlight sprite
                if (highlightImage != null)
                {
                    highlightImage.sprite = highlightSprite;
                    highlightImage.enabled = false; // Disable highlight initially
                }
            }
        }

        // Set up buttons and assign listeners
        for (int i = 0; i < thumbnailButtons.Count; i++)
        {
            var button = thumbnailButtons[i];
            Image thumbnailImage = button.transform.Find("Thumbnail").GetComponent<Image>();

            // Assign sprites
            thumbnailImage.sprite = so_EnvironData.environs[i]._thumbnail;

            int index = i; // Capture index to avoid closure issue in lambda
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnThumbnailClicked(index));
        }

        // Check PersistentGameData for the previously selected environment
        var selectedEnvironment = GameManager.Instance.GetSelectedEnvironment();
        if (selectedEnvironment != null)
        {
            // Find the index of the selected environment
            int selectedIndex = System.Array.IndexOf(so_EnvironData.environs, selectedEnvironment);
            if (selectedIndex >= 0)
            {
                UpdateHighlight(selectedIndex); // Highlight the previously selected thumbnail
                UpdateLargeImage(selectedIndex); // Update the large image
                return; // Exit as we've highlighted the selected environment
            }
        }

        // Highlight the first thumbnail by default if no environment was previously selected
        UpdateHighlight(0);
        UpdateLargeImage(0);
    }

    private void UpdateLargeImage(int index)
    {
        wLargeImage.sprite = so_EnvironData.environs[index]._largeImage;
    }

    public void OnThumbnailClicked(int index)
    {
        if (index == selectedEnvironmentIndex)
            return; // Prevent redundant selection

        UpdateHighlight(index); // Update the selected highlight
    }

    private void UpdateHighlight(int index)
    {
        // Deactivate all highlights
        for (int i = 0; i < highlightImages.Count; i++)
        {
            if (highlightImages[i] != null)
            {
                highlightImages[i].enabled = false;
            }
        }

        // Activate the highlight of the selected thumbnail
        if (highlightImages[index] != null)
        {
            highlightImages[index].enabled = true;
        }

        // Update the large image and the selected index
        wLargeImage.sprite = so_EnvironData.environs[index]._largeImage;
        selectedEnvironmentIndex = index;

        Debug.Log($"Selected Environment: {so_EnvironData.environs[index].environName}");
    }

    public void OnAcceptClick()
    {
        if (selectedEnvironmentIndex >= 0 && selectedEnvironmentIndex < so_EnvironData.environs.Length)
        {
            var selectedEnvironment = so_EnvironData.environs[selectedEnvironmentIndex];
            GameManager.Instance.SetSelectedEnvironment(selectedEnvironment);
            Debug.Log($"Accepted Environment: {selectedEnvironment.environName}");
        }
        else
        {
            Debug.LogError("Selected environment index is out of bounds or invalid.");
        }

        Destroy(this.gameObject); // Close the canvas
    }

    // Methods to manually activate highlights for specific buttons
    public void Environ0() => UpdateHighlight(0);
    public void Environ1() => UpdateHighlight(1);
    public void Environ2() => UpdateHighlight(2);
    public void Environ3() => UpdateHighlight(3);
    public void Environ4() => UpdateHighlight(4);
}
