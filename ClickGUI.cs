using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Action;

public class ClickGUI : MonoBehaviour {

	UserAction action;
	zyzCharacterController chaController;

    void Start()
    {
        action = Director.getInstance().currentSceneController as UserAction;
    }

    public void setController(zyzCharacterController _characterController) {
		chaController = _characterController;
	}

	void OnMouseDown() {

		if (gameObject.name == "boat") 
		{
			action.moveBoatToTheOtherCoast ();
		} 
		else 
		{
			action.characterBeClicked (chaController);
		}
	
	}
}
