using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Input_Manager_Controller : MonoBehaviour
{

    //states can be changed from elsewhere, show what needs to be shown based on state not click?
    //destroy footsteps if not in state?
    //Settlement hitbox much bigger then unit hitbox?

    void Awake()
    {
        foodProdGenerator = GameObject.Find("GenerateFoodProd").GetComponent<GenerateFoodProd>();
        controller = GameObject.Find("Script_Holder_Ingame")?.GetComponent<Controller>();
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

        mainCanvas = GameObject.Find("Main_Canvas");
        cultureWindow = GameObject.Find("Cultures_Window");
        cultureWindow.SetActive(false);
        peoplesWindow = GameObject.Find("Peoples_Window");
        peoplesWindow.SetActive(false);
        interactionWindow = GameObject.Find("Interactions_Menu");
        interactionWindow.SetActive(false);
        filenameInput = GameObject.Find("Input_Save_Name").GetComponent<TMP_InputField>();
        escapeMenu = GameObject.Find("Escape_Menu");
        escapeMenu.SetActive(false);
    }
    //TODO integrate turn system
    //TODO right-click drag window

    private bool togglFoodProduction = false;
    private Controller controller;
    private static Input_Manager_State currentState = Input_Manager_State.No_Input_Load;   //ENUM that contains all unity layers
    public TextMeshProUGUI GameStateInfoText; //TOP RIGHT 
    public GameObject canvasGameObject; //Big lower left ui
    private TextMeshProUGUI UI_Info; //text in canvas object
    public Button buttonPrefab; // prefab to add to canvasobject
    public TextMeshProUGUI textPrefab; // prefab to add to canvasobject
    public TextMeshProUGUI endturnbutton; private int currentTurn = 1;

    void Update() //check gamestate here?
    {
        GameStateInfoText.text = currentState.ToString(); //TODO remove
        //dont move camera if in windows?
        if (currentState == Input_Manager_State.Escape_Menu ||
            currentState == Input_Manager_State.Cultures_Window ||
            currentState == Input_Manager_State.Diplomacy_Window ||
            currentState == Input_Manager_State.Peoples_Window ||
            currentState == Input_Manager_State.Pause_Menu ||
            currentState == Input_Manager_State.Interactions_Window)
        { }
        else { moveCamera(); }
        checkmouseInput();
        checkKeyboardInput();
    }

    //Values for camera movement
    private float moveSpeed = 30f; private float scrollSpeed = 5f; private float minOrthoSize = 2f; private float maxOrthoSize = 20f; public Camera cam; private Vector2 moveInput; private float scrollInput;
    private void moveCamera()
    {
        // WASD and Arrow Key Movement using new Input System
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput.x += 1;
        }
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        float scaledSpeed = moveSpeed * cam.orthographicSize / 5f; // 5f is your default zoom
        cam.transform.position += move * scaledSpeed * Time.deltaTime;

        // Scroll to zoom in/out using new Input System
        scrollInput = 0f;
        if (Mouse.current != null)
        {
            scrollInput = Mouse.current.scroll.ReadValue().y;
        }
        if (scrollInput != 0f && cam.orthographic)
        {
            cam.orthographicSize -= scrollInput * scrollSpeed * 0.1f; // 0.1f to match old scroll sensitivity
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoSize, maxOrthoSize);
        }
    }

    //escape key to open menu, enter to end turn
    private void checkKeyboardInput()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) //isPressed
        {
            if (currentState == Input_Manager_State.Escape_Menu)
            {
                SetGameState(Input_Manager_State.Base_Tile_Layer);
                return;
            }
            else //what if in some other menu?
            {
                SetGameState(Input_Manager_State.Escape_Menu);
            }
        }
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame) //isPressed TODO can hold
        {
            EndTurn();
        }
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame) //isPressed
        {
            goToplayer();
            controller.Outline();
        }

    }

    //right click deselect, left click select
    private void checkmouseInput()
    {
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
                        Left_Clicked_Object(selected);
                    }
                }
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Deselect_right_click();
            }
            //{
            //    DoOtherStuff();
            //    Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            //    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            //    int layerMask = LayerMask.GetMask(currentState.ToString());
            //    RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero, 0f, layerMask);
            //    if (hits.Length > 0)
            //    {
            //        GameObject[] hitObjects = hits.Select(h => h.collider.gameObject).ToArray();
            //        GameObject selected = CustomOrder(hitObjects);
            //        if (selected != null)
            //        {
            //            DoOtherStuff(selected);
            //        }
            //    }
            //}
        }
    }

    private void goToplayer()
    {
        var t = Controller.players[0];
        (float,float) r = My_TilemapRenderer.TileToWorld(t.x, t.y);
        cam.transform.position = new Vector3(r.Item1, r.Item2, cam.transform.position.z);

    }

    //if multiple objects hit, choose which one to use - helper function
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

    //the player has left-clicked on a gameobject
    void Left_Clicked_Object(GameObject a)
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
                    //gameEntityComp.game_Entity.move_starter();//create and displays all options for the user
                    gameEntityComp.game_Entity.presentActions_and_Data(); // Show actions available for the unit
                }
                else if (script is Settlement_Component settlementComp)
                {
                    SetGameState(Input_Manager_State.Settlement_Management_Layer);
                    settlementComp.settlement.presentActions_and_Data(); // Show actions available for the settlement
                }
                else if (script is HexTileComponent hexTileComp)
                {
                    //Debug.Log(hexTileComp.hexTileInfo.ToString());
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
                    gameEntityComp.game_Entity.presentActions_and_Data(); //moves the unit from the footstep function
                    Clear_all_Footsteps(); // Clear all footsteps after moving
                    SetGameState(Input_Manager_State.Base_Tile_Layer);
                }
            }
        }
        else if (currentState == Input_Manager_State.Game_Entity_Layer)
        {
        }
    }

    //the player has right-clicked
    void Deselect_right_click()
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

    private void Clear_all_Footsteps()
    {

        if (controller != null && Controller.movables != null)
        {
            Controller.movables
                .Where(e => (e is Footsteps))
                .ToList()
                .ForEach(e => controller.AddEntity_toremove(e));
        }
    }

    //------------------------------Direct User Input--------------------------------

    //Add/Remove UI elements to the general user interface (lower left)
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

    public void clearUIElements()
    {
        foreach (Transform child in canvasGameObject.transform)
        {
            if (child.GetComponent<TextMeshProUGUI>() != UI_Info)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void EndTurn()
    {
        currentTurn++;
        endturnbutton.text = $"Current Turn: {currentTurn}. Click this button, or press Enter for next Turn.";
        SetGameState(Input_Manager_State.No_Input_Load);
        // Call TurnEnd on all movables
        if (controller != null && Controller.movables != null)
        {
            foreach (var entity in Controller.movables)
            {
                if (entity != null)
                {
                    entity.Turnend();
                }
            }
        }
        SetGameState(Input_Manager_State.Base_Tile_Layer);
    }

    //---------------------------Explicit Other Windows-------------------------------
    private GenerateFoodProd foodProdGenerator; //Done in Awake
    public void ToggleFoodProduction()
    {
        if (togglFoodProduction == false)
            foodProdGenerator.Show_food_prod_on_map(controller);
        else
            foodProdGenerator.deactivate_food_prod();
        togglFoodProduction = !togglFoodProduction;
    }

    public static bool messengersneverRendered = true;
    public void RenderMessengers()
    {
        messengersneverRendered = !messengersneverRendered;
        foreach (var entity in Controller.movables)
        {
            if (entity is Messenger messenger)
            {
                messenger.haschanged_render = true; // Mark for rendering

                if (messengersneverRendered) {
                    messenger.neverRender = true;
                } else {
                    messenger.neverRender = false;
                }
            }
        }
    }

    //Cavases
    private GameObject cultureWindow; private GameObject mainCanvas; private GameObject peoplesWindow; private GameObject escapeMenu;  private GameObject interactionWindow;//Set in the Awake function//Set in the Awake function all canvases
    //Culture Window
    public void OpenCultureWindow() { SetGameState(Input_Manager_State.Cultures_Window); }
    public void CloseCultureWindow() { SetGameState(Input_Manager_State.Base_Tile_Layer); }
    // Family window
    public void OpenPeoplesWindow() { SetGameState(Input_Manager_State.Peoples_Window); }
    public void ClosePeoplesWindow() { SetGameState(Input_Manager_State.Base_Tile_Layer); }

    private TMPro.TMP_InputField filenameInput;//savegamename input field
    public void SaveGame()
    {
        string name = filenameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            name = "no_name_provided";
            filenameInput.text = name;
        }
        controller.SaveCurrentGame(name + ".json");
    }

    public void LeaveToMenu()
    {
        //clean, -> remove all statics
        SceneManager.LoadScene("Main_Menu");
    }
    
    public void OpenFeedback()
    {
        Application.OpenURL("https://forums.wesnoth.org/viewtopic.php?t=57032");
    }

    public void LeaveToDesktop()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    //---------------------------State Management Functions--------------------------------
    public void SetGameState(Input_Manager_State newState)
    {
        leaveState(currentState);
        switch (newState)
        {
            case Input_Manager_State.No_Input_Load:
                UI_Info.text = "Game does currently not accept input, is loading...";
                break;
            case Input_Manager_State.Base_Tile_Layer:
                UI_Info.text = "You have not selected anything, click on your units or cities to control them. Press 'M' to center on yourself.";
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
            case Input_Manager_State.Cultures_Window:
                cultureWindow.SetActive(true); mainCanvas.SetActive(false);
                UI_Info.text = "You have opened the cultures window, manage your cultures here.";
                break;
            case Input_Manager_State.Peoples_Window:
                peoplesWindow.SetActive(true); mainCanvas.SetActive(false);
                UI_Info.text = "You have opened the peoples window, manage your people here.";
                break;
            case Input_Manager_State.Escape_Menu:
                escapeMenu.SetActive(true); mainCanvas.SetActive(false);
                UI_Info.text = "You have opened the escape menu, manage your game here.";
                break;
            case Input_Manager_State.Interactions_Window:
                interactionWindow.SetActive(true); mainCanvas.SetActive(false);
                UI_Info.text = "You have opened the interactions window, manage your interactions here.";
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
        switch (oldState)
        {
            case Input_Manager_State.Cultures_Window:
                cultureWindow.SetActive(false);
                mainCanvas.SetActive(true);
                break;
            case Input_Manager_State.Peoples_Window:
                peoplesWindow.SetActive(false);
                mainCanvas.SetActive(true);
                break;
            case Input_Manager_State.Escape_Menu:
                escapeMenu.SetActive(false);
                mainCanvas.SetActive(true);
                break;
            case Input_Manager_State.Interactions_Window:
                interactionWindow.SetActive(false);
                mainCanvas.SetActive(true);
                break;
        }
        //logic
        Clear_all_Footsteps();
    }

    public Input_Manager_State GetCurrentState()
    {
        return currentState;
    }
}
public enum Input_Manager_State //Different Scenes?
{
    Base_Tile_Layer,    // Game_Map_Overview
    Unit_Movement_Layer, //Movables movement
    Settlement_Management_Layer, //Settlement management
    Game_Entity_Layer, //Entity_Selection TODO
    Pause_Menu, //TODO?
    Character_Selection, //TODO
    Campaign_Manager, //TODO
    Battle, //TODO
    Escape_Menu, //Menu
    No_Input_Load, //Error?
    Interactions_Window, //Menu
    Cultures_Window, //Menu
    Peoples_Window, //Menu
    Diplomacy_Window, //TODO
}   
        