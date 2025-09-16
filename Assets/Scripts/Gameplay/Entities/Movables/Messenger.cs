using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;


public class Messenger : Movable
{
    public Messenger(int x, int y, Person leader) : base(7, x, y, leader)
    {
        neverRender = Input_Manager_Controller.messengersneverRendered;
        haschanged_render = true;
        this.maxMovePoints = 3;
        this.movePoints = maxMovePoints;
        this.health = 12;
    }

    public int x_destination;
    public int y_destination;

    public int InteractionId; // To track which interaction this messenger is delivering

    public Person recipient;

    private bool isonhorse = false;

    public override void presentActions_and_Data()
    {
        //TODO?

    }
    
    private bool checkIfOntarget()
    {
        if (x == x_destination && y == y_destination)
        {
            //Debug.Log("Messenger has arrived at destination (" + x + "," + y + "). Delivering message: " + message + "\n");
            // Here you can implement logic to deliver the message to the recipient
            // For example, you might want to notify the recipient or trigger an event
            isOntarget();
            return true;
        }
        return false;
    }

    private void isOntarget()
    {
        foreach (var entity in Controller.movables)
        {
            if (entity is Person person && person.x == x_destination && person.y == y_destination)
            {
                Debug.Log("Messenger delivered interaction (ID: " + InteractionId + ") to " + person.name + "\n");
                person.receiveInteraction(InteractionId);
                // Here you can implement logic to notify the recipient
                // For example, you might want to add the message to the recipient's inbox or trigger an event
                // After delivering the message, you might want to remove the messenger from the game
                Controller_GameData.AddEntity_toremove(this);
                return;
            }
        }
        Debug.Log("TODO WHAT TO DO NOW- return? No recipient found at destination (" + x_destination + "," + y_destination + "). Message not delivered.\n");
    }

    public static Messenger sendMessenger(int current_x, int current_y, Person recipient, int InteractionId)
    {
        Messenger messenger = new Messenger(current_x, current_y, null);
        messenger.x_destination = recipient.x;
        messenger.y_destination = recipient.y;
        messenger.InteractionId = InteractionId;
        Controller_GameData.AddEntity(messenger);
        messenger.AutoMove();
        return messenger;
    }

    public override void Turnend()
    {
        AutoMove();
        movePoints = maxMovePoints; // Reset move points at the end of the turn
    }

    public void AutoMove()
    {
        //Debug.Log("Messenger at " + x + "," + y + " moving towards " + x_destination + "," + y_destination + " with " + movePoints + " move points left. Sending message: " + message + ".\n");

        if (movePoints <= 0) return;

        // Use the *new* function to find the path towards the destination,
        // which will return the farthest reachable path within budget.
        List<Vector2Int> path = NeolithianRev.Utility.MovementAlgos.GetPathTowardsDestination( // <--- Use the new function
            ConvertToTileTypeArray_helper(Controller.GameMap),
            new Vector2Int(x, y),
            new Vector2Int(x_destination, y_destination),
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );

        //Debug.Log("Path found: " + (path != null ? string.Join(" -> ", path) : "No path found") + "\n");

        if (path != null && path.Count > 1) // path.Count > 1 means there's at least one step beyond start
        {
            // The path returned by GetPathTowardsDestination *already* represents
            // the longest path towards the destination within the budget.
            // So, we just need to move to the last tile in this path (which is the farthest reachable).

            // Calculate total cost to reach the final position in the path
            int totalCost = 0;
            Terrain[][] tileTypeMap = ConvertToTileTypeArray_helper(Controller.GameMap);

            // Start from 1 to skip the current tile and sum up costs of moves *into* tiles
            for (int i = 1; i < path.Count; i++)
            {
                Vector2Int pos = path[i]; // This is the tile we are moving *into*
                Terrain tileType = tileTypeMap[pos.x][pos.y];
                if (Entity_Stats.Settler_Movement_Costs.TryGetValue(tileType, out int cost))
                {
                    totalCost += cost;
                }
                else
                {
                    // This scenario should ideally not happen if GetPathTowardsDestination is robust
                    // But as a safeguard: if the path contains impassable terrain (no cost),
                    // it implies a problem in pathfinding or map data.
                    //Debug.LogError($"Path contains tile {pos} with no defined movement cost for terrain type {tileType}!");
                    return;
                }
            }

            Vector2Int finalPos = path[path.Count - 1]; // The last position in the path is the farthest reachable
            //Debug.Log("Calculated: " + finalPos + " as farthest reachable position.");

            x = finalPos.x;
            y = finalPos.y;
            movePoints -= totalCost;
            if (movePoints < 0) movePoints = 0; // Should ideally not be negative if costs are calculated correctly

            haschanged_render = true; // Mark for rendering
        }
        checkIfOntarget();
        // else: no movement possible (path is null or only contains startPos)
    }
}