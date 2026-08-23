using UnityEditor;
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

    private void ExitClicked() =>
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
}
