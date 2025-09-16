using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Settler_Unit : Movable
{
    public Settler_Unit(int x, int y, Person leader) : base(1, x, y, leader)
    {
        this.maxMovePoints = 6;
        this.movePoints = maxMovePoints;
        this.health = 100;
    }

    public override void presentActions_and_Data()
    {

        Button newButton = Controller_GameData.inputManagerController.addUIButton();
        newButton.GetComponentInChildren<TMP_Text>().text = "Click this button to found a new settlement.";
        newButton.onClick.AddListener(() => Settlement.foundSettlement(x, y, this.leader));


        Button newButtontwo = Controller_GameData.inputManagerController.addUIButton();
        newButtontwo.GetComponentInChildren<TMP_Text>().text = "fortify here.";
        newButtontwo.onClick.AddListener(() => fortify());
        move_starter();

    }

    private void fortify()
    {
        Controller_GameData.inputManagerController.SetGameState(Input_Manager_State.Settlement_Management_Layer);
        Debug.Log("Fortifying at position: " + x + ", " + y);
    }

    private void buildSettlement()
    {
        // Implement settlement building logic here
    }

        public override void Turnend()
    {
        movePoints = maxMovePoints; // Reset move points at the end of the turn
    }
}
