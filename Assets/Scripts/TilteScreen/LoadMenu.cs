using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadMenu : MonoBehaviour
{
    public Button saveGamePrefab; // prefab for savegames
    private string savePath;
    public MainMenu mainMenu;
    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        foreach (var file in Directory.GetFiles(savePath, "*.json"))
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            Button saveButton = Instantiate(saveGamePrefab, transform);
            saveButton.gameObject.SetActive(true);
        
            // Set button text
            var textComponent = saveButton.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = fileName;
            }
        
            // Add click listener to load the game
            string filePath = file;
            saveButton.onClick.AddListener(() => LoadGame(filePath));
        
            // Hook up Delete button
            Transform deleteTransform = saveButton.transform.Find("Delete");
            if (deleteTransform != null)
            {
                Button deleteButton = deleteTransform.GetComponent<Button>();
                if (deleteButton != null)
                {
                    deleteButton.onClick.AddListener(() =>
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        Destroy(saveButton.gameObject);
                    });
                }
            }
        }
    }

    void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) //isPressed
        {
            mainMenu.CloseLoadMenu();
        }
    }
    public void LoadGame(string path)
    {
        StartupGame.entryMode = 1;
        StartupGame.savePath = path;
        SceneManager.LoadScene("Ingame");
    }   
}