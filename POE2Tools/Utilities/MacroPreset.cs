using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace POE2Tools.Utilities
{
    public enum MacroActionType
    {
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp
    }

    public class MacroAction
    {
        public MacroActionType Type { get; set; }
        // Milliseconds since the start of the recording
        public int Time { get; set; }
        // Used by KeyDown / KeyUp
        public Keys Key { get; set; }
        // Used by MouseDown / MouseUp. The position is in percent of the screen size (0 - 100),
        // so a macro still clicks at the right place on another resolution of the same ratio
        public MouseButtons Button { get; set; }
        public double XPercent { get; set; }
        public double YPercent { get; set; }

        // Position in pixels, only found in files saved by older versions. Converted by MacroPresetStore.Load
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? X { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Y { get; set; }

        [JsonIgnore]
        public bool IsMouse => Type == MacroActionType.MouseDown || Type == MacroActionType.MouseUp;

        public MacroAction Clone()
        {
            return (MacroAction)MemberwiseClone();
        }

        public bool IsSameAs(MacroAction other)
        {
            return Type == other.Type && Time == other.Time && Key == other.Key
                && Button == other.Button && XPercent == other.XPercent && YPercent == other.YPercent;
        }

        public string Describe()
        {
            switch (Type)
            {
                case MacroActionType.KeyDown: return "Key down";
                case MacroActionType.KeyUp: return "Key up";
                case MacroActionType.MouseDown: return "Mouse down";
                default: return "Mouse up";
            }
        }

        public string DescribeDetail()
        {
            return IsMouse ? Button + " at (" + XPercent.ToString("0.00") + "%, " + YPercent.ToString("0.00") + "%)" : Key.ToString();
        }
    }

    public enum MacroType
    {
        // Plays the recorded clicks and key presses
        Repeat,
        // Does nothing for some time, then ends
        Delay,
        // Clicks the item labels found on the screen, ends when there is none left
        ItemPickup
    }

    // The colors of an item label worth clicking, as the loot filter shows it
    public class LabelColor
    {
        // "#RRGGBB"
        public string Background { get; set; } = "#FFFFFF";
        public string Text { get; set; } = "#000000";

        public LabelColor()
        {
        }

        public LabelColor(System.Drawing.Color background, System.Drawing.Color text)
        {
            Background = ToHex(background);
            Text = ToHex(text);
        }

        [JsonIgnore]
        public System.Drawing.Color BackgroundColor => FromHex(Background);
        [JsonIgnore]
        public System.Drawing.Color TextColor => FromHex(Text);

        public static string ToHex(System.Drawing.Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        // Black if the text is not a valid color
        public static System.Drawing.Color FromHex(string hex)
        {
            if (hex != null && hex.Length == 7 && hex[0] == '#'
                && int.TryParse(hex.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out int rgb))
            {
                return System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(rgb));
            }
            return System.Drawing.Color.Black;
        }
    }

    // Used by MacroType.ItemPickup
    public class ItemPickupSettings
    {
        public List<LabelColor> Labels { get; set; } = new List<LabelColor>();
        // How far each of R, G, B can be from the background color. The text is anti-aliased, it gets twice that
        public int ColorTolerance { get; set; } = 25;
        // Wait this long after a click before looking again, the character has to walk to the item
        public int DelayAfterClick { get; set; } = 600;
        // Wait this long before looking again when nothing was found
        public int ScanInterval { get; set; } = 200;
        // The macro ends when nothing was found this many times in a row
        public int EmptyScansToEnd { get; set; } = 3;
        // The macro also ends after this many clicks, in case an item can't be picked up (inventory full...)
        public int MaxClicks { get; set; } = 30;
        // Where to look, in percent of the screen size. The default leaves the bottom HUD out
        public double RegionLeft { get; set; } = 0;
        public double RegionTop { get; set; } = 3;
        public double RegionRight { get; set; } = 100;
        public double RegionBottom { get; set; } = 82;

        public static ItemPickupSettings CreateDefault()
        {
            ItemPickupSettings settings = new ItemPickupSettings();
            // White box with red text, salmon box with black text, and the orange box of the uniques
            settings.Labels.Add(new LabelColor { Background = "#F0EDED", Text = "#C80F0F" });
            settings.Labels.Add(new LabelColor { Background = "#DF5F51", Text = "#190000" });
            settings.Labels.Add(new LabelColor { Background = "#B05D2A", Text = "#EBEBEB" });
            return settings;
        }
    }

    public class MacroPreset
    {
        // Shown in the type dropdown of the Automation window, in the order of MacroType
        public static readonly string[] TypeNames = { "Repeat macro", "Delay", "Item pickup" };

        public string Name { get; set; } = "";
        public MacroType Type { get; set; } = MacroType.Repeat;
        // Used by MacroType.Delay, in milliseconds
        public int DelayMs { get; set; } = 1000;
        // Used by MacroType.ItemPickup
        public ItemPickupSettings Pickup { get; set; } = ItemPickupSettings.CreateDefault();
        // Everything below is only used by MacroType.Repeat
        // Length of one loop in milliseconds (time from record start to record stop)
        public int Duration { get; set; }
        public List<MacroAction> Actions { get; set; } = new List<MacroAction>();

        public bool IsSameMacro(List<MacroAction> actions, int duration)
        {
            if (Duration != duration || Actions.Count != actions.Count) return false;
            for (int i = 0; i < actions.Count; i++)
            {
                if (!Actions[i].IsSameAs(actions[i])) return false;
            }
            return true;
        }
    }

    public enum ConditionType
    {
        // The macro preset of the step played until its end
        MacroEnded,
        // An area of the screen looks like the saved sample
        ScreenMatch,
        // The macro preset of the step played until its end a number of times
        MacroLooped,
        // The macro preset of the step has been playing for some time. Checked at the end of each loop
        MacroRanFor
    }

    public enum ConditionTarget
    {
        NextStep,
        GoToStep,
        EndAction
    }

    public class ActionCondition
    {
        public ConditionType Type { get; set; } = ConditionType.MacroEnded;
        public ConditionTarget Target { get; set; } = ConditionTarget.NextStep;
        // Index of the step to jump to, used by ConditionTarget.GoToStep
        public int TargetStep { get; set; }

        // Used by ConditionType.MacroLooped
        public int LoopCount { get; set; } = 5;
        // Used by ConditionType.MacroRanFor
        public double RunSeconds { get; set; } = 60;

        // Everything below is only used by ConditionType.ScreenMatch
        public int CheckInterval { get; set; } = 500;
        // How far each of R, G, B can be from the sample for a pixel to count as matching
        public int ColorTolerance { get; set; } = 20;
        public int MatchPercent { get; set; } = 90;
        // Where the sample was cropped from, in screen pixels. Only this area is captured when running
        public int RegionX { get; set; }
        public int RegionY { get; set; }
        public int RegionWidth { get; set; }
        public int RegionHeight { get; set; }
        // The sample picture, as a base64 PNG
        public string SampleBase64 { get; set; } = "";

        [JsonIgnore]
        public System.Drawing.Rectangle Region => new System.Drawing.Rectangle(RegionX, RegionY, RegionWidth, RegionHeight);

        public ActionCondition Clone()
        {
            return (ActionCondition)MemberwiseClone();
        }

        public string Describe()
        {
            if (Type == ConditionType.MacroEnded) return "Macro preset ended";
            if (Type == ConditionType.MacroLooped) return "Macro preset has looped " + LoopCount + " times";
            if (Type == ConditionType.MacroRanFor) return "Macro preset has run for " + RunSeconds.ToString("0.#") + " seconds";
            return "Every " + CheckInterval + " ms, screen at (" + RegionX + ", " + RegionY + ") " + RegionWidth + "x" + RegionHeight
                + " matches sample >= " + MatchPercent + "%";
        }
    }

    public class ActionStep
    {
        // Name of the macro preset to play. Empty means the step only waits for its conditions
        public string MacroName { get; set; } = "";
        public List<ActionCondition> Conditions { get; set; } = new List<ActionCondition>();
    }

    public class ActionPreset
    {
        public string Name { get; set; } = "";
        public List<ActionStep> Steps { get; set; } = new List<ActionStep>();
    }

    public class MacroPresetStore
    {
        public string SelectedPreset { get; set; } = "";
        public List<MacroPreset> Presets { get; set; } = new List<MacroPreset>();
        public string SelectedActionPreset { get; set; } = "";
        public List<ActionPreset> ActionPresets { get; set; } = new List<ActionPreset>();
        // "Stop after" option of the Automation window
        public bool StopAfterEnabled { get; set; } = false;
        public int StopAfterCount { get; set; } = 10;
        public bool SleepWhenDone { get; set; } = false;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private const string FILE_NAME = "AutomationPresets.json";

        // Next to the executable, so the presets travel with the tool
        public static string FilePath
        {
            get { return Path.Combine(AppContext.BaseDirectory, FILE_NAME); }
        }

        // Where the file was before it moved next to the executable
        private static string OldFilePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "POE2Tools", FILE_NAME); }
        }

        public MacroPreset Find(string name)
        {
            return Presets.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public ActionPreset FindAction(string name)
        {
            return ActionPresets.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        // Used to deep copy action presets, and to tell if one was modified
        public static string ToJson(ActionPreset preset)
        {
            return JsonSerializer.Serialize(preset, _jsonOptions);
        }

        public static string ToJson(ItemPickupSettings settings)
        {
            return JsonSerializer.Serialize(settings, _jsonOptions);
        }

        public static ItemPickupSettings ClonePickup(ItemPickupSettings settings)
        {
            return JsonSerializer.Deserialize<ItemPickupSettings>(ToJson(settings), _jsonOptions);
        }

        public static ActionStep CloneStep(ActionStep step)
        {
            return JsonSerializer.Deserialize<ActionStep>(JsonSerializer.Serialize(step, _jsonOptions), _jsonOptions);
        }

        public static ActionPreset CloneAction(ActionPreset preset)
        {
            return JsonSerializer.Deserialize<ActionPreset>(ToJson(preset), _jsonOptions);
        }

        public static MacroPresetStore Load()
        {
            try
            {
                // Bring the presets made by older versions along. The old file is left where it is
                if (!File.Exists(FilePath) && File.Exists(OldFilePath))
                {
                    File.Copy(OldFilePath, FilePath);
                }

                if (File.Exists(FilePath))
                {
                    MacroPresetStore store = JsonSerializer.Deserialize<MacroPresetStore>(File.ReadAllText(FilePath), _jsonOptions);
                    if (store != null)
                    {
                        store.Presets ??= new List<MacroPreset>();
                        store.ActionPresets ??= new List<ActionPreset>();
                        foreach (MacroPreset preset in store.Presets)
                        {
                            preset.Actions ??= new List<MacroAction>();
                            preset.Pickup ??= ItemPickupSettings.CreateDefault();
                            preset.Pickup.Labels ??= new List<LabelColor>();
                            foreach (MacroAction action in preset.Actions)
                            {
                                ConvertLegacyPosition(action);
                            }
                        }
                        return store;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load automation presets:\n" + ex.Message, "Automation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return new MacroPresetStore();
        }

        // Older versions saved the click positions in pixels. The resolution they were recorded on
        // is unknown, the current one is the best guess
        private static void ConvertLegacyPosition(MacroAction action)
        {
            if (action.X == null && action.Y == null) return;

            // Key presses have no position, theirs was always (0, 0)
            if (action.IsMouse)
            {
                System.Drawing.Size screen = Screen.PrimaryScreen.Bounds.Size;
                action.XPercent = InputHook.PixelToPercent(action.X ?? 0, screen.Width);
                action.YPercent = InputHook.PixelToPercent(action.Y ?? 0, screen.Height);
            }
            action.X = null;
            action.Y = null;
        }

        public bool Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, JsonSerializer.Serialize(this, _jsonOptions));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save automation presets:\n" + ex.Message, "Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
