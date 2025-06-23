using System.Collections.Generic;
using System.Linq;
using Data.ChartData;
using Data.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Note = Data.ChartEdit.Note;

namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditNote : MonoBehaviour
    {
        public RectTransform viewport;
        public GridLayoutGroup gridLayoutGroup;

        public NotePropertyEdit notePropertyEdit;

        public List<Note> originNotes;
        public List<Note> notes;

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
        ///     多选编辑用的东西
        /// </summary>
        /// <param name="selectedBoxItems"></param>
        public void Set(List<ISelectBoxItem> selectedBoxItems)
        {
            if (selectedBoxItems.Count <= 0)
            {
                return;
            }

            noteEditText.text = $"音符编辑 {selectedBoxItems.Count}";
            List<Scenes.Edit.NoteEditItem> noteEdits = selectedBoxItems.Cast<Scenes.Edit.NoteEditItem>().ToList();
            originNotes = new List<Note>();
            notes.Clear();
            foreach (Scenes.Edit.NoteEditItem note in noteEdits)
            {
                originNotes.Add(new Note(note.thisNoteData));
                notes.Add(note.thisNoteData);
            }

            SetNoteValue2Form();
            notePropertyEdit.EditEvent.gameObject.SetActive(false);
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void SetNoteValue2Form()
        {
            noteType.SetValueWithoutNotify((int)notes[0].noteType);
            hitEffect.SetIsOnWithoutNotify(
                notes[0].effect.HasFlag(NoteEffect.CommonEffect));
            hitRipple.SetIsOnWithoutNotify(notes[0].effect.HasFlag(NoteEffect.Ripple));
            startTime.SetTextWithoutNotify(
                $"{notes[0].HitBeats.integer}:{notes[0].HitBeats.molecule}/{notes[0].HitBeats.denominator}");
            if (notes[0].noteType == NoteType.Hold)
            {
                holdTime.interactable = true;
                holdTime.SetTextWithoutNotify(
                    $"{notes[0].EndBeats.integer}:{notes[0].EndBeats.molecule}/{notes[0].EndBeats.denominator}");
            }
            else
            {
                holdTime.interactable = false;
                holdTime.SetTextWithoutNotify("仅Hold音符可用");
            }

            postionX.SetTextWithoutNotify($"{notes[0].positionX}");
            isClockwise.SetIsOnWithoutNotify(notes[0].isClockwise);
        }
    }
}