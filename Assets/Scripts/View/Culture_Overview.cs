using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Culture_Overview : MonoBehaviour
{

    private GameObject cultureCanvas;
    public Culture cultureToDisplay;
    public TextMeshProUGUI textPrefab; // prefab to add to canvasobject

    //how to show a different culture? defaults to players

    void OnEnable()
    {
        cultureCanvas = GameObject.Find("CultureOverview");
        cultureToDisplay = GameObject.Find("Script_Holder_Ingame").GetComponent<Controller>().players[0].culture;
        //TODO get culture traits and visualize foreach (var trait in cultureToDisplay.traits)
        TextMeshProUGUI newText = Instantiate(textPrefab, cultureCanvas.transform);
        newText.text = cultureToDisplay.ToString(); // Display culture name
    }

}