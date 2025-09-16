
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Person : Movable
{
    // x and y are inherited from Game_Entity
    //private int age;
    //public Person Spouse;
    //leader Should not loop
    //public Person engaged_or_betrothed_promised;
    //public List<Person> Siblings;
    //public List<Person> Children;
    //public Person Father;
    //public Person Mother;
    public bool isAlive;
    public string name;
    public String information;
    public bool isSettlementLeader = false;
    public bool isUnitLeader = false;
    public Clan isClanLeader = null;



    public override void presentActions_and_Data()
    {

        Button newButtontwo = Controller_GameData.inputManagerController.addUIButton();
        newButtontwo.GetComponentInChildren<TMP_Text>().text = "Send Message.";
        newButtontwo.onClick.AddListener(() => sendMessage());
        move_starter();
    }

    public Person getClanLeader()
    {
        if (leader == null)
        {
            return this;
        }
        else
        {
            return leader;
        }
    }

    private void sendMessage()
    {
        Controller_GameData.inputManagerController.SetGameState(Input_Manager_State.Interactions_Window);
        Interactions.SendInteractionto(this);
        //Debug.Log("Trying to interact with " + name);
    }

    public Person(string name, Culture culture, int x, int y, Person leader = null)
        : base(6, x, y, leader)
    {
        this.maxMovePoints = 6;
        this.movePoints = maxMovePoints;
        this.health = 100;
        this.name = name;
        this.isAlive = true;
        this.culture = culture;
        if (leader != null && leader.leader != null)
        {
            throw new ArgumentException("The sworn chief cannot have their own sworn chief.");
        }
        this.information = " Information about " + name + ": -TODO-\n";
    }

    public void dies()
    {
        isAlive = false;
        information += "Died of natural causes.\n";
    }

    public override void Turnend()
    {
        movePoints = maxMovePoints; // Reset move points at the end of the turn
        //TODO recalculate clanshit
        //mones and stuff
    }

    public void receiveInteraction(int interactionId)
    {
        var interaction = Interactions.InteractionList.FirstOrDefault(i => i.id == interactionId);
        if (interaction != null)
        {
            Debug.Log(name + " received interaction (ID: " + interactionId + ") from " + interaction.sender.name + "\n");
        }
        else
        {
            Debug.LogWarning($"No interaction found with ID: {interactionId}");
        }
    }
}