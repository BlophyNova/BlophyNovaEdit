using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Data.ShortcutKey
{
    public partial class ShortcutKeyData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("maps")]
        public Map[] Maps { get; set; }

        [JsonProperty("controlSchemes")]
        public ControlScheme[] ControlSchemes { get; set; }
    }

    public partial class ControlScheme
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bindingGroup")]
        public string BindingGroup { get; set; }

        [JsonProperty("devices")]
        public Device[] Devices { get; set; }
    }

    public partial class Device
    {
        [JsonProperty("devicePath")]
        public string DevicePath { get; set; }

        [JsonProperty("isOptional")]
        public bool IsOptional { get; set; }

        [JsonProperty("isOR")]
        public bool IsOr { get; set; }
    }

    public partial class Map
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("actions")]
        public Action[] Actions { get; set; }

        [JsonProperty("bindings")]
        public Binding[] Bindings { get; set; }
    }

    public partial class Action
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("expectedControlType")]
        public ExpectedControlType ExpectedControlType { get; set; }

        [JsonProperty("processors")]
        public string Processors { get; set; }

        [JsonProperty("interactions")]
        public string Interactions { get; set; }

        [JsonProperty("initialStateCheck")]
        public bool InitialStateCheck { get; set; }
    }

    public partial class Binding
    {
        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("interactions")]
        public string Interactions { get; set; }

        [JsonProperty("processors")]
        public string Processors { get; set; }

        [JsonProperty("groups")]
        public Groups Groups { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("isComposite")]
        public bool IsComposite { get; set; }

        [JsonProperty("isPartOfComposite")]
        public bool IsPartOfComposite { get; set; }
    }

    public enum ExpectedControlType { Axis, Button };

    public enum TypeEnum { Button, Value };

    public enum Groups { Empty, KeyboardMouse };

    public enum Name { Binding, Empty, Modifier, Modifier1, Modifier2, OneModifier, TwoModifiers };

    public partial class ShortcutKeyData
    {
        public static ShortcutKeyData FromJson(string json) => JsonConvert.DeserializeObject<ShortcutKeyData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ShortcutKeyData self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ExpectedControlTypeConverter.Singleton,
                TypeEnumConverter.Singleton,
                GroupsConverter.Singleton,
                NameConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ExpectedControlTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ExpectedControlType) || t == typeof(ExpectedControlType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Axis":
                    return ExpectedControlType.Axis;
                case "Button":
                    return ExpectedControlType.Button;
            }
            throw new Exception("Cannot unmarshal type ExpectedControlType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ExpectedControlType)untypedValue;
            switch (value)
            {
                case ExpectedControlType.Axis:
                    serializer.Serialize(writer, "Axis");
                    return;
                case ExpectedControlType.Button:
                    serializer.Serialize(writer, "Button");
                    return;
            }
            throw new Exception("Cannot marshal type ExpectedControlType");
        }

        public static readonly ExpectedControlTypeConverter Singleton = new ExpectedControlTypeConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Button":
                    return TypeEnum.Button;
                case "Value":
                    return TypeEnum.Value;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Button:
                    serializer.Serialize(writer, "Button");
                    return;
                case TypeEnum.Value:
                    serializer.Serialize(writer, "Value");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class GroupsConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Groups) || t == typeof(Groups?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return Groups.Empty;
                case "Keyboard&Mouse":
                    return Groups.KeyboardMouse;
            }
            throw new Exception("Cannot unmarshal type Groups");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Groups)untypedValue;
            switch (value)
            {
                case Groups.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case Groups.KeyboardMouse:
                    serializer.Serialize(writer, "Keyboard&Mouse");
                    return;
            }
            throw new Exception("Cannot marshal type Groups");
        }

        public static readonly GroupsConverter Singleton = new GroupsConverter();
    }

    internal class NameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Name) || t == typeof(Name?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return Name.Empty;
                case "One Modifier":
                    return Name.OneModifier;
                case "Two Modifiers":
                    return Name.TwoModifiers;
                case "binding":
                    return Name.Binding;
                case "modifier":
                    return Name.Modifier;
                case "modifier1":
                    return Name.Modifier1;
                case "modifier2":
                    return Name.Modifier2;
            }
            throw new Exception("Cannot unmarshal type Name");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Name)untypedValue;
            switch (value)
            {
                case Name.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case Name.OneModifier:
                    serializer.Serialize(writer, "One Modifier");
                    return;
                case Name.TwoModifiers:
                    serializer.Serialize(writer, "Two Modifiers");
                    return;
                case Name.Binding:
                    serializer.Serialize(writer, "binding");
                    return;
                case Name.Modifier:
                    serializer.Serialize(writer, "modifier");
                    return;
                case Name.Modifier1:
                    serializer.Serialize(writer, "modifier1");
                    return;
                case Name.Modifier2:
                    serializer.Serialize(writer, "modifier2");
                    return;
            }
            throw new Exception("Cannot marshal type Name");
        }

        public static readonly NameConverter Singleton = new NameConverter();
    }
}
