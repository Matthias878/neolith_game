using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using UnityEngine.UI;


public class Input_Manager_Controller : MonoBehaviour
{

    //states can be changed from elsewhere, show what needs to be shown based on state not click?
    //destroy footsteps if not in state?
    //Settlement hitbox much bigger then unit hitbox?

    void Awake()
    {
        foodProdGenerator = GameObject.Find("GenerateFoodProd").GetComponent<GenerateFoodProd>();
        displayController = GameObject.Find("Script_Holder_Ingame")?.GetComponent<Controller>();
        if (canvasGameObject != null)
        {
            Transform infoTransform = canvasGameObject.transform.Find("Current_Information");
            if (infoTransform != null)
            {
                UI_Info = infoTransform.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("Current_Information child not found under canvasGameObject.");
            }
        }
        else
        {
            Debug.LogWarning("canvasGameObject reference is not set in the inspector.");
        }

        cultureWindow = GameObject.Find("Cultures_Window");
        mainCanvas = GameObject.Find("Main_Canvas");
        cultureWindow.SetActive(false);
    }
    //TODO integrate turn system
    //TODO right-click drag window


    private bool togglFoodProduction = false;
    private Controller displayController;
    private static Input_Manager_State currentState = Input_Manager_State.No_Input_Load;   //ENUM that contains all unity layers
    public TextMeshProUGUI GameStateInfoText; //TOP RIGHT 
    public GameObject canvasGameObject; //Big lower left ui
    private TextMeshProUGUI UI_Info; //part of canvas object
    public Button buttonPrefab; // prefab to add to canvasobject
    public TextMeshProUGUI textPrefab; // prefab to add to canvasobject

    public static void InputFail()
    {
        Debug.LogError("Input failed in state: " + currentState);
        //TODO
    }

    //DO smth on rightclick
    void Update()
    {

        GameStateInfoText.text = currentState.ToString();
        //Debug.Log("Tries to check");
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                int layerMask = LayerMask.GetMask(currentState.ToString());
                RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero, 0f, layerMask);
                if (hits.Length > 0)
                {
                    GameObject[] hitObjects = hits.Select(h => h.collider.gameObject).ToArray();
                    GameObject selected = CustomOrder(hitObjects);
                    if (selected != null)
                    {
                        DoStuff(selected);
                    }
                }
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                DoOtherStuff();
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                int layerMask = LayerMask.GetMask(currentState.ToString());
                RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero, 0f, layerMask);
                if (hits.Length > 0)
                {
                    GameObject[] hitObjects = hits.Select(h => h.collider.gameObject).ToArray();
                    GameObject selected = CustomOrder(hitObjects);
                    if (selected != null)
                    {
                        DoOtherStuff(selected);
                    }
                }
            }
        }
    }


    private GameObject CustomOrder(GameObject[] objects)
    {
        // Priority: unit > settlement > tile > other
        foreach (var obj in objects)
        {
            if (obj.TryGetComponent<Game_Entity_Component>(out _))
                return obj;
        }
        foreach (var obj in objects)
        {
            if (obj.TryGetComponent<Settlement_Component>(out _))
                return obj;
        }
        foreach (var obj in objects)
        {
            if (obj.TryGetComponent<HexTileComponent>(out _))
                return obj;
        }
        Debug.LogWarning("No valid GameObject found in CustomOrder.");
        if (objects.Length > 0)
            return objects[0];
        return null;
    }

    //the player has right-clicked
    void DoOtherStuff()
    {
        if (currentState == Input_Manager_State.Unit_Movement_Layer)
        {
            //Debug.Log("Right clicked in Unit Movement Layer");
            Clear_all_Footsteps(); //Movement was cancelled
            SetGameState(Input_Manager_State.Base_Tile_Layer);
        }
        else if (currentState == Input_Manager_State.Settlement_Management_Layer)
        {
            //Debug.Log("Right clicked in Settlement Management Layer");
            SetGameState(Input_Manager_State.Base_Tile_Layer);
        }
    }

    void DoOtherStuff(GameObject a)
    {
        if (currentState == Input_Manager_State.Unit_Movement_Layer)
        {
            //Debug.Log("Right clicked in Unit Movement Layer");
            Clear_all_Footsteps(); //Movement was cancelled
            SetGameState(Input_Manager_State.Base_Tile_Layer);
        }

    }

    //the player has left-clicked on a gameobject
    void DoStuff(GameObject a)
    {
        if (currentState == Input_Manager_State.Base_Tile_Layer)
        {
            //Debug.Log("Clicked on GameObject in Base Tile Layer");
            MonoBehaviour[] scripts = a.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                //The clicked GameObject has a unit attached -> unit should be moved //TODO other actions
                if (script is Game_Entity_Component gameEntityComp)
                { //TODO check script is unit
                    //Debug.Log("Unit type: " + gameEntityComp.game_Entity.type);
                    SetGameState(Input_Manager_State.Unit_Movement_Layer);
                    gameEntityComp.game_Entity.move_starter();//create and displays all options for the user
                    gameEntityComp.game_Entity.presentActions_and_Data(); // Show actions available for the unit
                }
                else if (script is Settlement_Component settlementComp)
                {
                    SetGameState(Input_Manager_State.Settlement_Management_Layer);
                    settlementComp.settlement.presentActions_and_Data(); // Show actions available for the settlement
                }
                else if (script is HexTileComponent hexTileComp)
                {
                    Debug.Log(hexTileComp.hexTileInfo.ToString());
                }
                else
                {
                    Debug.Log("Error: Clicked on unrecognised GameObject: ");
                }
            }
            //Debug.Log("Finished processing GameObject in Base Tile Layer");
        }
        else if (currentState == Input_Manager_State.Unit_Movement_Layer)
        {
            MonoBehaviour[] scripts = a.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                //The clicked GameObject has a unit attached -> unit should be moved //TODO other actions
                if (script is Game_Entity_Component gameEntityComp)
                {//check what type the game_entitiy is
                    //Debug.Log("Game Entity type: " + gameEntityComp.game_Entity.type);
                    gameEntityComp.game_Entity.move_starter(); //moves the unit from the footstep function
                    Clear_all_Footsteps(); // Clear all footsteps after moving
                    SetGameState(Input_Manager_State.Base_Tile_Layer);
                }
            }
        }
        else if (currentState == Input_Manager_State.Game_Entity_Layer)
        {
        }
    }

    private GenerateFoodProd foodProdGenerator; //Done in Awake
    public void ToggleFoodProduction()
    {
        if (togglFoodProduction == false)
            foodProdGenerator.Show_food_prod_on_map(displayController);
        else
            foodProdGenerator.deactivate_food_prod();
        togglFoodProduction = !togglFoodProduction;
    }

    private void Clear_all_Footsteps()
    {

        if (displayController != null && displayController.movables != null)
        {
            displayController.movables
                .Where(e => (e is Footsteps))
                .ToList()
                .ForEach(e => displayController.AddEntity_toremove(e));
        }
    }
    public void SetGameState(Input_Manager_State newState)
    {
        leaveState(currentState);
        switch (newState)
        {
            case Input_Manager_State.No_Input_Load:
                UI_Info.text = "Game does currently not accept input, is loading...";
                break;
            case Input_Manager_State.Base_Tile_Layer:
                UI_Info.text = "You have not selected anything, click on your units or cities to control them.";
                break;
            case Input_Manager_State.Unit_Movement_Layer:
                UI_Info.text = "You have selected a unit, click on a destination tile to move it or choose an action to the right of this field.";
                break;
            case Input_Manager_State.Settlement_Management_Layer:
                UI_Info.text = "You have selected a settlement, manage its production and resources.";
                break;
            case Input_Manager_State.Game_Entity_Layer:
                UI_Info.text = "You have selected a game entity, interact with it.";
                break;
            default:
                Debug.LogWarning("Unknown game state: " + newState);
                break;
                //return; // Exit if the new state is unknown
        }
        currentState = newState;
        foreach (Transform child in canvasGameObject.transform)
        {
            if (child.GetComponent<TextMeshProUGUI>() != UI_Info)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void leaveState(Input_Manager_State oldState)
    {
        //logic
        Clear_all_Footsteps();
    }

    public Button addUIButton()
    {
        Button newButton = Instantiate(buttonPrefab, canvasGameObject.transform);
        return newButton;
    }

    public TextMeshProUGUI addUIText()
    {
        TextMeshProUGUI newText = Instantiate(textPrefab, canvasGameObject.transform);
        return newText;
    }

    public Input_Manager_State GetCurrentState()
    {
        return currentState;
    }

    private GameObject cultureWindow;private GameObject mainCanvas; //Set in the Awake function
    public void OpenCultureWindow(){SetGameState(Input_Manager_State.Cultures_Window);cultureWindow.SetActive(true);mainCanvas.SetActive(false);}public void CloseCultureWindow(){SetGameState(Input_Manager_State.Base_Tile_Layer);cultureWindow.SetActive(false);mainCanvas.SetActive(true);}

}

public enum Input_Manager_State //Different Scenes?
{
    Main_Menu, //?
    Base_Tile_Layer,// Game_Map_Overview, //Base_Tile_Layer
    Diplomacy_Window,
    Game_Entity_Layer, //Entity_Selection, //Game_Entity_Layer
    Unit_Movement_Layer,
    Settlement_Management_Layer,
    Pause_Menu,
    Settlement_Management,
    Character_Selection,
    Campaign_Manager,
    Battle,
    No_Input_Load,
    Cultures_Window
}   
        