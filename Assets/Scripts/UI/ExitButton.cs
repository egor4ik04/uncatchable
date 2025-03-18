using UnityEngine;

[RequireComponent(typeof(ButtonClicked))]
public class ExitButton : MonoBehaviour
{
    ButtonClicked buttonController;

    void Start()
    {
        buttonController = GetComponent<ButtonClicked>();
        buttonController.OnClickUp -= ExitClicked;
        buttonController.OnClickUp += ExitClicked;
    }

    private void ExitClicked() => Application.Quit();
}
