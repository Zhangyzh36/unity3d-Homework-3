using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Action;

public class UserGUI : MonoBehaviour {

    private UserAction action;
    public int status = 0;
    GUIStyle style1;
    GUIStyle style2;

    void Start()
    {
        action = Director.getInstance().currentSceneController as UserAction;

        style1 = new GUIStyle();
        style1.alignment = TextAnchor.MiddleCenter;
        style1.fontSize = 40;

        style2 = new GUIStyle("button");
        style2.alignment = TextAnchor.MiddleCenter;
        style2.fontSize = 30;
    }
    void OnGUI()
    {
        int widthOfLabel = 100;
        int heightOfLabel = 50;
        int widthOfButton = 140;
        int heightOfButton = 70;
       
        if (status == 1)
        {
            GUI.Label(new Rect((Screen.width - widthOfLabel) / 2, Screen.height / 2 - 85, widthOfLabel, heightOfLabel), "You lose!", style1);
           
        }
        else if (status == 2)
        {
            GUI.Label(new Rect((Screen.width - widthOfLabel) / 2, Screen.height / 2 - 85, widthOfLabel, heightOfLabel), "You win!", style1);

        }

        if (GUI.Button(new Rect((Screen.width - widthOfLabel) / 2, Screen.height / 2 - 200, widthOfButton, heightOfButton), "restart", style2))
        {
            status = 0;
            action.restartGame();
        }
    }
}
