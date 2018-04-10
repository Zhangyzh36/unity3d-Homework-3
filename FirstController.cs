using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Action;

public class FirstController : MonoBehaviour, UserAction, SceneController {
    UserGUI userGUI;

    public CoastController leftCoast;
    public CoastController rightCoast;
    public BoatController boat;
    private zyzCharacterController[] characters;

    private CCActionManager actionManager;

    void Start()
    {
        actionManager = GetComponent<CCActionManager>();
    }

    void Awake()
    {
        Director director = Director.getInstance();
        director.currentSceneController = this;
        userGUI = gameObject.AddComponent(typeof(UserGUI)) as UserGUI;
        characters = new zyzCharacterController[6];
        loadResources();
    }

    public void loadResources()
    {
        leftCoast = new CoastController("left");
        rightCoast = new CoastController("right");
        boat = new BoatController();

        for (int i = 0; i < 3; i++)
        {
            zyzCharacterController newPriest = new zyzCharacterController("priest");
            newPriest.setName("priest" + i);
            newPriest.setPosition(leftCoast.getEmptyPosition());
            newPriest.toCoast(leftCoast);
            leftCoast.toCoast(newPriest);

            characters[i] = newPriest;

        }

        for (int i = 0; i < 3; i++)
        {

            zyzCharacterController newDevil = new zyzCharacterController("devil");
            newDevil.setName("devil" + i);
            newDevil.setPosition(leftCoast.getEmptyPosition());
            newDevil.toCoast(leftCoast);
            leftCoast.toCoast(newDevil);

            characters[i + 3] = newDevil;
        }

    }

    public void moveBoatToTheOtherCoast()
    {
        if (boat.isEmpty()) return;
        actionManager.moveBoat(boat);
        boat.Move();
        userGUI.status = judge();
    }

    public void characterBeClicked(zyzCharacterController _characterController)
    {
        if (_characterController.isOnBoat())
        {
            CoastController theCoast;
            if (boat.getLeftRight() == false)
            { 
                theCoast = rightCoast;
            }
            else
            {
                theCoast = leftCoast;
            }

            boat.offBoat(_characterController.getName());
            actionManager.moveCharacter(_characterController, theCoast.getEmptyPosition());
            _characterController.toCoast(theCoast);
            theCoast.toCoast(_characterController);

        }
        else
        {                                   
            CoastController theCoast = _characterController.getCoastController();

            if (boat.getEmptyIndex() == -1)
            {       
                return;
            }

            if (theCoast.getLeftRight() != boat.getLeftRight())   
                return;

            theCoast.offCoast(_characterController.getName());
            actionManager.moveCharacter(_characterController, boat.getEmptyPosition());
            _characterController.toBoat(boat);
            boat.toBoat(_characterController);
        }
        userGUI.status = judge();
    }

    int judge()
    {  
        int leftPriest = 0;
        int leftDevil = 0;
        int rightPriest = 0;
        int rightDevil = 0;

        int[] leftnum = leftCoast.getCharacterNumber();
        leftPriest += leftnum[0];
        leftDevil += leftnum[1];

        int[] rightnum = rightCoast.getCharacterNumber();
        rightPriest += rightnum[0];
        rightDevil += rightnum[1];

        if (rightPriest + rightDevil == 6)     
            return 2;

        int[] numOnBoat = boat.getCharacterNumber();

        if (boat.getLeftRight() == false)
        {   
            rightPriest += numOnBoat[0];
            rightDevil += numOnBoat[1];
        }
        else
        {  
            leftPriest += numOnBoat[0];
            leftDevil += numOnBoat[1];
        }

        if (leftPriest > 0 && leftPriest < leftDevil || rightPriest > 0 && rightPriest < rightDevil )
        {       
            return 1;
        }
        return 0;          
    }

    public void restartGame()
    {
        boat.clear();
        leftCoast.clear();
        rightCoast.clear();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].clear();
        }
       
    }

}
