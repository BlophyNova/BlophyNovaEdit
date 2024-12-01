using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UtilityCode.Singleton;

public class Steps : MonoBehaviourSingleton<Steps>
{
    public int maxLength;
    public List<Step> steps = new();
    public int currentStepsIndex = 0;

    public void Add(Action undo,Action redo)
    {
        steps.Add(new(){Undo = undo,Redo = redo});
        currentStepsIndex++;
        CheckMaxLength();
    }

    public void Undo()
    {
        steps[currentStepsIndex--].Undo();
        CheckMaxLength();
    }

    public void Redo()
    {
        steps[++currentStepsIndex].Redo();
        CheckMaxLength();
    }

    void CheckMaxLength()
    {
        if (steps.Count<=maxLength)return;
        int overflowSteps = steps.Count - maxLength;
        for (int i = overflowSteps; i>=0; i--)
        {
            steps.RemoveAt(i);
            currentStepsIndex--;
        }
        
        if (currentStepsIndex + 2 < steps.Count)
        {
            for (int i = currentStepsIndex+2; i < steps.Count; i++)
            {
                steps.RemoveAt(i+2);
            }
        }
    }
}

public class Step
{
    public Action Undo;
    public Action Redo;
}