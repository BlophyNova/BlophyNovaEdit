using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smart
{
    using static UnityEditor.EditorPrefs;

    public partial class Inspector
    {
        public static PrefsBool showInfo = new PrefsBool(true, "SI_ShowInfo");
        public static PrefsBool showSettings = new PrefsBool(true, "SI_ShowSettings");
        public static PrefsBool showSearchBar = new PrefsBool(true, "SI_ShowSearchBar");
        public static PrefsBool showButtons = new PrefsBool(true, "SI_ShowButtons");
        public static PrefsBool matchWord = new PrefsBool(true, "SI_MatchWord");
        public static PrefsInt maxSlots = new PrefsInt(6, "SI_MaxSlots");
        public static PrefsInt viewTypeValue = new PrefsInt(0, "SI_ViewType");
        public static PrefsFloat slotHeight = new PrefsFloat(30, "SI_SlotHeight");

        public void DeleteOldKeys()
        {
            showInfo.DeleteOldKeys();
            showSettings.DeleteOldKeys();
            showSearchBar.DeleteOldKeys();
            showSearchBar.DeleteOldKeys();
            showButtons.DeleteOldKeys();
            matchWord.DeleteOldKeys();
            maxSlots.DeleteOldKeys();
            viewTypeValue.DeleteOldKeys();
            slotHeight.DeleteOldKeys();
        }

        public class Prefs
        {
            protected string[] oldKeys;

            public Prefs(string[] oldKeys)
            {
                this.oldKeys = oldKeys;
            }

            public void DeleteOldKeys()
            {
                for(int i = 0; i < oldKeys.Length; i++)
                {
                    DeleteKey(oldKeys[i]);
                }
            }
        }

        public class PrefsBool : Prefs
        {
            private string key;
            private bool defaultValue;

            public bool Value
            {
                get => GetBool(key, defaultValue);
                set => SetBool(key, value);
            }

            public PrefsBool(bool defaultValue, string key, params string[] oldKeys) : base(oldKeys)
            {
                this.defaultValue = defaultValue;
                this.key = key;
            }
        }

        public class PrefsInt : Prefs
        {
            private string key;
            private int defaultValue;

            public int Value
            {
                get => GetInt(key, defaultValue);
                set => SetInt(key, value);
            }

            public PrefsInt(int defaultValue, string key, params string[] oldKeys) : base(oldKeys)
            {
                this.defaultValue = defaultValue;
                this.key = key;
            }
        }

        public class PrefsFloat : Prefs
        {
            private string key;
            private float defaultValue;

            public float Value
            {
                get => GetFloat(key, defaultValue);
                set => SetFloat(key, value);
            }

            public PrefsFloat(float defaultValue, string key, params string[] oldKeys) : base(oldKeys)
            {
                this.defaultValue = defaultValue;
                this.key = key;
            }
        }
    }
}
