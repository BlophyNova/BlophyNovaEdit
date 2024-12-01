using System.Collections.Generic;
using UtilityCode.Singleton;

namespace System
{
    public class Steps : MonoBehaviourSingleton<Steps>
    {
        public int maxLength;
        public int currentStepsIndex;
        public List<Step> steps = new();

        public void Add(Action undo, Action redo)
        {
            steps.Add(new Step { Undo = undo, Redo = redo });
            currentStepsIndex++;
            CheckMaxLength();
        }

        public void Undo()
        {
            if (currentStepsIndex - 1 < 0)
            {
                return;
            }

            steps[currentStepsIndex-- - 1].Undo();
            CheckMaxLength();
        }

        public void Redo()
        {
            steps[currentStepsIndex++].Redo();
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
        public Action Redo;
        public Action Undo;
    }
}