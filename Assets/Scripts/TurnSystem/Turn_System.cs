using UnityEngine;
using TMPro;

public class Turn_System : MonoBehaviour
{
    public TextMeshProUGUI turnbuttontext;
    private int currentTurn = 1;

    public Display_Controller displayController;

    public void EndTurn()
    {
        currentTurn++;
        turnbuttontext.text = $"Current Turn: {currentTurn}, Click here to End Turn";

        // Call TurnEnd on all movables
        if (displayController != null && displayController.movables != null)
        {
            foreach (var entity in displayController.movables)
            {
                if (entity != null)
                {
                    entity.Turnend();
                }
            }
        }
    }
}
