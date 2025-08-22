using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Ingame");
    }

    public void GenerateWorld()
    {
        //Step 1 Size
        //Step 2 Normal Climate? -> random direction for temperature
        //Step 3 simulateWorld History -> random Ice Ages for soils, continent movement for terrain roughness plus simultanisoly repeat cycle: terrain -> calculate water -> calculate soils -> calculate trees 
        //Step 4 calculate terrain

    }

    public void OpenSettings()
    {
        // Open settings menu
    }

    public void OpenCredits()
    {
        // Open credits menu
    }

    public void OpenHelp()
    {
        // Open help menu
    }   

    public void OpenAbout()
    {
        // Open about menu
    }

    public void OpenFeedback()
    {
        // Open feedback menu
    }      

    public void OpenPrivacyPolicy()
    {
        // Open privacy policy menu
    }   

    public void OpenTermsOfService()
    {
        // Open terms of service menu
    }

    public void OpenLegalNotice()
    {
        // Open legal notice menu
    }

    public void OpenUserAgreement()
    {
        // Open user agreement menu
    }   

    public void OpenSocials()
    {
        // Open socials menu
    }



    public void QuitGame()
    {
        Application.Quit();
        // In editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
