using System;
using System.Collections.Generic;
using UtilityCode.Extension;
using UtilityCode.Singleton;

namespace CustomSystem
{
    public class Steps : MonoBehaviourSingleton<Steps>
    {
        public int maxLength;
        public int currentStepsIndex;
        public List<Step> steps = new();

        public void Add(Action undo, Action redo, Action @finally)
        {
            @finally ??= () => { };
            if (currentStepsIndex == steps.Count - 1)
            {
                steps.Add(new Step { Undo = undo, Redo = redo, Finally = @finally });
                currentStepsIndex++;
            }
            else
            {
                steps = steps.RemoveOfEnd(++currentStepsIndex);
                steps.Add(new Step { Undo = undo, Redo = redo, Finally = @finally });
                //currentStepsIndex++;
            }

            CheckMaxLength();
        }

        public void Undo()
        {
            if (currentStepsIndex < 0)
            {
                return;
            }

            if (currentStepsIndex >= steps.Count)
            {
                currentStepsIndex = steps.Count - 1;
            }

            steps[currentStepsIndex].Undo();
            steps[currentStepsIndex--].Finally();
            CheckMaxLength();
        }

        public void Redo()
        {
            if (currentStepsIndex >= steps.Count - 1)
            {
                return;
            }

            if (currentStepsIndex < 0)
            {
                currentStepsIndex = -1;
            }

            steps[++currentStepsIndex].Redo();
            steps[currentStepsIndex].Finally();
            CheckMaxLength();
        }

        private void CheckMaxLength()
        {
            if (steps.Count <= maxLength)
            {
                return;
            }

            int overflowSteps = steps.Count - maxLength;
            for (int i = overflowSteps; i >= 0; i--)
            {
                steps.RemoveAt(i);
                currentStepsIndex--;
            }

            if (currentStepsIndex + 2 < steps.Count)
            {
                for (int i = currentStepsIndex + 2; i < steps.Count; i++)
                {
                    steps.RemoveAt(i + 2);
                }
            }
        }
    }

    public class Step
    {
        public Action Finally;
        public Action Redo;
        public Action Undo;
    }
}