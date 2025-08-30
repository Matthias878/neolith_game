using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;

public class Culture_Overview : MonoBehaviour
{

    private GameObject cultureCanvas;
    public Culture cultureToDisplay;
    public TextMeshProUGUI textPrefab; // prefab to add to canvasobject
    private bool gamestart = true;

    //how to show a different culture? defaults to players

    void OnDisable()
    {
        if (cultureCanvas != null)
        {
            foreach (Transform child in cultureCanvas.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void OnEnable()
    {
        cultureCanvas = gameObject;
        if (gamestart) { gamestart = false; return; } //skip first enable on game start
        cultureToDisplay = Controller.players[0].culture;
        //TODO get culture traits and visualize foreach (var trait in cultureToDisplay.traits)
        TextMeshProUGUI newText = Instantiate(textPrefab, cultureCanvas.transform);
        newText.text = cultureToDisplay.ToString(); // Display culture name
        Show_Knowledge();
    }

    //show knowledge
    public void Show_Knowledge()
    {
        TextMeshProUGUI headerText = Instantiate(textPrefab, cultureCanvas.transform);
        headerText.text = $"Knowledge known by the {cultureToDisplay.nominativPlural}:";
        foreach (var tech in cultureToDisplay.knownKnowledge)
        {
            TextMeshProUGUI newText = Instantiate(textPrefab, cultureCanvas.transform);
            newText.text = tech.returnDescription(cultureToDisplay);
        }
    }
    //show traits

}