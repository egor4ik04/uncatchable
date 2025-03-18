using UnityEngine;

[RequireComponent(typeof(ButtonClicked))]
public class PlayButton : MonoBehaviour
{
    ButtonClicked buttonController;
    GameManager gameManager;    

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        buttonController = GetComponent<ButtonClicked>();
        buttonController.OnClickUp -= PlayClicked;
        buttonController.OnClickUp += PlayClicked;
    }

    private void PlayClicked() => gameManager.StartGame();
}
