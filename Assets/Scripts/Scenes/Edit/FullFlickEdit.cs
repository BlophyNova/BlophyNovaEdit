using System;
using Data.ChartData;
using UnityEngine;
using UnityEngine.UI;
using Note = Data.ChartEdit.Note;

namespace Scenes.Edit
{
    public class FullFlickEdit : NoteEditItem
    {
        public Image thisTexture;
        public Texture2D fullFlickBlue;
        public Texture2D fullFlickPink;

        public override NoteEditItem Init(Note note)
        {
            base.Init(note);
            thisTexture.sprite = note.noteType switch
            {
                NoteType.FullFlickBlue => Sprite.Create(fullFlickBlue,
                    new Rect(0, 0, fullFlickBlue.width, fullFlickBlue.height), Vector2.zero),
                NoteType.FullFlickPink => Sprite.Create(fullFlickPink,
                    new Rect(0, 0, fullFlickPink.width, fullFlickPink.height), Vector2.zero),
                _ => throw new Exception("为什么FullFlick会收到其它音符类型呢？")
            };
            transform.rotation = note.isClockwise switch
            {
                false => Quaternion.identity,
                true => Quaternion.Euler(0, 0, 180)
            };
            return this;
        }
    }
}