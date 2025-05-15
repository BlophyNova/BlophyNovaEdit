using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditNote : MonoBehaviour
    {
        public RectTransform viewport;
        public GridLayoutGroup gridLayoutGroup;

        public NotePropertyEdit notePropertyEdit;

        public List<Data.ChartEdit.Note> originNotes;
        public List<Data.ChartEdit.Note> notes;

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
            List<Scenes.Edit.NoteEdit> noteEdits = selectedBoxItems.Cast<Scenes.Edit.NoteEdit>().ToList();
            originNotes = new();
            notes.Clear();
            foreach (var note in noteEdits)
            {
                originNotes.Add(new(note.thisNoteData));
                notes.Add(note.thisNoteData);
            }
            SetNoteValue2Form();
            notePropertyEdit.EditEvent.gameObject.SetActive(false);
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void SetNoteValue2Form()
        {
            noteType.SetValueWithoutNotify((int)this.notes[0].noteType);
            hitEffect.SetIsOnWithoutNotify(
                this.notes[0].effect.HasFlag(NoteEffect.CommonEffect));
            hitRipple.SetIsOnWithoutNotify(this.notes[0].effect.HasFlag(NoteEffect.Ripple));
            startTime.SetTextWithoutNotify(
                $"{this.notes[0].HitBeats.integer}:{this.notes[0].HitBeats.molecule}/{this.notes[0].HitBeats.denominator}");
            if (this.notes[0].noteType == NoteType.Hold)
            {
                holdTime.interactable = true;
                holdTime.SetTextWithoutNotify(
                    $"{this.notes[0].EndBeats.integer}:{this.notes[0].EndBeats.molecule}/{this.notes[0].EndBeats.denominator}");
            }
            else
            {

                holdTime.interactable = false;
                holdTime.SetTextWithoutNotify($"仅Hold音符可用");
            }

            postionX.SetTextWithoutNotify($"{this.notes[0].positionX}");
            isClockwise.SetIsOnWithoutNotify(this.notes[0].isClockwise);
        }
    }
}