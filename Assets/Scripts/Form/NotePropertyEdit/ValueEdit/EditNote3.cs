using Controller;
using CustomSystem;
using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Form.PropertyEdit;
using UnityEngine;
using NCalc;
namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditNote
    {
        private void Start()
        {
            notePropertyEdit.labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged;
            LabelWindow_onWindowSizeChanged();

            noteType.onValueChanged.AddListener(value => 
            {
                Steps.Instance.Add(Undo,Redo,Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        notes[i].noteType = originNotes[i].noteType;
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        note.noteType = (NoteType)value;
                    }
                }
            });
            hitEffect.onValueChanged.AddListener(value => 
            {
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        notes[i].effect = originNotes[i].effect;
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        if (value)
                        {
                            note.effect |= NoteEffect.CommonEffect;
                        }
                        else
                        {
                            note.effect ^= NoteEffect.CommonEffect;
                        }
                    }
                }
            });
            hitRipple.onValueChanged.AddListener(value =>
            {
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        notes[i].effect = originNotes[i].effect;
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        if (value)
                        {
                            note.effect |= NoteEffect.Ripple;
                        }
                        else
                        {
                            note.effect ^= NoteEffect.Ripple;
                        }
                    }
                }
            });
            startTime.onEndEdit.AddListener(value => 
            {
                Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
                if (!match.Success) return;
                //EventValueChanged(match, note.HitBeats);
                BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++) 
                    {
                        notes[i].HitBeats = new(originNotes[i].HitBeats);
                    }
                }
                void Redo() 
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        note.HitBeats = new(targetValue);
                    }
                }
            });

            holdTime.onEndEdit.AddListener(value =>
            {
                Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
                if (!match.Success) return;
                //EventValueChanged(match, note.HitBeats);
                BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        notes[i].holdBeats = new(originNotes[i].holdBeats);
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        note.holdBeats = new(targetValue);
                    }
                }
                
            });
            postionX.onEndEdit.AddListener(value => 
            {
                if (!float.TryParse(value, out float result))
                {
                    Expression expression = new(value);
                    try
                    {
                        result = float.Parse($"{expression.Evaluate()}");
                    }
                    catch
                    {
                        return;
                    }
                };
                Steps.Instance.Add(Undo,Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        notes[i].positionX = originNotes[i].positionX;
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        note.positionX = result;
                    }
                }
            });
            isClockwise.onValueChanged.AddListener(value => 
            {
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        notes[i].isClockwise = originNotes[i].isClockwise;
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Note note in notes)
                    {
                        if (value)
                        {
                            note.isClockwise = true;
                        }
                        else
                        {
                            note.isClockwise = false;
                        }
                    }
                }
            });
            void Finally()
            {
                GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1),new(){typeof(NoteEdit.NoteEdit)});
                GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1),null);
            }
        }

        private void LabelWindow_onWindowSizeChanged()
        {
            gridLayoutGroup.cellSize = new(viewport.rect.width, 50);
        }
    }
}