using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Form.NotePropertyEdit.ValueEdit
{
    public class EditNote : MonoBehaviour
    {
        public NotePropertyEdit notePropertyEdit;

        public Data.ChartEdit.Note originNote;
        public Data.ChartEdit.Note note;

        private List<ISelectBoxItem> selectedBoxItems = new();

        public TextMeshProUGUI noteEditText;
        public TMP_Dropdown noteType;
        public Toggle hitEffect;
        public Toggle hitRipple;
        public TMP_InputField startTime;
        public TMP_InputField holdTime;
        public TMP_InputField postionX;
        public Toggle isClockwise;
        public TMP_InputField speed;
        public Toggle isFakeNote;
        /// <summary>
        /// 多选编辑用的东西
        /// </summary>
        /// <param name="selectedBoxItems"></param>
        public void Set(List<ISelectBoxItem> selectedBoxItems)
        {
            if (selectedBoxItems.Count <= 0) return;
            noteEditText.text = $"音符编辑 {selectedBoxItems.Count}";
            this.selectedBoxItems =new(selectedBoxItems);
            Set((Scenes.Edit.NoteEdit)this.selectedBoxItems[0]);
        }
        void Set(Scenes.Edit.NoteEdit note)
        {
            Set(note.thisNoteData);
        }
        void Set(Data.ChartEdit.Note note)
        {
            originNote = new(note);
            this.note = note;
            SetNoteValue2Form();
            notePropertyEdit.editEvent.gameObject.SetActive(false);
            gameObject.SetActive(true);

        }

        private void SetNoteValue2Form()
        {
            noteType.SetValueWithoutNotify((int)this.note.noteType);
            hitEffect.SetIsOnWithoutNotify(
                this.note.effect.HasFlag(NoteEffect.CommonEffect));
            hitRipple.SetIsOnWithoutNotify(this.note.effect.HasFlag(NoteEffect.Ripple));
            startTime.SetTextWithoutNotify(
                $"{this.note.HitBeats.integer}:{this.note.HitBeats.molecule}/{this.note.HitBeats.denominator}");
            if (this.note.noteType == NoteType.Hold)
            {
                holdTime.interactable = true;
                holdTime.SetTextWithoutNotify(
                    $"{this.note.EndBeats.integer}:{this.note.EndBeats.molecule}/{this.note.EndBeats.denominator}");
            }
            else
            {

                holdTime.interactable = false;
                holdTime.SetTextWithoutNotify($"仅Hold音符可用");
            }

            postionX.SetTextWithoutNotify($"{this.note.positionX}");
            isClockwise.SetIsOnWithoutNotify(this.note.isClockwise);
        }
    }
}