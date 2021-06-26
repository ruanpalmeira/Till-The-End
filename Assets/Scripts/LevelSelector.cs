using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour{

    public Button[] levelButtons;
    public GameController gameController;

    void Start(){
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++){
            if(i + 1 > levelReached){
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponentInChildren<Text>().color = MyColors.lightBlue;
            }else{
                levelButtons[i].GetComponentInChildren<Text>().color = MyColors.yellow;
            }
            
        }
    }

    public void Select(string level){
        gameController.loadLevel(level);
    }
}