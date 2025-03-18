using System;
using UnityEngine;

[Serializable]
public class DifficultyFloatPropertyModifier : DifficultyPropertyModifier<float>
{
    public override float Value => Mathf.Lerp(StartValue, EndValue, (float)DifficultyManager.DifficultyCurrentStep / (float)DifficultyManager.DifficultySteps);
}
[Serializable]
public abstract class DifficultyPropertyModifier<T>
{
    public abstract T Value { get; }
    public T StartValue;
    public T EndValue;
}