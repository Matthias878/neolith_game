using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class CreateMenu : MonoBehaviour
{
    public ToggleGroup group;

    public TMPro.TMP_Text selectedNameText;

    public string GetSelectedName()
    {
        var activeToggle = group.ActiveToggles().FirstOrDefault();
        return activeToggle != null ? activeToggle.name : "None";
    }


    public void GenerateNewGame()
    {
        int t = 3;
        switch (GetSelectedName())
        {
            case "Small":
                t = 2;
                break;
            case "Medium":
                t = 3;
                break;
            case "Large":
                t = 4;
                break;
            default:
                Debug.LogWarning("No valid world size selected. using Medium");
                t = 3;
                break;
        }
        StartupGame.entryMode = t;
        StartupGame.savePath = "";
        SceneManager.LoadScene("Ingame");
    }

    void Update()
    {
        selectedNameText.text = "Current Selection: Size-" + GetSelectedName();
    }
}