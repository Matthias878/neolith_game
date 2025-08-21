using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;

public class Input_Manager_Controller : MonoBehaviour
{
    //TODO integrate turn system
    public TextMeshProUGUI GameStateInfoText;
    private static Input_Manager_State currentState = Input_Manager_State.No_Input_Load;   //ENUM that contains all unity layers

    public Display_Controller displayController;

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
                //Debug.Log("mouse was clicked");
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                int layerMask = LayerMask.GetMask(currentState.ToString());
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, layerMask);

                if (hit.collider != null)
                {
                    Debug.Log("a GameObject was hit (left click)");
                    DoStuff(hit.collider.gameObject);
                }
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                //Debug.Log("right mouse was clicked");
                DoOtherStuff();
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                int layerMask = LayerMask.GetMask(currentState.ToString());
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, layerMask);

                if (hit.collider != null)
                {
                    Debug.Log("a GameObject was hit (right click)");
                    DoOtherStuff(hit.collider.gameObject);
                }
            }
        }
    }

    void DoOtherStuff()
    {
        if (currentState == Input_Manager_State.Unit_Movement_Layer)
        {
            //Debug.Log("Right clicked in Unit Movement Layer");
            Clear_all_Footsteps(); //Movement was cancelled
            SetGameState(Input_Manager_State.Base_Tile_Layer);
        }
        //continues to try to call with gameobject
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

    //the player has clicked on a gameobject
    void DoStuff(GameObject a)
    {
        if (currentState == Input_Manager_State.Base_Tile_Layer)
        {
            Debug.Log("Clicked on GameObject in Base Tile Layer");
            MonoBehaviour[] scripts = a.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                //The clicked GameObject has a unit attached -> unit should be moved //TODO other actions
                if (script is Game_Entity_Component gameEntityComp)
                { //TODO check script is unit
                    //Debug.Log("Unit type: " + gameEntityComp.game_Entity.type);
                    SetGameState(Input_Manager_State.Unit_Movement_Layer);
                    gameEntityComp.game_Entity.move();//create and displays all options for the user
                }else {
                    Debug.Log("Non-unit GameObject clicked: " + a.name);
                }
            }
            Debug.Log("Finished processing GameObject in Base Tile Layer");
        }
        else if (currentState == Input_Manager_State.Unit_Movement_Layer)
        {
            MonoBehaviour[] scripts = a.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                //The clicked GameObject has a unit attached -> unit should be moved //TODO other actions
                if (script is Game_Entity_Component gameEntityComp)
                {//check what type the game_entitiy is
                    Debug.Log("Game Entity type: " + gameEntityComp.game_Entity.type);
                    gameEntityComp.game_Entity.move();
                    Clear_all_Footsteps(); // Clear all footsteps after moving
                    SetGameState(Input_Manager_State.Base_Tile_Layer);
                }
            }
        }
        else if (currentState == Input_Manager_State.Game_Entity_Layer)
        {
        }//...
    }


    private void Clear_all_Footsteps()
    {
        if (displayController != null && displayController.movables != null)
        {
            displayController.movables = displayController.movables
                .Where(e => !(e is Footsteps))
                .ToArray();
        }
    }
    public void SetGameState(Input_Manager_State newState)
    {
        currentState = newState;
    }
}