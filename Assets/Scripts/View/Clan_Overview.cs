using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Clan_Overview : MonoBehaviour
{

    private List<Clan> clansList = new List<Clan>(); //all cultures

    private GameObject clanCanvas;
    public Culture cultureToDisplay;
    public TextMeshProUGUI textPrefab; // prefab to add to canvasobject

    //private Controller controller;

    private bool gamestart = true;


    //how to show a different culture? defaults to players

    void OnEnable()
    {
        //controller = GameObject.Find("Script_Holder_Ingame").GetComponent<Controller>();
        if (gamestart) { gamestart = false; return; } //skip first enable on game start
        clanCanvas = gameObject;
        foreach (Transform child in clanCanvas.transform)
        {
            Destroy(child.gameObject);
        }
        cultureToDisplay = Controller.players[0].culture;
    }


    void Update()
    {
        List<Person> oldLeaders = new List<Person>();
        foreach (var clan in clansList)
        {
            if (clan.Leader.swornChief == null)
            {
                oldLeaders.Add(clan.Leader);
            }
        }

        Game_Entity[] movables = Controller.movables;
        Person[] people = Controller.people;

        List<Person> newLeaders = new List<Person>();

        if (movables != null)
        {
            foreach (var entity in movables)
            {
                if (entity.leader.swornChief == null && !newLeaders.Contains(entity.leader))
                {
                    newLeaders.Add(entity.leader);
                }
            }
        }

        if (people != null)
        {
            foreach (var entity in people)
            {
                if (entity.swornChief == null && !newLeaders.Contains(entity))
                {
                    newLeaders.Add(entity); //Wanderers not clanleaders?
                }
            }
        }

        //TODO handle people that had a clan and now no longer have one
        List<Person> beenLeaders = new List<Person>();

        //TODO handle people that have a clan now and did not have one before
        List<Person> becameLeaders = new List<Person>();

        //TODO handle people that had a clan before and have one now
        List<Person> stillLeaders = new List<Person>();


        foreach (var person in oldLeaders)
        {
            if (!newLeaders.Contains(person))
            {
                beenLeaders.Add(person);
            }
            else
            {
                stillLeaders.Add(person);
                newLeaders.Remove(person);
            }
        }
        becameLeaders = newLeaders;

        clansList.Clear();

        foreach (var person in becameLeaders)
        {
            clansList.Add(new Clan(person.name + "'s Clan", person));
        }

        foreach (var person in stillLeaders)
        {
            clansList.Add(new Clan(person.name + "'s Clan", person));
        }

        foreach (Transform child in clanCanvas.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var clan in clansList)
        {
            TextMeshProUGUI newText = Instantiate(textPrefab, clanCanvas.transform);
            newText.text = clan.ToString();
        }

    }

}