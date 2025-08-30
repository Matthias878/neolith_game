using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{//TODO allow renaming/deleting save files, grouping by playtrough

    //public Controller controller;
    //public My_TilemapRenderer tilemapRenderer;
    public Button saveGamePrefab; // prefab for savegames

    private string savePath;

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
        }
    }


    public void LoadGame(string path)
    {
        StartupGame.entryMode = 1;
        StartupGame.savePath = path;
        SceneManager.LoadScene("Ingame");
        
        //controller.SetGameMap(state.gameMap);
        //controller.movables = state.movables;
        //controller.people = state.people;
        //controller.players = state.players;

        // Update the tilemap renderer with the new game state
        //tilemapRenderer.StartRendering(controller.GameMap, controller.movables);

        // Unpause the game after loading
        //Time.timeScale = 1f;
        // Assuming there's a method or property to set the paused state
        // For example, if you have a GameManager class, you might do:
        // GameManager.Instance.isPaused = false;
    }   
}