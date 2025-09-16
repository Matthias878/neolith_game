using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;

public class Interactions : MonoBehaviour
{

    public static List<Interaction> InteractionList = new List<Interaction>();//interactions and their ID
    public static Interactions singleton;
    public static Person entity_to_interact_with;
    public Button buttonPrefab;
    public TextMeshProUGUI Guy_info_text;
    public static Controller controller;
    private void Awake()
    {
        singleton = this;
    }

    private static string[] interaction_forall = new string[] { 
        //Overall
        "Go_Back",
        "Manage_settlement", "Manage_unit", "Manage_person",
        "View_settlement", "View_unit", "View_person",
        //general            
        "Send_gift", "Arrange_marriage","Demand_tribute", "Request_aid","Assassinate",
        "Declare_war", "Send_positive_message", "Send_negative_message", "Negotiate_peace","Offer_non-aggression_pact",
        "Request_gift",  "Request_alliance", "Demand_hostage", "Request_non-aggression_pact",
        "Spy",
        //other
        "Open_journal", "Write_entry", "Read_entry_journal",
        "Open_encyclopedia", "Search_entry", "Read_entry_encyclopedia",
        "End_turn", "Next_turn", "Previous_turn",
    };
    private static string[] interaction_forvasselsettel = new string[] {
        "Set_tax_rate", "Build_structure",  "Set_trade_route", "Allow_own_trade_agreements",
        "Sabotage",
    };
    private static string[] interaction_forsettleclan = new string[] {
        "Offer_trade_agreement","Sabotage", "Demand_submission",
    };
    private static string[] interaction_forvasselsettelclan = new string[] {
        "Offer_trade_agreement","Sabotage",
    };
    private static string[] interaction_forvassalunit = new string[] {
        "Attack_settlement", "Attack_unit", "Attack_person", "Recruit_unit",
        "Set_rally_point", "Disband_unit", "Fortify_unit", "Train_unit",
        "Set_patrol_route",
    };
    private static string[] interaction_forunitclan = new string[] {
        "",
    };
    private static string[] interaction_forvassalunitclan = new string[] {
        "",
    };

    public static void SendInteractionto(Person entity)
    {
        entity_to_interact_with = entity;
        singleton.SendInteractionto();
        singleton.setLeftText();
    }

    private void setLeftText()
    {
        Guy_info_text.text = "This is: " + entity_to_interact_with.name + "\n" + entity_to_interact_with.information;
    }

    private void SendInteractionto()
    {
        controller = GameObject.Find("Script_Holder_Ingame").GetComponent<Controller>();
        clearUI();
        setup();

        addInteractions(interaction_forall);
        if (entity_to_interact_with.isClanLeader == null && entity_to_interact_with.leader == Controller.players[0])
        {//is your vassal
            if (entity_to_interact_with.isSettlementLeader)
            {
                //settlement leader 
                addInteractions(interaction_forvasselsettel);
            }
            else if (entity_to_interact_with.isUnitLeader)
            {
                //unit leader
                addInteractions(interaction_forvassalunit);
            }
            else
            {
                //some dude
            }
        }
        else if (entity_to_interact_with.isClanLeader == null)
        {// is someone else's vassal
            if (entity_to_interact_with.isSettlementLeader)
            {
                //settlement leader 
                addInteractions(interaction_forvasselsettelclan);
            }
            else if (entity_to_interact_with.isUnitLeader)
            {
                //unit leader 
                addInteractions(interaction_forvassalunitclan);
            }
            else
            {
                //some dude
            }
        }
        else
        {//is clan leader
            if (entity_to_interact_with.isSettlementLeader)
            {
                //settlement leader 
                addInteractions(interaction_forsettleclan);
            }
            else if (entity_to_interact_with.isUnitLeader)
            {
                //unit leader 
                addInteractions(interaction_forunitclan);
            }
            else
            {
                //some dude??
            }

        }


    }

    private void addInteractions(string[] interactionIdeas)
    {
        foreach (string idea in interactionIdeas)
        {
            Button btn = Instantiate(buttonPrefab, transform);
            // Reflection: find method with the name
            var method = this.GetType().GetMethod(idea,
                         System.Reflection.BindingFlags.Instance |
                         System.Reflection.BindingFlags.NonPublic);

            if (method != null)
            {
                btn.onClick.AddListener(() => method.Invoke(this, null));
            }
            else
            {
                Debug.Log("Interaction method not yet implemented, TODO see Interactions.cs");
            }
            LayoutElement layoutElement = btn.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 40f;
            TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            string newidea = idea.Replace('_', ' ');
            if (tmp != null)
                tmp.text = newidea;
            else if (btn.GetComponentInChildren<Text>() != null)
                btn.GetComponentInChildren<Text>().text = newidea;
            btn.name = newidea + " Button";
        }
    }

    private void clearUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private static void setup()
    {
        //Debug.Log("Setting up interaction for " + entity_to_interact_with.type + " at " + entity_to_interact_with.x + ", " + entity_to_interact_with.y + " led by " + (entity_to_interact_with.leader != null ? entity_to_interact_with.leader.name : "no one"));
        if (entity_to_interact_with.leader == null)
        {
            if (entity_to_interact_with is Person)
            {
                Person personToInteractWith = entity_to_interact_with as Person;
                entity_to_interact_with.isClanLeader = new Clan(personToInteractWith);
            }
            else
            {
                Debug.LogWarning("Entity should be a Person, but is not. Entity type: " + entity_to_interact_with.type + "x, y: " + entity_to_interact_with.x + ", " + entity_to_interact_with.y);
            }
        }
        else
        {
            foreach (var entity in Controller.movables)
            {
                if (entity.leader == entity_to_interact_with)
                {
                    if (entity is Settler_Unit)
                    {
                        entity_to_interact_with.isUnitLeader = true;
                    }
                    else if (entity is Settlement)
                    {
                        entity_to_interact_with.isSettlementLeader = true;
                    }
                }
            }
        }

    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) //isPressed
        {
            Go_Back();
        }
    }

    private void Go_Back()
    {
        controller.inputManagerController.SetGameState(Input_Manager_State.Base_Tile_Layer);
    }

    private void Manage_settlement()
    {
        Debug.LogWarning("trying to manage settlement.");
    }


    private void Send_gift()
    {
        Person p = (Person)Controller.players[0];
        var t = new Gift(p, entity_to_interact_with, 10);
        InteractionList.Add(t);
        Messenger.sendMessenger(Controller.players[0].x, Controller.players[0].y, entity_to_interact_with, t.id);
        Go_Back();
    }
}

//Demand tribute, request aid, declare war, propose alliance, arrange marriage, send message, trade agreement, negotiate peace, form coalition
//Spy, sabotage, assassinate, bribe, incite rebellion, spread propaganda, offer
//Visit settlement, visit camp, visit unit, visit person
//Offer gift, offer marriage, offer alliance, offer trade agreement, offer non-aggression pact
//Request gift, request marriage, request alliance, request trade agreement, request non-aggression pact
//Attack settlement, attack unit, attack person
//Recruit unit, recruit person
//Manage settlement, manage unit, manage person
//View settlement, view unit, view person
//Set rally point, disband unit, fortify unit
//Set patrol route, set trade route
//Set tax rate, set conscription rate, set production focus
//Build structure, train unit, recruit person
//View culture, view religion, view government, view laws
//View diplomacy, view wars, view alliances, view treaties
//View clan, view family, view notable people, view commoners
//View technology, view research, view resources
//View map, view terrain, view climate, view resources
//View objectives, view achievements, view statistics
//View help, view credits, view settings
//Save game, load game, quit game
//Pause game, resume game, fast forward game
//Zoom in, zoom out, rotate camera, reset camera
//Toggle grid, toggle fog of war, toggle resource icons, toggle unit icons
//Toggle settlement icons, toggle culture borders, toggle terrain types, toggle climate zones
//Toggle objectives, toggle achievements, toggle statistics, toggle help
//Toggle settings, toggle save/load menu, toggle pause menu, toggle main menu
//Exit to main menu, exit to desktop
//End turn, next turn, previous turn
//Switch player, switch faction, switch clan
//Open chat, send message, receive message
//Open journal, write entry, read entry
//Open encyclopedia, search entry, read entry



            //Left information
            //"View culture", "View religion", "View government", "View laws",
            //"View diplomacy", "View wars", "View alliances", "View treaties",
            //"View clan", "View family", "View notable people", "View commoners",
            //"View technology", "View research", "View resources",
            //"View map", "View terrain", "View climate", "View resources",
            //Player actions
            //"Switch player", "Switch faction", "Switch clan",
            //"Open chat", "Send message", "Receive message",

            //implement interaction, letter messenger, go yourself. tribute subjugation, release person, demand marriage, come to event(religous, normal) etc...
            //Demand conversion adopt religion, convert religion, change government, change laws, religious meeting?