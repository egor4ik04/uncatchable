using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CatData", menuName = "UnCATchable/Data/Cat")]
public class CatOptions : ScriptableObject
{
    [SerializeField] public List<RuntimeAnimatorController> CatsControllers;    
    [SerializeField] private int currentControllerIndex;

    public int CurrentControllerIndex 
    { 
        get => currentControllerIndex; 
        set
        {
            currentControllerIndex = value;
            if (currentControllerIndex < 0)
                currentControllerIndex = CatsControllers.Count - 1;
            if (currentControllerIndex >= CatsControllers.Count)
                currentControllerIndex = 0;
            OnIndexChanged?.Invoke();
        } 
    }

    public event Action OnIndexChanged;
}