using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Creditswindow : MonoBehaviour
{
    public MainMenu mainMenu;
    void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) //isPressed
        {
            mainMenu.CloseCredits();
        }
    }
}