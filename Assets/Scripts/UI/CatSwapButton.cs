using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ButtonClicked))]
public class CatSwapButton : MonoBehaviour
{
    [SerializeField] bool selectingNext;
    [SerializeField] CatOptions options;
    ButtonClicked buttonController;

    void Start()
    {
        buttonController = GetComponent<ButtonClicked>();
        buttonController.OnClickUp -= CatSwap;
        buttonController.OnClickUp += CatSwap;
    }

    private void CatSwap()
    {
        if (selectingNext)
            options.CurrentControllerIndex++;
        else
            options.CurrentControllerIndex--;
    }
}
