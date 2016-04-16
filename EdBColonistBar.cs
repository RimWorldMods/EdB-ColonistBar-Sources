using RimWorld;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml;
using System;
using UnityEngine;
using Verse.Sound;
using Verse;

namespace EdB.Interface
{
	public abstract class BooleanPreference : IPreference
	{
		public delegate void ValueChangedHandler (bool value);

		public static float LabelMargin = BooleanPreference.CheckboxWidth + BooleanPreference.CheckboxMargin;

		public static float CheckboxMargin = 18;

		public static float CheckboxWidth = 24;

		public int tooltipId = 0;

		private bool? boolValue = null;

		private string stringValue;

		public abstract bool DefaultValue {
			get;
		}

		public virtual bool Disabled {
			get {
				return false;
			}
		}

		public virtual bool DisplayInOptions {
			get {
				return true;
			}
		}

		public abstract string Group {
			get;
		}

		public virtual bool Indent {
			get {
				return false;
			}
		}

		public virtual string Label {
			get {
				return Translator.Translate (this.Name);
			}
		}

		public abstract string Name {
			get;
		}

		public virtual string Tooltip {
			get {
				return null;
			}
		}

		protected virtual int TooltipId {
			get {
				int result;
				if (this.tooltipId == 0) {
					this.tooltipId = Translator.Translate (this.Tooltip).GetHashCode ();
					result = this.tooltipId;
				}
				else {
					result = 0;
				}
				return result;
			}
		}

		public virtual bool Value {
			get {
				bool result;
				if (this.boolValue.HasValue) {
					result = this.boolValue.Value;
				}
				else {
					result = this.DefaultValue;
				}
				return result;
			}
			set {
				bool? flag = this.boolValue;
				this.boolValue = new bool? (value);
				this.stringValue = (value ? "true" : "false");
				if ((!flag.HasValue || flag.Value != this.boolValue) && this.ValueChanged != null) {
					this.ValueChanged (value);
				}
			}
		}

		public virtual bool ValueForDisplay {
			get {
				bool result;
				if (this.boolValue.HasValue) {
					result = this.boolValue.Value;
				}
				else {
					result = this.DefaultValue;
				}
				return result;
			}
		}

		public string ValueForSerialization {
			get {
				return this.stringValue;
			}
			set {
				if ("true".Equals (value)) {
					this.boolValue = new bool? (true);
					this.stringValue = value;
				}
				else if ("false".Equals (value)) {
					this.boolValue = new bool? (false);
					this.stringValue = value;
				}
				else {
					if (!string.IsNullOrEmpty (value)) {
						this.boolValue = null;
						this.stringValue = null;
						throw new ArgumentException ("Cannot set this true/false preference to the specified non-boolean value.");
					}
					this.boolValue = null;
					this.stringValue = null;
				}
			}
		}

		public BooleanPreference ()
		{
		}

		public void OnGUI (float positionX, ref float positionY, float width)
		{
			bool disabled = this.Disabled;
			float num = this.Indent ? WidgetDrawer.IndentSize : 0;
			string label = this.Label;
			float num2 = Text.CalcHeight (label, width - BooleanPreference.LabelMargin - num);
			Rect rect = new Rect (positionX - 4 + num, positionY - 3, width + 6 - num, num2 + 5);
			if (Mouse.IsOver (rect)) {
				Widgets.DrawHighlight (rect);
			}
			Rect rect2 = new Rect (positionX + num, positionY, width - BooleanPreference.LabelMargin - num, num2);
			if (disabled) {
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			GUI.Label (rect2, label);
			GUI.color = Color.white;
			if (this.Tooltip != null) {
				TipSignal tipSignal = new TipSignal (() => Translator.Translate (this.Tooltip), this.TooltipId);
				TooltipHandler.TipRegion (rect2, tipSignal);
			}
			bool valueForDisplay = this.ValueForDisplay;
			Widgets.Checkbox (new Vector2 (positionX + width - BooleanPreference.CheckboxWidth, positionY - 2), ref valueForDisplay, 24, disabled);
			this.Value = valueForDisplay;
			positionY += num2;
		}

		public event BooleanPreference.ValueChangedHandler ValueChanged;
	}

	public class Button
	{
		public static LazyLoadTexture ButtonBGAtlas = new LazyLoadTexture ("EdB/Interface/TextButton");

		protected static readonly Color MouseoverOptionColor = Color.yellow;

		protected static Color InactiveButtonColor = new Color (1, 1, 1, 0.5f);

		public static LazyLoadTexture ButtonBGAtlasClick = new LazyLoadTexture ("UI/Widgets/ButtonBGClick");

		public static LazyLoadTexture ButtonBGAtlasMouseover = new LazyLoadTexture ("UI/Widgets/ButtonBGMouseover");

		public static bool IconButton (Rect rect, Texture texture, Color baseColor, Color highlightColor, bool enabled)
		{
			bool result;
			if (texture == null) {
				result = false;
			}
			else {
				if (!enabled) {
					GUI.color = Button.InactiveButtonColor;
				}
				else {
					GUI.color = Color.white;
				}
				Texture2D texture2 = Button.ButtonBGAtlas.Texture;
				if (enabled) {
					if (rect.Contains (Event.current.mousePosition)) {
						texture2 = Button.ButtonBGAtlasMouseover.Texture;
						if (Input.GetMouseButton (0)) {
							texture2 = Button.ButtonBGAtlasClick.Texture;
						}
					}
				}
				Widgets.DrawAtlas (rect, texture2);
				Rect rect2 = new Rect (rect.x + rect.width / 2 - (float)(texture.width / 2), rect.y + rect.height / 2 - (float)(texture.height / 2), (float)texture.width, (float)texture.height);
				if (!enabled) {
					GUI.color = Button.InactiveButtonColor;
				}
				else {
					GUI.color = baseColor;
				}
				if (enabled && rect.Contains (Event.current.mousePosition)) {
					GUI.color = highlightColor;
				}
				GUI.DrawTexture (rect2, texture);
				GUI.color = Color.white;
				result = (enabled && Widgets.InvisibleButton (rect));
			}
			return result;
		}

		public static bool ImageButton (Rect butRect, Texture2D tex)
		{
			return Button.ImageButton (butRect, tex, GenUI.MouseoverColor);
		}

		public static bool ImageButton (Rect butRect, Texture2D tex, Color baseColor, Color mouseOverColor)
		{
			if (butRect.Contains (Event.current.mousePosition)) {
				GUI.color = mouseOverColor;
			}
			GUI.DrawTexture (butRect, tex);
			GUI.color = baseColor;
			return Widgets.InvisibleButton (butRect);
		}

		public static bool ImageButton (Rect butRect, Texture2D tex, Color highlightColor)
		{
			Color color = GUI.color;
			if (butRect.Contains (Event.current.mousePosition)) {
				GUI.color = highlightColor;
			}
			GUI.DrawTexture (butRect, tex);
			GUI.color = color;
			return Widgets.InvisibleButton (butRect);
		}

		public static bool TextButton (Rect rect, string label, bool drawBackground, bool doMouseoverSound, bool enabled)
		{
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			GUI.color = enabled ? Color.white : Button.InactiveButtonColor;
			if (drawBackground) {
				Texture2D texture = Button.ButtonBGAtlas.Texture;
				if (enabled && rect.Contains (Event.current.mousePosition)) {
					texture = Button.ButtonBGAtlasMouseover.Texture;
					if (Input.GetMouseButton (0)) {
						texture = Button.ButtonBGAtlasClick.Texture;
					}
				}
				Widgets.DrawAtlas (rect, texture);
			}
			if (doMouseoverSound) {
				MouseoverSounds.DoRegion (rect);
			}
			if (!drawBackground) {
				if (enabled && rect.Contains (Event.current.mousePosition)) {
					GUI.color = Button.MouseoverOptionColor;
				}
			}
			if (drawBackground) {
				Text.Anchor = TextAnchor.MiddleCenter;
			}
			else {
				Text.Anchor = TextAnchor.MiddleLeft;
			}
			Widgets.Label (rect, label);
			Text.Anchor = anchor;
			GUI.color = color;
			return enabled && Widgets.InvisibleButton (rect);
		}
	}

	public class ColonistBar
	{
		public delegate void SelectedGroupChangedHandler (ColonistBarGroup group);

		protected static Color BrowseButtonColor = new Color (1, 1, 1, 0.15f);

		protected static Color BrowseButtonHighlightColor = new Color (1, 1, 1, 0.5f);

		protected static Color GroupNameColor = new Color (0.85f, 0.85f, 0.85f);

		protected static float GroupNameDisplayDuration = 1;

		protected static float GroupNameEaseOutDuration = 0.4f;

		protected static float GroupNameEaseOutStart = ColonistBar.GroupNameDisplayDuration - ColonistBar.GroupNameEaseOutDuration;

		public static LazyLoadTexture BrowseGroupsDown = new LazyLoadTexture ("EdB/Interface/ColonistBar/BrowseGroupDown");

		protected static readonly bool LoggingEnabled = false;

		public static LazyLoadTexture BrowseGroupsUp = new LazyLoadTexture ("EdB/Interface/ColonistBar/BrowseGroupUp");

		protected bool alwaysShowGroupName = false;

		protected List<TrackedColonist> slots = new List<TrackedColonist> ();

		protected GameObject drawerGameObject = null;

		protected ColonistBarDrawer drawer = null;

		protected ColonistBarGroup currentGroup = null;

		protected float lastKeyTime = 0;

		protected KeyCode lastKey;

		protected PreferenceSmallIcons preferenceSmallIcons = new PreferenceSmallIcons ();

		protected string currentGroupId = "";

		protected List<ColonistBarGroup> groups = new List<ColonistBarGroup> ();

		private bool displayGroupName = true;

		protected float squadNameDisplayTimestamp = 0;

		protected List<KeyBindingDef> squadSelectionBindings = new List<KeyBindingDef> ();

		protected PreferenceEnabled preferenceEnabled = new PreferenceEnabled ();

		protected List<IPreference> preferences = new List<IPreference> ();

		protected bool enableGroups = false;

		protected KeyBindingDef previousGroupKeyBinding = null;

		protected KeyBindingDef nextGroupKeyBinding = null;

		public bool AlwaysShowGroupName {
			get {
				return this.alwaysShowGroupName;
			}
			set {
				this.alwaysShowGroupName = value;
			}
		}

		public ColonistBarGroup CurrentGroup {
			get {
				return this.currentGroup;
			}
			set {
				bool flag = value != this.currentGroup;
				if (flag) {
					this.currentGroup = value;
					if (this.currentGroup != null) {
						if (!string.IsNullOrEmpty (this.currentGroupId) && this.currentGroup.Id != this.currentGroupId) {
							this.ResetGroupNameDisplay ();
						}
						this.currentGroupId = this.currentGroup.Id;
					}
					else {
						this.currentGroupId = "";
					}
					if (this.SelectedGroupChanged != null) {
						this.SelectedGroupChanged (this.currentGroup);
					}
				}
				if (this.currentGroup != null) {
					this.drawer.Slots = this.currentGroup.Colonists;
				}
			}
		}

		public bool DisplayGroupName {
			get {
				return this.displayGroupName;
			}
			set {
				this.displayGroupName = value;
			}
		}

		public ColonistBarDrawer Drawer {
			get {
				return this.drawer;
			}
		}

		public bool EnableGroups {
			get {
				return this.enableGroups;
			}
			set {
				this.enableGroups = value;
			}
		}

		public bool GroupsBrowsable {
			get {
				return this.groups.Count > 1;
			}
		}

		public IEnumerable<IPreference> Preferences {
			get {
				return this.preferences;
			}
		}

		public ColonistBar ()
		{
			this.preferences.Add (this.preferenceEnabled);
			this.preferences.Add (this.preferenceSmallIcons);
			this.Reset ();
		}

		public void AddGroup (ColonistBarGroup group)
		{
			this.groups.Add (group);
		}

		public void Draw ()
		{
			if (this.preferenceEnabled.Value) {
				if (this.currentGroup != null && this.currentGroup.Colonists.Count != 0) {
					if (this.drawer != null) {
						this.drawer.Draw ();
						this.drawer.DrawTexturesForSlots ();
						this.drawer.DrawToggleButton ();
					}
					if (this.enableGroups) {
						bool value = this.preferenceSmallIcons.Value;
						if (value && !this.drawer.SmallColonistIcons) {
							this.drawer.UseSmallIcons ();
						}
						else if (!value && this.drawer.SmallColonistIcons) {
							this.drawer.UseLargeIcons ();
						}
						if (this.GroupsBrowsable) {
							GUI.color = ColonistBar.BrowseButtonColor;
							Rect butRect = (!value) ? new Rect (592, 25, 32, 18) : new Rect (592, 15, 32, 18);
							if (butRect.Contains (Event.current.mousePosition)) {
								this.squadNameDisplayTimestamp = Time.time;
							}
							if (Button.ImageButton (butRect, ColonistBar.BrowseGroupsUp.Texture, ColonistBar.BrowseButtonHighlightColor)) {
								this.SelectNextGroup (-1);
							}
							GUI.color = ColonistBar.BrowseButtonColor;
							butRect = ((!value) ? new Rect (592, 49, 32, 18) : new Rect (592, 39, 32, 18));
							if (butRect.Contains (Event.current.mousePosition)) {
								this.squadNameDisplayTimestamp = Time.time;
							}
							if (Button.ImageButton (butRect, ColonistBar.BrowseGroupsDown.Texture, ColonistBar.BrowseButtonHighlightColor)) {
								this.SelectNextGroup (1);
							}
							GUI.color = Color.white;
						}
						bool flag = false;
						Color groupNameColor = ColonistBar.GroupNameColor;
						Color black = Color.black;
						if (this.alwaysShowGroupName) {
							flag = true;
						}
						else if (this.displayGroupName && this.squadNameDisplayTimestamp > 0) {
							float time = Time.time;
							float num = time - this.squadNameDisplayTimestamp;
							if (num < ColonistBar.GroupNameDisplayDuration) {
								flag = true;
								if (num > ColonistBar.GroupNameEaseOutStart) {
									float num2 = num - ColonistBar.GroupNameEaseOutStart;
									float groupNameEaseOutDuration = ColonistBar.GroupNameEaseOutDuration;
									float num3 = 1;
									float num4 = -1;
									num2 /= groupNameEaseOutDuration;
									float num5 = num4 * num2 * num2 + num3;
									groupNameColor = new Color (ColonistBar.GroupNameColor.r, ColonistBar.GroupNameColor.g, ColonistBar.GroupNameColor.b, num5);
									black.a = groupNameColor.a;
								}
							}
							else {
								this.squadNameDisplayTimestamp = 0;
							}
						}
						if (flag) {
							Rect rect = (!value) ? new Rect (348, 29, 225, 36) : new Rect (348, 20, 225, 36);
							if (!this.GroupsBrowsable) {
								rect.x = rect.x + 48;
							}
							Text.Anchor = TextAnchor.MiddleRight;
							Text.Font = GameFont.Small;
							GUI.color = black;
							Widgets.Label (new Rect (rect.x + 1, rect.y + 1, rect.width, rect.height), this.currentGroup.Name);
							if (rect.Contains (Event.current.mousePosition)) {
								GUI.color = Color.white;
							}
							else {
								GUI.color = groupNameColor;
							}
							Widgets.Label (rect, this.currentGroup.Name);
							if (Widgets.InvisibleButton (rect)) {
								this.drawer.SelectAllActive ();
							}
							Text.Anchor = TextAnchor.UpperLeft;
							Text.Font = GameFont.Small;
							GUI.color = Color.white;
						}
					}
				}
			}
		}

		protected ColonistBarGroup FindNextGroup (int direction)
		{
			ColonistBarGroup result;
			if (this.groups.Count == 0) {
				result = null;
			}
			else if (this.groups.Count == 1) {
				result = this.groups [0];
			}
			else {
				int num = this.groups.IndexOf (this.currentGroup);
				if (num == -1) {
					result = this.groups [0];
				}
				else {
					num += direction;
					if (num < 0) {
						num = this.groups.Count - 1;
					}
					else if (num > this.groups.Count - 1) {
						num = 0;
					}
					result = this.groups [num];
				}
			}
			return result;
		}

		protected void Message (string message)
		{
			if (ColonistBar.LoggingEnabled) {
				Log.Message (message);
			}
		}

		public void Reset ()
		{
			this.drawerGameObject = new GameObject ("ColonistBarDrawer");
			this.drawer = this.drawerGameObject.AddComponent<ColonistBarDrawer> ();
			this.drawer.enabled = false;
		}

		protected void ResetGroupNameDisplay ()
		{
			if (this.currentGroup != null && this.displayGroupName) {
				this.squadNameDisplayTimestamp = Time.time;
			}
			else {
				this.squadNameDisplayTimestamp = 0;
			}
		}

		public void SelectAllPawns ()
		{
			this.drawer.SelectAllActive ();
		}

		public void SelectNextGroup (int direction)
		{
			this.CurrentGroup = this.FindNextGroup (direction);
		}

		public void UpdateGroups (List<ColonistBarGroup> groups, ColonistBarGroup selected)
		{
			this.Message (string.Concat (new object[] {
				"UpdateGroups(",
				groups.Count,
				", ",
				(selected == null) ? "null" : selected.Name,
				")"
			}));
			foreach (ColonistBarGroup current in groups) {
				this.Message ("group = " + ((current == null) ? "null" : current.Name));
			}
			this.Message ("Already selected: " + ((this.currentGroup == null) ? "null" : this.currentGroup.Name));
			if (selected == null && groups.Count > 0) {
				selected = this.FindNextGroup (-1);
				this.Message ("Previous group: " + ((selected == null) ? "null" : selected.Name));
			}
			this.groups.Clear ();
			this.groups.AddRange (groups);
			if (selected == null && groups.Count > 0) {
				selected = groups [0];
			}
			this.CurrentGroup = selected;
		}

		public void UpdateScreenSize (int width, int height)
		{
			this.drawer.SizeCamera (width, height);
		}

		public event ColonistBar.SelectedGroupChangedHandler SelectedGroupChanged;
	}

	public class ColonistBarDrawer : MonoBehaviour
	{
		public static Material SlotBackgroundMat = null;

		public static Vector2 HealthOffsetLarge = new Vector2 (52, 6);

		public static Vector2 HealthSizeLarge = new Vector2 (5, 46);

		public static Vector2 SlotSizeSmall = new Vector2 (56, 38);

		public static Vector2 NormalSlotPaddingSmall = new Vector2 (12, 24);

		public static Vector2 WideSlotPaddingSmall = new Vector2 (36, 24);

		public static Vector2 BackgroundSizeSmall = new Vector2 (40, 36);

		public static Vector2 MentalHealthSizeLarge = new Vector2 (76, 76);

		public static Vector2 PortraitOffsetLarge = new Vector2 (5, 5);

		public static Vector2 PortraitSizeLarge = new Vector2 (46, 46);

		public static Vector2 BodySizeLarge = new Vector2 (76, 76);

		public static Vector2 BodyOffsetLarge = new Vector2 (-9, -11);

		public static Vector2 HeadSizeLarge = new Vector2 (76, 76);

		public static Vector2 HeadOffsetLarge = new Vector2 (-9, -26);

		public static Vector2 MentalHealthOffsetLarge = new Vector2 (-2, -22);

		public static Vector2 BackgroundOffsetSmall = new Vector2 (7, 0);

		public static Vector2 HealthOffsetSmall = new Vector2 (41, 4);

		public static Vector2 HealthSizeSmall = new Vector2 (3, 28);

		public static Color ColorBroken = new Color (0.65f, 0.9f, 0.93f);

		public static Color ColorPsycho = new Color (0.9f, 0.2f, 0.5f);

		public static Color ColorDead = new Color (0.5f, 0.5f, 0.5f, 1);

		public static Color ColorFrozen = new Color (0.7f, 0.7f, 0.9f, 1);

		public static Color ColorNameUnderlay = new Color (0, 0, 0, 0.6f);

		public static Vector2 MentalHealthSizeSmall = new Vector2 (52, 52);

		public static Vector2 PortraitOffsetSmall = new Vector2 (11, 3);

		public static Vector2 PortraitSizeSmall = new Vector2 (28, 28);

		public static Vector2 BodySizeSmall = new Vector2 (50, 50);

		public static Vector2 BodyOffsetSmall = new Vector2 (1, -7);

		public static Vector2 HeadSizeSmall = new Vector2 (50, 50);

		public static Vector2 HeadOffsetSmall = new Vector2 (1, -20);

		public static Vector2 MentalHealthOffsetSmall = new Vector2 (7, -17);

		public static Vector2 BackgroundOffsetLarge = new Vector2 (0, 0);

		public static LazyLoadTexture UnhappyTex = new LazyLoadTexture ("Things/Pawn/Effects/Unhappy");

		public static LazyLoadTexture MentalBreakImminentTex = new LazyLoadTexture ("Things/Pawn/Effects/MentalStateImminent");

		public static LazyLoadTexture ToggleButton = new LazyLoadTexture ("EdB/Interface/ColonistBar/ToggleBar");

		public static Vector2 StartingPosition = new Vector2 (640, 16);

		public static Vector2 SlotSize;

		public static Vector2 SlotPadding;

		public static Vector2 BackgroundSize;

		public static Material SlotSelectedMatSmall = null;

		public static Material SlotBordersMat = null;

		public static Material SlotSelectedMat = null;

		public static Material SlotBackgroundMatLarge = null;

		public static Material SlotBordersMatLarge = null;

		public static Material SlotSelectedMatLarge = null;

		public static Material SlotBackgroundMatSmall = null;

		public static Material SlotBordersMatSmall = null;

		public static Vector2 BackgroundOffset;

		public static Vector2 HealthOffset;

		public static Vector2 HealthSize;

		public static Vector2 MaxLabelSize;

		public static Vector2 SlotSizeLarge = new Vector2 (62, 56);

		public static Vector2 NormalSlotPaddingLarge = new Vector2 (16, 24);

		public static Vector2 WideSlotPaddingLarge = new Vector2 (36, 24);

		public static Vector2 BackgroundSizeLarge = new Vector2 (64, 64);

		public static Vector2 MentalHealthSize;

		public static Vector2 PortraitOffset;

		public static Vector2 PortraitSize;

		public static Vector2 BodySize;

		public static Vector2 BodyOffset;

		public static Vector2 HeadSize;

		public static Vector2 HeadOffset;

		public static Vector2 MentalHealthOffset;

		protected float doubleClickTime = -1;

		protected Camera camera;

		protected bool visible = true;

		protected Dictionary<Material, Material> deadMaterials = new Dictionary<Material, Material> ();

		protected Dictionary<Material, Material> cryptosleepMaterials = new Dictionary<Material, Material> ();

		protected List<TrackedColonist> slots;

		protected bool smallColonistIcons = false;

		protected Mesh backgroundMesh;

		protected Mesh bodyMesh;

		protected Mesh headMesh;

		protected MaterialPropertyBlock deadPropertyBlock = new MaterialPropertyBlock ();

		protected MaterialPropertyBlock cryptosleepPropertyBlock = new MaterialPropertyBlock ();

		protected SelectorUtility pawnSelector = new SelectorUtility ();

		public List<TrackedColonist> Slots {
			get {
				return this.slots;
			}
			set {
				this.slots = value;
			}
		}

		public bool SmallColonistIcons {
			get {
				return this.smallColonistIcons;
			}
		}

		public bool Visible {
			get {
				return this.visible;
			}
			set {
				this.visible = value;
			}
		}

		protected bool CanSelect (Pawn pawn)
		{
			return ThingSelectionUtility.SelectableNow (pawn);
		}

		public void Draw ()
		{
			if (this.visible) {
				this.camera.Render ();
			}
		}

		protected void DrawTextureForSlot (TrackedColonist slot, Vector2 position)
		{
			Pawn pawn = slot.Pawn;
			if (Widgets.InvisibleButton (new Rect (position.x, position.y, ColonistBarDrawer.SlotSize.x, ColonistBarDrawer.SlotSize.y))) {
				int button = Event.current.button;
				if (button == 2) {
					Pawn carrier = slot.Carrier;
					if (carrier == null) {
						if (slot.InMentalState) {
							this.SelectAllNotSane ();
						}
						else if (slot.Controllable) {
							this.SelectAllActive ();
						}
						else {
							this.SelectAllDead ();
						}
					}
				}
				if (button == 0) {
					if (Time.time - this.doubleClickTime < 0.3f) {
						if (!pawn.Dead) {
							Pawn carrier = slot.Carrier;
							if (carrier == null) {
								if (!slot.Missing) {
									Find.CameraMap.JumpTo (pawn.Position);
								}
							}
							else {
								Find.CameraMap.JumpTo (carrier.Position);
							}
						}
						else if (slot.Corpse != null) {
							Find.CameraMap.JumpTo (slot.Corpse.Position);
						}
						this.doubleClickTime = -1;
					}
					else {
						if (!pawn.Dead && !ThingUtility.DestroyedOrNull (pawn)) {
							if ((Event.current.shift || Event.current.control) && Find.Selector.IsSelected (pawn)) {
								Find.Selector.Deselect (pawn);
							}
							else {
								Pawn carrier = slot.Carrier;
								if (carrier == null) {
									if (!Event.current.alt) {
										if (this.CanSelect (pawn)) {
											this.pawnSelector.SelectThing (pawn, Event.current.shift);
										}
									}
									else if (slot.InMentalState) {
										this.SelectAllNotSane ();
									}
									else {
										this.SelectAllActive ();
									}
								}
							}
						}
						else {
							if (slot.Corpse == null || slot.Missing) {
								this.doubleClickTime = -1;
								return;
							}
							if (Event.current.shift && Find.Selector.IsSelected (slot.Corpse)) {
								Find.Selector.Deselect (slot.Corpse);
							}
							else if (Event.current.alt) {
								this.SelectAllDead ();
							}
							else {
								this.pawnSelector.SelectThing (slot.Corpse, Event.current.shift);
							}
						}
						if (!Event.current.shift) {
							this.doubleClickTime = Time.time;
						}
					}
				}
				else {
					this.doubleClickTime = -1;
				}
				if (button == 1) {
					List<FloatMenuOption> list = new List<FloatMenuOption> ();
					if (slot.Missing || slot.Corpse != null) {
						string text = slot.Missing ? Translator.Translate ("EdB.ColonistBar.RemoveMissingColonist") : Translator.Translate ("EdB.ColonistBar.RemoveDeadColonist");
						list.Add (new FloatMenuOption (text, delegate {
							ColonistTracker.Instance.StopTrackingPawn (slot.Pawn);
						}, MenuOptionPriority.Medium, null, null));
					}
					list.Add (new FloatMenuOption (Translator.Translate ("EdB.ColonistBar.HideColonistBar"), delegate {
						this.visible = false;
					}, MenuOptionPriority.Medium, null, null));
					FloatMenu floatMenu = new FloatMenu (list, "", false, false);
					Find.WindowStack.Add (floatMenu);
				}
			}
			if (Event.current.type == EventType.Repaint) {
				if (!slot.Dead) {
					if (slot.Incapacitated) {
						GUI.color = new Color (0.7843f, 0, 0);
					}
					else if ((double)slot.HealthPercent < 0.95f) {
						GUI.color = new Color (0.7843f, 0.7843f, 0);
					}
					else {
						GUI.color = new Color (0, 0.7843f, 0);
					}
					if (slot.Missing) {
						GUI.color = new Color (0.4824f, 0.4824f, 0.4824f);
					}
					float num = ColonistBarDrawer.HealthSize.y * slot.HealthPercent;
					GUI.DrawTexture (new Rect (position.x + ColonistBarDrawer.HealthOffset.x, position.y + ColonistBarDrawer.HealthOffset.y + ColonistBarDrawer.HealthSize.y - num, ColonistBarDrawer.HealthSize.x, num), BaseContent.WhiteTex);
				}
				Vector2 vector = Text.CalcSize (pawn.LabelBaseShort);
				if (vector.x > ColonistBarDrawer.MaxLabelSize.x) {
					vector.x = ColonistBarDrawer.MaxLabelSize.x;
				}
				vector.x += 4;
				GUI.color = ColonistBarDrawer.ColorNameUnderlay;
				GUI.DrawTexture (new Rect (position.x + ColonistBarDrawer.SlotSize.x / 2 - vector.x / 2, position.y + ColonistBarDrawer.PortraitSize.y, vector.x, 12), BaseContent.BlackTex);
				Text.Font = GameFont.Tiny;
				GUI.skin.label.alignment = TextAnchor.UpperCenter;
				Text.Anchor = TextAnchor.UpperCenter;
				Color color = Color.white;
				MentalStateDef mentalState = slot.MentalState;
				if (mentalState != null) {
					color = mentalState.nameColor;
				}
				GUI.color = color;
				Widgets.Label (new Rect (position.x + ColonistBarDrawer.SlotSize.x / 2 - vector.x / 2, position.y + ColonistBarDrawer.PortraitSize.y - 2, vector.x, 20), pawn.LabelBaseShort);
				if (slot.Drafted) {
					vector.x -= 4;
					GUI.DrawTexture (new Rect (position.x + ColonistBarDrawer.SlotSize.x / 2 - vector.x / 2, position.y + ColonistBarDrawer.PortraitSize.y + 11, vector.x, 1), BaseContent.WhiteTex);
				}
				Text.Anchor = TextAnchor.UpperLeft;
				string text2 = null;
				if (slot.Missing) {
					text2 = Translator.Translate ("EdB.ColonistBar.Status.MISSING");
				}
				else if (slot.Corpse != null) {
					text2 = Translator.Translate ("EdB.ColonistBar.Status.DEAD");
				}
				else if (slot.Captured) {
					text2 = Translator.Translate ("EdB.ColonistBar.Status.KIDNAPPED");
				}
				else if (slot.Cryptosleep) {
					text2 = Translator.Translate ("EdB.ColonistBar.Status.CRYPTOSLEEP");
				}
				else if (mentalState != null) {
					if (mentalState == MentalStateDefOf.Berserk) {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.RAMPAGE");
					}
					else if (mentalState == MentalStateDefOf.BingingAlcohol) {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.BINGING");
					}
					else if (mentalState == MentalStateDefOf.SocialFighting) {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.FIGHT");
					}
					else if (mentalState == MentalStateDefOf.PanicFlee) {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.PANIC");
					}
					else if (mentalState.defName == "ConfusedWander") {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.DAZED");
					}
					else if (mentalState == MentalStateDefOf.DazedWander) {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.DAZED");
					}
					else {
						text2 = Translator.Translate ("EdB.ColonistBar.Status.BROKEN");
					}
				}
				if (text2 != null) {
					Vector2 vector2 = Text.CalcSize (text2);
					vector2.x += 4;
					GUI.color = new Color (0, 0, 0, 0.4f);
					GUI.DrawTexture (new Rect (position.x + ColonistBarDrawer.SlotSize.x / 2 - vector2.x / 2, position.y + ColonistBarDrawer.PortraitSize.y + 12, vector2.x, 13), BaseContent.BlackTex);
					Text.Font = GameFont.Tiny;
					GUI.skin.label.alignment = TextAnchor.UpperCenter;
					Text.Anchor = TextAnchor.UpperCenter;
					GUI.color = color;
					Widgets.Label (new Rect (position.x + ColonistBarDrawer.SlotSize.x / 2 - vector2.x / 2, position.y + ColonistBarDrawer.PortraitSize.y + 10, vector2.x, 20), text2);
					Text.Anchor = TextAnchor.UpperLeft;
				}
				GUI.color = new Color (1, 1, 1);
				if (!slot.Cryptosleep) {
					if (slot.MentalBreakWarningLevel == 2 && (double)Time.time % 1.2f < 0.4f) {
						ColonistBarDrawer.MentalBreakImminentTex.Draw (new Rect (position.x + ColonistBarDrawer.PortraitOffset.x, position.y + ColonistBarDrawer.PortraitOffset.y, ColonistBarDrawer.MentalHealthSize.x, ColonistBarDrawer.MentalHealthSize.y));
					}
					else if (slot.MentalBreakWarningLevel == 1 && (double)Time.time % 1.2f < 0.4f) {
						ColonistBarDrawer.UnhappyTex.Draw (new Rect (position.x + ColonistBarDrawer.MentalHealthOffset.x, position.y + ColonistBarDrawer.MentalHealthOffset.y, ColonistBarDrawer.MentalHealthSize.x, ColonistBarDrawer.MentalHealthSize.y));
					}
				}
			}
		}

		public void DrawTexturesForSlots ()
		{
			if (this.visible && this.slots != null && this.slots.Count != 0) {
				Vector2 startingPosition = ColonistBarDrawer.StartingPosition;
				float num = (float)Screen.width;
				foreach (TrackedColonist current in this.slots) {
					this.DrawTextureForSlot (current, startingPosition);
					startingPosition.x += ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x;
					if (startingPosition.x + ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x > num) {
						startingPosition.y += ColonistBarDrawer.SlotSize.y + ColonistBarDrawer.SlotPadding.y;
						startingPosition.x = ColonistBarDrawer.StartingPosition.x;
					}
				}
			}
		}

		public void DrawToggleButton ()
		{
			if (!this.visible) {
				Texture2D texture = ColonistBarDrawer.ToggleButton.Texture;
				Rect rect = new Rect ((float)(Screen.width - texture.width - 16), ColonistBarDrawer.StartingPosition.y + 4, (float)texture.width, (float)texture.height);
				ColonistBarDrawer.ToggleButton.Draw (rect);
				if (Widgets.InvisibleButton (rect)) {
					SoundStarter.PlayOneShotOnCamera (SoundDefOf.TickTiny);
					this.visible = true;
				}
			}
		}

		protected Material GetDeadMaterial (Material material)
		{
			Material material2;
			if (!this.deadMaterials.TryGetValue (material, out material2)) {
				material2 = new Material (material);
				this.deadMaterials [material] = material2;
			}
			return material2;
		}

		protected Material GetFrozenMaterial (Material material)
		{
			Material material2;
			if (!this.cryptosleepMaterials.TryGetValue (material, out material2)) {
				material2 = new Material (material);
				this.cryptosleepMaterials [material] = material2;
			}
			return material2;
		}

		public void OnGUI ()
		{
			if (this.slots != null && this.slots.Count != 0) {
				Vector2 startingPosition = ColonistBarDrawer.StartingPosition;
				float num = (float)Screen.width;
				foreach (TrackedColonist current in this.slots) {
					this.RenderSlot (current, startingPosition);
					startingPosition.x += ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x;
					if (startingPosition.x + ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x > num) {
						startingPosition.y += ColonistBarDrawer.SlotSize.y + ColonistBarDrawer.SlotPadding.y;
						startingPosition.x = ColonistBarDrawer.StartingPosition.x;
					}
				}
			}
		}

		protected void RenderSlot (TrackedColonist slot, Vector2 position)
		{
			if (Event.current.type == EventType.Repaint) {
				Rot4 south = Rot4.South;
				Pawn pawn = slot.Pawn;
				PawnGraphicSet graphics = pawn.Drawer.renderer.graphics;
				if (!graphics.AllResolved) {
					graphics.ResolveAllGraphics ();
				}
				bool flag = slot.Dead || slot.Missing;
				bool cryptosleep = slot.Cryptosleep;
				Quaternion identity = Quaternion.identity;
				Vector3 one = Vector3.one;
				Graphics.DrawMesh (this.backgroundMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.BackgroundOffset.x, position.y + ColonistBarDrawer.BackgroundOffset.y, 0), identity, one), ColonistBarDrawer.SlotBackgroundMat, 1, this.camera, 0, null);
				MaterialPropertyBlock materialPropertyBlock = null;
				if (flag) {
					materialPropertyBlock = this.deadPropertyBlock;
				}
				else if (slot.Cryptosleep) {
					materialPropertyBlock = this.cryptosleepPropertyBlock;
				}
				float num = 1;
				Material material;
				foreach (Material current in graphics.MatsBodyBaseAt (south, 0)) {
					material = current;
					if (flag) {
						material = this.GetDeadMaterial (current);
					}
					else if (cryptosleep) {
						material = this.GetFrozenMaterial (current);
					}
					Graphics.DrawMesh (this.bodyMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.PortraitOffset.x, position.y + ColonistBarDrawer.PortraitOffset.y, num), identity, one), material, 1, this.camera, 0, materialPropertyBlock);
					num += 1;
				}
				Material material2;
				for (int i = 0; i < graphics.apparelGraphics.Count; i++) {
					ApparelGraphicRecord apparelGraphicRecord = graphics.apparelGraphics [i];
					if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayer.Shell) {
						material2 = apparelGraphicRecord.graphic.MatAt (south, null);
						material2 = graphics.flasher.GetDamagedMat (material2);
						material = material2;
						if (flag) {
							material = this.GetDeadMaterial (material2);
						}
						else if (cryptosleep) {
							material = this.GetFrozenMaterial (material2);
						}
						Graphics.DrawMesh (this.bodyMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.PortraitOffset.x, position.y + ColonistBarDrawer.PortraitOffset.y, num), identity, one), material, 1, this.camera, 0, materialPropertyBlock);
						num += 1;
					}
				}
				Graphics.DrawMesh (this.backgroundMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.BackgroundOffset.x, position.y + ColonistBarDrawer.BackgroundOffset.y, num), identity, one), ColonistBarDrawer.SlotBordersMat, 1, this.camera);
				num += 1;
				if ((slot.Corpse == null) ? Find.Selector.IsSelected (pawn) : Find.Selector.IsSelected (slot.Corpse)) {
					Graphics.DrawMesh (this.backgroundMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.BackgroundOffset.x, position.y + ColonistBarDrawer.BackgroundOffset.y, num), identity, one), ColonistBarDrawer.SlotSelectedMat, 1, this.camera);
					num += 1;
				}
				material2 = pawn.Drawer.renderer.graphics.HeadMatAt (south, 0);
				material = material2;
				if (flag) {
					material = this.GetDeadMaterial (material2);
				}
				else if (cryptosleep) {
					material = this.GetFrozenMaterial (material2);
				}
				Graphics.DrawMesh (this.headMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.HeadOffset.x, position.y + ColonistBarDrawer.HeadOffset.y, num), identity, one), material, 1, this.camera, 0, materialPropertyBlock);
				num += 1;
				bool flag2 = false;
				List<ApparelGraphicRecord> apparelGraphics = graphics.apparelGraphics;
				for (int j = 0; j < apparelGraphics.Count; j++) {
					if (apparelGraphics [j].sourceApparel.def.apparel.LastLayer == ApparelLayer.Overhead) {
						flag2 = true;
						material2 = apparelGraphics [j].graphic.MatAt (south, null);
						material2 = graphics.flasher.GetDamagedMat (material2);
						material = material2;
						if (flag) {
							material = this.GetDeadMaterial (material2);
						}
						else if (cryptosleep) {
							material = this.GetFrozenMaterial (material2);
						}
						Graphics.DrawMesh (this.headMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.HeadOffset.x, position.y + ColonistBarDrawer.HeadOffset.y, num), identity, one), material, 1, this.camera, 0, materialPropertyBlock);
						num += 1;
					}
				}
				if (!flag2) {
					if (slot.Pawn.story.hairDef != null) {
						material2 = graphics.HairMatAt (south);
						material = material2;
						if (flag) {
							material = this.GetDeadMaterial (material2);
						}
						else if (cryptosleep) {
							material = this.GetFrozenMaterial (material2);
						}
						Graphics.DrawMesh (this.headMesh, Matrix4x4.TRS (new Vector3 (position.x + ColonistBarDrawer.HeadOffset.x, position.y + ColonistBarDrawer.HeadOffset.y, num), identity, one), material, 1, this.camera, 0, materialPropertyBlock);
						num += 1;
					}
				}
			}
		}

		protected void ResetMaxLabelSize ()
		{
			float num = ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x - 8;
			ColonistBarDrawer.MaxLabelSize = new Vector2 (num, 12);
		}

		public void ResetTextures ()
		{
			ColonistBarDrawer.SlotBackgroundMatLarge = MaterialPool.MatFrom ("EdB/Interface/ColonistBar/PortraitBackgroundLarge");
			ColonistBarDrawer.SlotBackgroundMatLarge.mainTexture.filterMode = 0;
			ColonistBarDrawer.SlotBordersMatLarge = MaterialPool.MatFrom ("EdB/Interface/ColonistBar/PortraitBordersLarge");
			ColonistBarDrawer.SlotBordersMatLarge.mainTexture.filterMode = 0;
			ColonistBarDrawer.SlotSelectedMatLarge = MaterialPool.MatFrom ("EdB/Interface/ColonistBar/PortraitSelectedLarge");
			ColonistBarDrawer.SlotSelectedMatLarge.mainTexture.filterMode = 0;
			ColonistBarDrawer.SlotBackgroundMatSmall = MaterialPool.MatFrom ("EdB/Interface/ColonistBar/PortraitBackgroundSmall");
			ColonistBarDrawer.SlotBackgroundMatSmall.mainTexture.filterMode = 0;
			ColonistBarDrawer.SlotBordersMatSmall = MaterialPool.MatFrom ("EdB/Interface/ColonistBar/PortraitBordersSmall");
			ColonistBarDrawer.SlotBordersMatSmall.mainTexture.filterMode = 0;
			ColonistBarDrawer.SlotSelectedMatSmall = MaterialPool.MatFrom ("EdB/Interface/ColonistBar/PortraitSelectedSmall");
			ColonistBarDrawer.SlotSelectedMatSmall.mainTexture.filterMode = 0;
		}

		protected void ResizeMeshes ()
		{
			this.backgroundMesh = new Mesh ();
			this.backgroundMesh.vertices = new Vector3[] {
				new Vector3 (0, 0, 0),
				new Vector3 (ColonistBarDrawer.BackgroundSize.x, 0, 0),
				new Vector3 (0, ColonistBarDrawer.BackgroundSize.y, 0),
				new Vector3 (ColonistBarDrawer.BackgroundSize.x, ColonistBarDrawer.BackgroundSize.y, 0)
			};
			this.backgroundMesh.uv = new Vector2[] {
				new Vector2 (0, 1),
				new Vector2 (1, 1),
				new Vector2 (0, 0),
				new Vector2 (1, 0)
			};
			this.backgroundMesh.triangles = new int[] {
				0,
				1,
				2,
				1,
				3,
				2
			};
			this.bodyMesh = new Mesh ();
			this.bodyMesh.vertices = new Vector3[] {
				new Vector3 (0, 0, 0),
				new Vector3 (ColonistBarDrawer.PortraitSize.x, 0, 0),
				new Vector3 (0, ColonistBarDrawer.PortraitSize.y, 0),
				new Vector3 (ColonistBarDrawer.PortraitSize.x, ColonistBarDrawer.PortraitSize.y, 0)
			};
			Vector2 vector = new Vector2 ((ColonistBarDrawer.PortraitOffset.x - ColonistBarDrawer.BodyOffset.x) / ColonistBarDrawer.BodySize.x, (ColonistBarDrawer.PortraitOffset.y - ColonistBarDrawer.BodyOffset.y) / ColonistBarDrawer.BodySize.y);
			Vector2 vector2 = new Vector2 ((ColonistBarDrawer.PortraitOffset.x - ColonistBarDrawer.BodyOffset.x + ColonistBarDrawer.PortraitSize.x) / ColonistBarDrawer.BodySize.x, (ColonistBarDrawer.PortraitOffset.y - ColonistBarDrawer.BodyOffset.y + ColonistBarDrawer.PortraitSize.y) / ColonistBarDrawer.BodySize.y);
			this.bodyMesh.uv = new Vector2[] {
				new Vector2 (vector.x, vector2.y),
				new Vector2 (vector2.x, vector2.y),
				new Vector2 (vector.x, vector.y),
				new Vector2 (vector2.x, vector.y)
			};
			this.bodyMesh.triangles = new int[] {
				0,
				1,
				2,
				1,
				3,
				2
			};
			this.headMesh = new Mesh ();
			this.headMesh.vertices = new Vector3[] {
				new Vector3 (0, 0, 0),
				new Vector3 (ColonistBarDrawer.HeadSize.x, 0, 0),
				new Vector3 (0, ColonistBarDrawer.HeadSize.y, 0),
				new Vector3 (ColonistBarDrawer.HeadSize.x, ColonistBarDrawer.HeadSize.y, 0)
			};
			this.headMesh.uv = new Vector2[] {
				new Vector2 (0, 1),
				new Vector2 (1, 1),
				new Vector2 (0, 0),
				new Vector2 (1, 0)
			};
			this.headMesh.triangles = new int[] {
				0,
				1,
				2,
				1,
				3,
				2
			};
		}

		public void SelectAllActive ()
		{
			this.pawnSelector.ClearSelection ();
			foreach (TrackedColonist current in this.slots) {
				if (current.Controllable) {
					this.pawnSelector.AddToSelection (current.Pawn);
				}
			}
		}

		public void SelectAllDead ()
		{
			this.pawnSelector.ClearSelection ();
			foreach (TrackedColonist current in this.slots) {
				if (current.HealthPercent == 0 && !current.Missing) {
					if (current.Corpse != null) {
						this.pawnSelector.AddToSelection (current.Corpse);
					}
				}
			}
		}

		public void SelectAllNotSane ()
		{
			this.pawnSelector.ClearSelection ();
			foreach (TrackedColonist current in this.slots) {
				if (current.InMentalState) {
					this.pawnSelector.AddToSelection (current.Pawn);
				}
			}
		}

		public void SizeCamera (int width, int height)
		{
			float num = (float)width * 0.5f;
			float num2 = (float)height * 0.5f;
			this.camera.orthographicSize = num2;
			this.camera.transform.position = new Vector3 (num, num2, 100);
			this.camera.transform.LookAt (new Vector3 (num, num2, 0), new Vector3 (0, -1, 0));
			this.camera.aspect = num / num2;
		}

		public void Start ()
		{
			this.camera = base.gameObject.AddComponent<Camera> ();
			this.camera.orthographic = true;
			this.camera.backgroundColor = new Color (0, 0, 0, 0);
			this.SizeCamera (Screen.width, Screen.height);
			this.camera.clearFlags = CameraClearFlags.Depth;
			this.camera.nearClipPlane = 1;
			this.camera.farClipPlane = 200;
			this.camera.depth = -1;
			this.camera.enabled = false;
			this.UseLargeIcons ();
			this.deadPropertyBlock = new MaterialPropertyBlock ();
			this.deadPropertyBlock.Clear ();
			this.deadPropertyBlock.AddColor (Shader.PropertyToID ("_Color"), ColonistBarDrawer.ColorDead);
			this.cryptosleepPropertyBlock = new MaterialPropertyBlock ();
			this.cryptosleepPropertyBlock.Clear ();
			this.cryptosleepPropertyBlock.AddColor (Shader.PropertyToID ("_Color"), ColonistBarDrawer.ColorFrozen);
		}

		public void UseLargeIcons ()
		{
			ColonistBarDrawer.SlotSize = ColonistBarDrawer.SlotSizeLarge;
			ColonistBarDrawer.BackgroundSize = ColonistBarDrawer.BackgroundSizeLarge;
			ColonistBarDrawer.BackgroundOffset = ColonistBarDrawer.BackgroundOffsetLarge;
			ColonistBarDrawer.SlotPadding = ColonistBarDrawer.NormalSlotPaddingLarge;
			ColonistBarDrawer.PortraitOffset = ColonistBarDrawer.PortraitOffsetLarge;
			ColonistBarDrawer.PortraitSize = ColonistBarDrawer.PortraitSizeLarge;
			ColonistBarDrawer.BodySize = ColonistBarDrawer.BodySizeLarge;
			ColonistBarDrawer.BodyOffset = ColonistBarDrawer.BodyOffsetLarge;
			ColonistBarDrawer.HeadSize = ColonistBarDrawer.HeadSizeLarge;
			ColonistBarDrawer.HeadOffset = ColonistBarDrawer.HeadOffsetLarge;
			ColonistBarDrawer.MentalHealthOffset = ColonistBarDrawer.MentalHealthOffsetLarge;
			ColonistBarDrawer.MentalHealthSize = ColonistBarDrawer.MentalHealthSizeLarge;
			ColonistBarDrawer.HealthOffset = ColonistBarDrawer.HealthOffsetLarge;
			ColonistBarDrawer.HealthSize = ColonistBarDrawer.HealthSizeLarge;
			ColonistBarDrawer.SlotBackgroundMat = ColonistBarDrawer.SlotBackgroundMatLarge;
			ColonistBarDrawer.SlotBordersMat = ColonistBarDrawer.SlotBordersMatLarge;
			ColonistBarDrawer.SlotSelectedMat = ColonistBarDrawer.SlotSelectedMatLarge;
			this.smallColonistIcons = false;
			this.ResizeMeshes ();
			this.ResetMaxLabelSize ();
		}

		public void UseSmallIcons ()
		{
			ColonistBarDrawer.SlotSize = ColonistBarDrawer.SlotSizeSmall;
			ColonistBarDrawer.BackgroundSize = ColonistBarDrawer.BackgroundSizeSmall;
			ColonistBarDrawer.BackgroundOffset = ColonistBarDrawer.BackgroundOffsetSmall;
			ColonistBarDrawer.SlotPadding = ColonistBarDrawer.NormalSlotPaddingSmall;
			ColonistBarDrawer.PortraitOffset = ColonistBarDrawer.PortraitOffsetSmall;
			ColonistBarDrawer.PortraitSize = ColonistBarDrawer.PortraitSizeSmall;
			ColonistBarDrawer.BodySize = ColonistBarDrawer.BodySizeSmall;
			ColonistBarDrawer.BodyOffset = ColonistBarDrawer.BodyOffsetSmall;
			ColonistBarDrawer.HeadSize = ColonistBarDrawer.HeadSizeSmall;
			ColonistBarDrawer.HeadOffset = ColonistBarDrawer.HeadOffsetSmall;
			ColonistBarDrawer.MentalHealthOffset = ColonistBarDrawer.MentalHealthOffsetSmall;
			ColonistBarDrawer.MentalHealthSize = ColonistBarDrawer.MentalHealthSizeSmall;
			ColonistBarDrawer.HealthOffset = ColonistBarDrawer.HealthOffsetSmall;
			ColonistBarDrawer.HealthSize = ColonistBarDrawer.HealthSizeSmall;
			ColonistBarDrawer.SlotBackgroundMat = ColonistBarDrawer.SlotBackgroundMatSmall;
			ColonistBarDrawer.SlotBordersMat = ColonistBarDrawer.SlotBordersMatSmall;
			ColonistBarDrawer.SlotSelectedMat = ColonistBarDrawer.SlotSelectedMatSmall;
			this.smallColonistIcons = true;
			this.ResizeMeshes ();
			this.ResetMaxLabelSize ();
		}
	}

	public class ColonistBarGroup
	{
		private List<TrackedColonist> colonists;

		private string name;

		private bool visible;

		private string id = "";

		public List<TrackedColonist> Colonists {
			get {
				return this.colonists;
			}
		}

		public string Id {
			get {
				return this.id;
			}
			set {
				this.id = value;
			}
		}

		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}

		public int OrderHash {
			get {
				int num = 33;
				foreach (TrackedColonist current in this.colonists) {
					num = 17 * num + current.Pawn.GetUniqueLoadID ().GetHashCode ();
				}
				num = 17 * num + this.name.GetHashCode ();
				return num;
			}
		}

		public bool Visible {
			get {
				return this.visible;
			}
			set {
				this.visible = value;
			}
		}

		public ColonistBarGroup (string name, List<TrackedColonist> colonists)
		{
			this.colonists = colonists;
			this.name = name;
		}

		public ColonistBarGroup (int reserve)
		{
			if (reserve > 0) {
				this.colonists = new List<TrackedColonist> (reserve);
			}
			else {
				this.colonists = new List<TrackedColonist> ();
			}
		}

		public ColonistBarGroup ()
		{
			this.colonists = new List<TrackedColonist> ();
		}

		public void Add (TrackedColonist colonist)
		{
			if (!this.colonists.Contains (colonist)) {
				this.colonists.Add (colonist);
			}
		}

		public void Clear ()
		{
			this.colonists.Clear ();
		}

		public bool Remove (TrackedColonist colonist)
		{
			return this.colonists.Remove (colonist);
		}

		public bool Remove (Pawn pawn)
		{
			int num = this.colonists.FindIndex ((TrackedColonist c) => c.Pawn == pawn);
			bool result;
			if (num != -1) {
				this.colonists.RemoveAt (num);
				result = true;
			}
			else {
				result = false;
			}
			return result;
		}
	}

	public class ColonistBarSlot
	{
		public Pawn pawn;

		protected bool remove = false;

		protected SelectorUtility pawnSelector = new SelectorUtility ();

		protected float missingTime = 0;

		public bool drafted = false;

		public float health = 0;

		public bool kidnapped = false;

		public Corpse corpse = null;

		public bool missing = false;

		public bool incapacitated = false;

		public bool dead = false;

		public MentalStateDef sanity = null;

		public int psychologyLevel = 0;

		public Corpse Corpse {
			get {
				return this.corpse;
			}
			set {
				this.corpse = value;
			}
		}

		public bool Missing {
			get {
				return this.missing;
			}
			set {
				this.missing = value;
			}
		}

		public float MissingTime {
			get {
				return this.missingTime;
			}
			set {
				this.missingTime = value;
			}
		}

		public Pawn Pawn {
			get {
				return this.pawn;
			}
		}

		public bool Remove {
			get {
				return this.remove;
			}
			set {
				this.remove = value;
			}
		}

		public ColonistBarSlot (Pawn pawn)
		{
			this.pawn = pawn;
		}

		public Pawn FindCarrier ()
		{
			Pawn result;
			if (this.pawn.holder != null && this.pawn.holder.owner != null) {
				Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
				if (pawn_CarryTracker != null && pawn_CarryTracker.pawn != null) {
					result = pawn_CarryTracker.pawn;
					return result;
				}
			}
			result = null;
			return result;
		}

		public void Update ()
		{
			if (this.pawn != null) {
				this.incapacitated = false;
				if (this.pawn.health != null) {
					this.health = this.pawn.health.summaryHealth.SummaryHealthPercent;
					this.incapacitated = this.pawn.health.Downed;
				}
				else {
					this.health = 0;
				}
				this.kidnapped = false;
				if (this.pawn.holder != null) {
					if (this.pawn.Destroyed) {
						this.missing = true;
					}
					else if (this.pawn.holder.owner != null) {
						Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
						if (pawn_CarryTracker != null && pawn_CarryTracker.pawn != null && pawn_CarryTracker.pawn.Faction != null) {
							if (pawn_CarryTracker.pawn.Faction != Faction.OfColony && pawn_CarryTracker.pawn.Faction.RelationWith (Faction.OfColony, false).hostile) {
								this.kidnapped = true;
							}
						}
					}
				}
				this.dead = this.pawn.Dead;
				if (this.dead) {
					if (this.WasReplaced (this.pawn)) {
						this.dead = false;
					}
				}
				this.sanity = null;
				if (this.pawn.mindState != null && this.pawn.InMentalState) {
					this.sanity = this.pawn.MentalStateDef;
				}
				this.drafted = (!this.dead && this.pawn.Drafted);
				this.psychologyLevel = 0;
				if (this.pawn.mindState != null && this.pawn.mindState.mentalStateStarter != null && !this.pawn.Downed && !this.pawn.Dead) {
					if (this.pawn.mindState.mentalStateStarter.HardMentalStateImminent) {
						this.psychologyLevel = 2;
					}
					else if (this.pawn.mindState.mentalStateStarter.MentalStateApproaching) {
						this.psychologyLevel = 1;
					}
				}
			}
		}

		protected bool WasReplaced (Pawn pawn)
		{
			bool result;
			foreach (Pawn current in Find.MapPawns.FreeColonists) {
				if (current.GetUniqueLoadID () == pawn.GetUniqueLoadID ()) {
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}

	public delegate void ColonistListSyncNeededHandler ();

	public class ColonistNotification
	{
		public TrackedColonist colonist;

		public ColonistNotificationType type;

		public Pawn relatedPawn;

		public ColonistNotification (ColonistNotificationType type, TrackedColonist colonist)
		{
			this.type = type;
			this.colonist = colonist;
			this.relatedPawn = null;
		}

		public ColonistNotification (ColonistNotificationType type, TrackedColonist colonist, Pawn relatedPawn)
		{
			this.type = type;
			this.colonist = colonist;
			this.relatedPawn = relatedPawn;
		}

		public override string ToString ()
		{
			NameTriple nameTriple = this.colonist.Pawn.Name as NameTriple;
			return string.Concat (new object[] {
				"ColonistNotification, ",
				this.type,
				": ",
				nameTriple.Nick
			});
		}
	}

	public delegate void ColonistNotificationHandler (ColonistNotification notification);

	public enum ColonistNotificationType
	{
		New,
		Died,
		Captured,
		Missing,
		Replaced,
		Freed,
		Buried,
		Lost,
		Deleted,
		Cryptosleep,
		WokeFromCryptosleep
	}

	public class ColonistTracker
	{
		protected static ColonistTracker instance;

		public static readonly bool LoggingEnabled = false;

		public static int MaxMissingDuration = 12000;

		protected bool initialized = false;

		protected ThingRequest corpseThingRequest;

		protected List<Pawn> removalList = new List<Pawn> ();

		protected HashSet<Pawn> pawnsInFaction = new HashSet<Pawn> ();

		protected HashSet<Pawn> colonistsInFaction = new HashSet<Pawn> ();

		protected Dictionary<Pawn, TrackedColonist> trackedColonists = new Dictionary<Pawn, TrackedColonist> ();

		public static ColonistTracker Instance {
			get {
				if (ColonistTracker.instance == null) {
					ColonistTracker.instance = new ColonistTracker ();
				}
				return ColonistTracker.instance;
			}
		}

		public List<Pawn> SortedPawns {
			get {
				List<Pawn> list = new List<Pawn> (this.trackedColonists.Keys);
				list.Sort (delegate (Pawn a, Pawn b) {
					int result;
					if (a.playerSettings != null && b.playerSettings != null && (a.playerSettings.joinTick != 0 || b.playerSettings.joinTick != 0)) {
						if (a.playerSettings.joinTick == 0 && b.playerSettings.joinTick != 0) {
							result = -1;
						}
						else if (a.playerSettings.joinTick != 0 && b.playerSettings.joinTick == 0) {
							result = 1;
						}
						else {
							result = a.playerSettings.joinTick.CompareTo (b.playerSettings.joinTick);
						}
					}
					else {
						result = b.GetUniqueLoadID ().CompareTo (a.GetUniqueLoadID ());
					}
					return result;
				});
				return list;
			}
		}

		protected ColonistTracker ()
		{
			this.corpseThingRequest = default(ThingRequest);
			this.corpseThingRequest.group = ThingRequestGroup.Corpse;
		}

		protected Faction FindCarryingFaction (Pawn pawn, out Pawn carrier)
		{
			ThingContainer holder = pawn.holder;
			Faction result;
			if (holder != null) {
				IThingContainerOwner owner = holder.owner;
				if (owner != null) {
					Pawn_CarryTracker pawn_CarryTracker = owner as Pawn_CarryTracker;
					if (pawn_CarryTracker != null) {
						Pawn pawn2 = pawn_CarryTracker.pawn;
						if (pawn2 != null) {
							carrier = pawn2;
							if (carrier.Faction != null && FactionUtility.HostileTo (carrier.Faction, Faction.OfColony)) {
								this.Message (pawn, "Carried by pawn (" + pawn2.NameStringShort + ") in hostile faction");
								result = pawn2.Faction;
								return result;
							}
							this.Message (pawn, "Carried by pawn (" + pawn2.NameStringShort + ") in non-hostile faction");
							result = Faction.OfColony;
							return result;
						}
					}
				}
			}
			carrier = null;
			result = null;
			return result;
		}

		protected Pawn FindColonist (Pawn pawn)
		{
			Pawn result;
			foreach (Pawn current in Find.MapPawns.PawnsInFaction (Faction.OfColony)) {
				if (current.GetUniqueLoadID () == pawn.GetUniqueLoadID ()) {
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		public TrackedColonist FindTrackedColonist (Pawn pawn)
		{
			TrackedColonist trackedColonist;
			TrackedColonist result;
			if (this.trackedColonists.TryGetValue (pawn, out trackedColonist)) {
				result = trackedColonist;
			}
			else {
				result = null;
			}
			return result;
		}

		public void InitializeWithDefaultColonists ()
		{
			this.Message ("InitializeWithDefaultColonists()");
			this.trackedColonists.Clear ();
			this.pawnsInFaction.Clear ();
			List<Pawn> list = new List<Pawn> ();
			foreach (Pawn current in Find.MapPawns.PawnsInFaction (Faction.OfColony)) {
				list.Add (current);
			}
			foreach (Thing current2 in Find.ListerThings.AllThings) {
				Corpse corpse = current2 as Corpse;
				if (corpse != null) {
					if (corpse.innerPawn != null) {
						if (corpse.innerPawn.Faction == Faction.OfColony && !this.IsBuried (corpse)) {
							list.Add (corpse.innerPawn);
						}
					}
				}
			}
			list.Sort (delegate (Pawn a, Pawn b) {
				int result;
				if (a.playerSettings != null && b.playerSettings != null && (a.playerSettings.joinTick != 0 || b.playerSettings.joinTick != 0)) {
					if (a.playerSettings.joinTick == 0 && b.playerSettings.joinTick != 0) {
						result = -1;
					}
					else if (a.playerSettings.joinTick != 0 && b.playerSettings.joinTick == 0) {
						result = 1;
					}
					else {
						result = a.playerSettings.joinTick.CompareTo (b.playerSettings.joinTick);
					}
				}
				else {
					result = b.GetUniqueLoadID ().CompareTo (a.GetUniqueLoadID ());
				}
				return result;
			});
			foreach (Pawn current in list) {
				this.StartTrackingPawn (current);
			}
		}

		protected bool IsBuried (Thing thing)
		{
			return thing.holder != null && thing.holder.owner != null && thing.holder.owner is Building_Grave;
		}

		protected void MarkColonistAsBuried (TrackedColonist colonist)
		{
			if (this.ColonistChanged != null) {
				this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Buried, colonist));
			}
			this.Message (colonist.Pawn, "Tracked colonist has been buried");
			this.removalList.Add (colonist.Pawn);
		}

		protected void MarkColonistAsCaptured (TrackedColonist colonist, Pawn carrier, Faction capturingFaction)
		{
			if (colonist.CapturingFaction != capturingFaction) {
				colonist.CapturingFaction = capturingFaction;
				if (this.ColonistChanged != null) {
					this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Captured, colonist));
				}
				this.Message (colonist.Pawn, "Colonist has been captured (by " + capturingFaction.name + ")");
			}
		}

		protected void MarkColonistAsDeleted (TrackedColonist colonist)
		{
			if (this.ColonistChanged != null) {
				this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Deleted, colonist));
			}
			this.Message (colonist.Pawn, "Tracked colonist has been deleted");
			this.removalList.Add (colonist.Pawn);
		}

		protected void MarkColonistAsEnteredCryptosleep (TrackedColonist colonist)
		{
			colonist.Cryptosleep = true;
			if (this.ColonistChanged != null) {
				this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Cryptosleep, colonist));
			}
			this.Message (colonist.Pawn, "Tracked colonist has entered cryptosleep.");
		}

		protected void MarkColonistAsFreed (TrackedColonist colonist)
		{
			colonist.CapturingFaction = null;
			if (!colonist.Pawn.Destroyed) {
				if (this.ColonistChanged != null) {
					this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Freed, colonist));
				}
				this.Message (colonist.Pawn, "Captured colonist has been freed.");
			}
		}

		protected void MarkColonistAsLost (TrackedColonist colonist)
		{
			if (this.ColonistChanged != null) {
				this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Lost, colonist));
			}
			this.Message ("Tracked colonist has been missing for more than " + ColonistTracker.MaxMissingDuration + " ticks");
			this.removalList.Add (colonist.Pawn);
		}

		protected void MarkColonistAsMissing (TrackedColonist colonist)
		{
			if (!colonist.Missing) {
				if (colonist.Captured) {
					this.Message (colonist.Pawn, "Captured colonist has been removed from the map (by " + colonist.CapturingFaction + ")");
				}
				colonist.Missing = true;
				colonist.MissingTimestamp = Find.TickManager.TicksGame;
				if (this.ColonistChanged != null) {
					this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Missing, colonist));
				}
				this.Message (colonist.Pawn, "Tracked colonist is missing (since " + colonist.MissingTimestamp + ")");
			}
		}

		protected void MarkColonistAsWokenFromCryptosleep (TrackedColonist colonist)
		{
			colonist.Cryptosleep = false;
			if (this.ColonistChanged != null) {
				this.ColonistChanged (new ColonistNotification (ColonistNotificationType.WokeFromCryptosleep, colonist));
			}
			this.Message (colonist.Pawn, "Tracked colonist has woken from cryptosleep.");
		}

		private void Message (Pawn pawn, string message)
		{
			NameTriple nameTriple = pawn.Name as NameTriple;
			string str = (nameTriple != null) ? nameTriple.Nick : pawn.Label;
			this.Message (str + ": " + message);
		}

		private void Message (string message)
		{
			if (ColonistTracker.LoggingEnabled) {
				Log.Message (message);
			}
		}

		protected void ReplaceTrackedPawn (TrackedColonist colonist, Pawn replacement)
		{
			this.trackedColonists.Remove (colonist.Pawn);
			colonist.Pawn = replacement;
			this.trackedColonists.Add (colonist.Pawn, colonist);
			this.Message (colonist.Pawn, "Tracked colonist was found.  Pawn was replaced.");
			if (this.ColonistChanged != null) {
				this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Replaced, colonist, replacement));
			}
		}

		public void Reset ()
		{
			this.initialized = false;
			this.colonistsInFaction.Clear ();
			this.pawnsInFaction.Clear ();
			this.trackedColonists.Clear ();
			this.removalList.Clear ();
		}

		public void ResolveMissingPawn (Pawn pawn, TrackedColonist colonist)
		{
			if (pawn.Dead || pawn.Destroyed) {
				this.Message (pawn, "Tracked colonist is dead or destroyed.  Searching for corpse.");
				Corpse corpse = (Corpse)Find.ListerThings.ThingsMatching (this.corpseThingRequest).FirstOrDefault (delegate (Thing thing) {
					Corpse corpse2 = thing as Corpse;
					return corpse2 != null && corpse2.innerPawn == pawn;
				});
				if (corpse != null) {
					if (!colonist.Dead) {
						colonist.Dead = true;
						if (this.ColonistChanged != null) {
							this.ColonistChanged (new ColonistNotification (ColonistNotificationType.Died, colonist));
						}
					}
					colonist.Corpse = corpse;
					this.Message (pawn, "Corpse found.  Colonist is dead.");
					return;
				}
				this.Message ("Corpse not found.");
			}
			Pawn pawn2 = null;
			Faction faction = this.FindCarryingFaction (pawn, out pawn2);
			if (faction != null) {
				if (faction != Faction.OfColony) {
					colonist.CapturingFaction = faction;
					this.Message (pawn, "Colonist is captured");
				}
				else {
					this.Message (pawn, "Colonist is being rescued");
				}
			}
			else {
				Pawn pawn3 = this.FindColonist (pawn);
				if (pawn3 == null) {
					if (!colonist.Missing) {
						this.MarkColonistAsMissing (colonist);
					}
				}
				else {
					this.ReplaceTrackedPawn (colonist, pawn3);
				}
			}
		}

		protected TrackedColonist StartTrackingPawn (Pawn pawn)
		{
			TrackedColonist result;
			if (pawn == null || !pawn.IsColonist) {
				result = null;
			}
			else {
				TrackedColonist trackedColonist = null;
				if (this.trackedColonists.TryGetValue (pawn, out trackedColonist)) {
					this.Message (pawn, "Already tracking colonist");
					result = trackedColonist;
				}
				else {
					trackedColonist = new TrackedColonist (pawn);
					if (!this.trackedColonists.ContainsKey (pawn)) {
						this.trackedColonists.Add (pawn, trackedColonist);
						if (this.ColonistChanged != null) {
							this.ColonistChanged (new ColonistNotification (ColonistNotificationType.New, trackedColonist));
						}
						this.Message (pawn, "Tracking new colonist");
					}
					else {
						this.Message (pawn, "Already tracking colonist");
					}
					result = trackedColonist;
				}
			}
			return result;
		}

		public void StartTrackingPawns (IEnumerable<Pawn> pawns)
		{
			this.Message ("StartTrackingPawns(" + pawns.Count<Pawn> () + ")");
			foreach (Pawn current in pawns) {
				this.StartTrackingPawn (current);
			}
			this.SyncColonistLists ();
		}

		public void StopTrackingPawn (Pawn pawn)
		{
			TrackedColonist trackedColonist = this.FindTrackedColonist (pawn);
			if (trackedColonist != null) {
				this.MarkColonistAsDeleted (trackedColonist);
			}
		}

		private void SyncColonistLists ()
		{
			this.pawnsInFaction.Clear ();
			this.colonistsInFaction.Clear ();
			foreach (Pawn pawn in Find.MapPawns.PawnsInFaction (Faction.OfColony)) {
				// Pawn pawn;
				this.pawnsInFaction.Add (pawn);
				if (pawn.IsColonist) {
					this.colonistsInFaction.Add (pawn);
				}
				if (!this.trackedColonists.ContainsKey (pawn)) {
					this.StartTrackingPawn (pawn);
				}
			}
			if (this.colonistsInFaction.Count != this.trackedColonists.Count) {
				this.Message ("Free colonist list count does not match tracked count.  Resolving.");
				foreach (TrackedColonist current in this.trackedColonists.Values) {
					Pawn pawn = current.Pawn;
					if (!this.pawnsInFaction.Contains (pawn)) {
						this.Message (pawn, "Tracked colonist not found in free list.  Resolving.");
						this.ResolveMissingPawn (pawn, current);
					}
				}
			}
			if (this.ColonistListSyncNeeded != null) {
				this.ColonistListSyncNeeded ();
			}
		}

		public void Update ()
		{
			if (!this.initialized) {
				this.InitializeWithDefaultColonists ();
				this.initialized = true;
			}
			if (Find.MapPawns.PawnsInFaction (Faction.OfColony).Count<Pawn> () != this.pawnsInFaction.Count) {
				this.Message ("Free colonist list changed.  Re-syncing");
				this.SyncColonistLists ();
			}
			foreach (KeyValuePair<Pawn, TrackedColonist> current in this.trackedColonists) {
				this.UpdateColonistState (current.Key, current.Value);
			}
			foreach (Pawn current2 in this.removalList) {
				this.trackedColonists.Remove (current2);
				this.Message (current2, "No longer tracking pawn");
			}
			this.removalList.Clear ();
		}

		protected void UpdateColonistState (Pawn pawn, TrackedColonist colonist)
		{
			Faction faction = null;
			bool flag = false;
			Pawn pawn2 = null;
			if (pawn.holder != null) {
				if (pawn.Destroyed) {
					this.MarkColonistAsMissing (colonist);
				}
				else if (pawn.holder.owner != null) {
					Pawn_CarryTracker pawn_CarryTracker = pawn.holder.owner as Pawn_CarryTracker;
					if (pawn_CarryTracker != null) {
						if (pawn_CarryTracker.pawn != null && pawn_CarryTracker.pawn.Faction != null && pawn_CarryTracker.pawn.Faction != Faction.OfColony && pawn_CarryTracker.pawn.Faction.RelationWith (Faction.OfColony, false).hostile) {
							pawn2 = pawn_CarryTracker.pawn;
							faction = pawn2.Faction;
						}
					}
					Building_CryptosleepCasket building_CryptosleepCasket = pawn.holder.owner as Building_CryptosleepCasket;
					if (building_CryptosleepCasket != null) {
						flag = true;
						if (!colonist.Cryptosleep) {
							colonist.Cryptosleep = true;
							this.Message (pawn, "Colonist has entered cryptosleep.");
						}
					}
					else {
						colonist.Cryptosleep = false;
						if (colonist.Cryptosleep) {
							colonist.Cryptosleep = false;
							this.Message (pawn, "Colonist has woken from cryptosleep.");
						}
					}
				}
			}
			else {
				faction = null;
				colonist.Cryptosleep = false;
				if (colonist.Captured) {
					this.Message (pawn, "Captured colonist has been freed.");
					this.MarkColonistAsFreed (colonist);
				}
				if (colonist.Cryptosleep) {
					colonist.Cryptosleep = false;
					this.Message (pawn, "Colonist has woken from cryptosleep.");
				}
			}
			if (!colonist.Captured && faction != null) {
				this.MarkColonistAsCaptured (colonist, pawn2, faction);
			}
			else if (colonist.Captured && faction == null) {
				this.MarkColonistAsFreed (colonist);
			}
			else if (colonist.Captured && faction != colonist.CapturingFaction) {
				this.MarkColonistAsCaptured (colonist, pawn2, faction);
			}
			if (flag && !colonist.Cryptosleep) {
				this.MarkColonistAsEnteredCryptosleep (colonist);
			}
			else if (!flag && colonist.Cryptosleep) {
				this.MarkColonistAsWokenFromCryptosleep (colonist);
			}
			int ticksGame = Find.TickManager.TicksGame;
			if (colonist.Dead && !colonist.Missing) {
				if (colonist.Corpse != null) {
					if (colonist.Corpse.Destroyed) {
						this.MarkColonistAsMissing (colonist);
					}
					else if (this.IsBuried (colonist.Corpse)) {
						this.MarkColonistAsBuried (colonist);
					}
				}
			}
			else if (colonist.Missing) {
				int num = ticksGame - colonist.MissingTimestamp;
				if (num > ColonistTracker.MaxMissingDuration) {
					this.MarkColonistAsLost (colonist);
				}
			}
		}

		public event ColonistNotificationHandler ColonistChanged;

		public event ColonistListSyncNeededHandler ColonistListSyncNeeded;
	}

	public class ComponentColonistBar : MapComponent
	{
		private bool loadedTextures = false;

		private bool initialized = false;

		private int height;

		private int width;

		private List<ColonistBarGroup> defaultGroups = new List<ColonistBarGroup> ();

		private ColonistBarGroup defaultGroup = new ColonistBarGroup ();

		private ColonistBar colonistBar;

		public ColonistBar ColonistBar {
			get {
				return this.colonistBar;
			}
		}

		public ColonistBarGroup DefaultGroup {
			get {
				return this.defaultGroup;
			}
		}

		public List<ColonistBarGroup> DefaultGroups {
			get {
				return this.defaultGroups;
			}
		}

		public string Name {
			get {
				return "ColonistBar";
			}
		}

		public bool RenderWithScreenshots {
			get {
				return true;
			}
		}

		public void ColonistNotificationHandler (ColonistNotification notification)
		{
			if (notification.type == ColonistNotificationType.New) {
				this.defaultGroup.Add (notification.colonist);
			}
			else if (notification.type == ColonistNotificationType.Buried || notification.type == ColonistNotificationType.Lost || notification.type == ColonistNotificationType.Deleted) {
				this.defaultGroup.Remove (notification.colonist);
			}
		}

		public override void ExposeData ()
		{
		}

		public void Initialize ()
		{
			ColonistTracker.Instance.Reset ();
			this.defaultGroups.Add (this.defaultGroup);
			this.colonistBar = new ColonistBar ();
			this.colonistBar.AddGroup (this.defaultGroup);
			this.colonistBar.CurrentGroup = this.defaultGroup;
			this.width = Screen.width;
			this.height = Screen.height;
			ColonistTracker.Instance.ColonistChanged += new ColonistNotificationHandler (this.ColonistNotificationHandler);
			LongEventHandler.ExecuteWhenFinished (delegate {
				this.colonistBar.Drawer.ResetTextures ();
				this.colonistBar.Drawer.enabled = true;
				this.loadedTextures = true;
			});
		}

		public override void MapComponentOnGUI ()
		{
			if (this.initialized && this.loadedTextures) {
				this.colonistBar.Draw ();
			}
		}

		public override void MapComponentUpdate ()
		{
			if (!this.initialized) {
				this.initialized = true;
				this.Initialize ();
			}
			if (this.initialized && this.loadedTextures) {
				ColonistTracker.Instance.Update ();
				if (this.width != Screen.width || this.height != Screen.height) {
					this.width = Screen.width;
					this.height = Screen.height;
					this.colonistBar.UpdateScreenSize (this.width, this.height);
				}
			}
		}
	}

	public class ComponentColonistTracker
	{
		public string Name {
			get {
				return "ColonistTracker";
			}
		}

		public ComponentColonistTracker ()
		{
			ColonistTracker.Instance.Reset ();
		}

		public void Initialize ()
		{
			ColonistTracker.Instance.InitializeWithDefaultColonists ();
		}

		public void Update ()
		{
			ColonistTracker.Instance.Update ();
		}
	}

	public abstract class IntegerOptionsPreference : IPreference
	{
		public delegate void ValueChangedHandler (int value);

		public static float LabelMargin = IntegerOptionsPreference.RadioButtonWidth + IntegerOptionsPreference.RadioButtonMargin;

		public static float RadioButtonMargin = 18;

		public static float RadioButtonWidth = 24;

		private string stringValue;

		private int? intValue = null;

		public int tooltipId = 0;

		public abstract int DefaultValue {
			get;
		}

		public virtual bool Disabled {
			get {
				return false;
			}
		}

		public virtual bool DisplayInOptions {
			get {
				return true;
			}
		}

		public abstract string Group {
			get;
		}

		public virtual bool Indent {
			get {
				return false;
			}
		}

		public virtual string Label {
			get {
				return Translator.Translate (this.Name);
			}
		}

		public abstract string Name {
			get;
		}

		public abstract string OptionValuePrefix {
			get;
		}

		public abstract IEnumerable<int> OptionValues {
			get;
		}

		public virtual string Tooltip {
			get {
				return null;
			}
		}

		protected virtual int TooltipId {
			get {
				int result;
				if (this.tooltipId == 0) {
					this.tooltipId = Translator.Translate (this.Tooltip).GetHashCode ();
					result = this.tooltipId;
				}
				else {
					result = 0;
				}
				return result;
			}
		}

		public virtual int Value {
			get {
				int result;
				if (this.intValue.HasValue) {
					result = this.intValue.Value;
				}
				else {
					result = this.DefaultValue;
				}
				return result;
			}
			set {
				int? num = this.intValue;
				this.intValue = new int? (value);
				this.stringValue = value.ToString ();
				if ((!num.HasValue || num.Value != this.intValue) && this.ValueChanged != null) {
					this.ValueChanged (value);
				}
			}
		}

		public virtual int ValueForDisplay {
			get {
				int result;
				if (this.intValue.HasValue) {
					result = this.intValue.Value;
				}
				else {
					result = this.DefaultValue;
				}
				return result;
			}
		}

		public string ValueForSerialization {
			get {
				return this.stringValue;
			}
			set {
				int value2;
				if (int.TryParse (value, out value2)) {
					if (this.OptionValues.Contains (value2)) {
						this.stringValue = value;
						this.intValue = new int? (value2);
						return;
					}
				}
				this.intValue = null;
				this.stringValue = null;
			}
		}

		public IntegerOptionsPreference ()
		{
		}

		public void OnGUI (float positionX, ref float positionY, float width)
		{
			bool disabled = this.Disabled;
			if (disabled) {
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			float num = this.Indent ? WidgetDrawer.IndentSize : 0;
			foreach (int current in this.OptionValues) {
				string text = Translator.Translate (this.OptionValuePrefix + "." + current);
				float num2 = Text.CalcHeight (text, width - IntegerOptionsPreference.LabelMargin - num);
				Rect rect = new Rect (positionX - 4 + num, positionY - 3, width + 6 - num, num2 + 5);
				if (Mouse.IsOver (rect)) {
					Widgets.DrawHighlight (rect);
				}
				Rect rect2 = new Rect (positionX + num, positionY, width - IntegerOptionsPreference.LabelMargin - num, num2);
				GUI.Label (rect2, text);
				if (this.Tooltip != null) {
					TipSignal tipSignal = new TipSignal (() => Translator.Translate (this.Tooltip), this.TooltipId);
					TooltipHandler.TipRegion (rect2, tipSignal);
				}
				int valueForDisplay = this.ValueForDisplay;
				bool flag = valueForDisplay == current;
				if (Widgets.RadioButton (new Vector2 (positionX + width - IntegerOptionsPreference.RadioButtonWidth, positionY - 3), flag) && !disabled) {
					this.Value = current;
				}
				positionY += num2 + WidgetDrawer.PreferencePadding.y;
			}
			positionY -= WidgetDrawer.PreferencePadding.y;
			GUI.color = Color.white;
		}

		public event IntegerOptionsPreference.ValueChangedHandler ValueChanged;
	}

	public interface IPreference
	{
		bool DisplayInOptions {
			get;
		}

		string Group {
			get;
		}

		string Name {
			get;
		}

		string ValueForSerialization {
			get;
			set;
		}

		void OnGUI (float positionX, ref float positionY, float width);
	}

	public class LazyLoadTexture
	{
		private string resource;

		private Texture2D texture;

		private bool loaded = false;

		public Texture2D Texture {
			get {
				if (!this.loaded) {
					this.Load ();
				}
				return this.texture;
			}
		}

		public LazyLoadTexture (string resource)
		{
			this.resource = resource;
		}

		public void Draw (Rect rect)
		{
			Texture2D texture2D = this.Texture;
			if (texture2D != null) {
				GUI.DrawTexture (rect, texture2D);
			}
		}

		public Texture2D Load ()
		{
			this.loaded = true;
			this.texture = ContentFinder<Texture2D>.Get (this.resource, true);
			if (this.texture == null) {
				this.texture = BaseContent.BadTex;
			}
			return this.texture;
		}
	}

	public abstract class MultipleSelectionStringOptionsPreference : IPreference
	{
		public delegate void ValueChangedHandler (IEnumerable<string> selectedOptions);

		public static float LabelMargin = MultipleSelectionStringOptionsPreference.RadioButtonWidth + MultipleSelectionStringOptionsPreference.RadioButtonMargin;

		public static float RadioButtonMargin = 18;

		public static float RadioButtonWidth = 24;

		public HashSet<string> selectedOptions = new HashSet<string> ();

		public int tooltipId = 0;

		private string setValue = null;

		private string stringValue;

		public abstract string DefaultValue {
			get;
		}

		public virtual bool Disabled {
			get {
				return false;
			}
		}

		public virtual bool DisplayInOptions {
			get {
				return true;
			}
		}

		public abstract string Group {
			get;
		}

		public virtual bool Indent {
			get {
				return true;
			}
		}

		public virtual string Label {
			get {
				return Translator.Translate (this.Name);
			}
		}

		public abstract string Name {
			get;
		}

		public abstract string OptionValuePrefix {
			get;
		}

		public abstract IEnumerable<string> OptionValues {
			get;
		}

		public IEnumerable<string> SelectedOptions {
			get {
				return this.selectedOptions;
			}
		}

		public virtual string Tooltip {
			get {
				return null;
			}
		}

		protected virtual int TooltipId {
			get {
				int result;
				if (this.tooltipId == 0) {
					this.tooltipId = Translator.Translate (this.Tooltip).GetHashCode ();
					result = this.tooltipId;
				}
				else {
					result = 0;
				}
				return result;
			}
		}

		public virtual string ValueForDisplay {
			get {
				string defaultValue;
				if (this.setValue != null) {
					defaultValue = this.setValue;
				}
				else {
					defaultValue = this.DefaultValue;
				}
				return defaultValue;
			}
		}

		public string ValueForSerialization {
			get {
				return this.stringValue;
			}
			set {
				this.stringValue = value;
				this.setValue = value;
				this.selectedOptions.Clear ();
				string[] array = this.stringValue.Split (new char[] {
					','
				});
				for (int i = 0; i < array.Length; i++) {
					string text = array [i];
					if (this.OptionValues.Contains (text)) {
						this.selectedOptions.Add (text);
					}
				}
			}
		}

		public MultipleSelectionStringOptionsPreference ()
		{
		}

		public bool IsOptionSelected (string option)
		{
			return this.selectedOptions.Contains (option);
		}

		public void OnGUI (float positionX, ref float positionY, float width)
		{
			bool disabled = this.Disabled;
			if (disabled) {
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			if (!string.IsNullOrEmpty (this.Name)) {
				string text = Translator.Translate (this.Name);
				float num = Text.CalcHeight (text, width);
				Rect rect = new Rect (positionX, positionY, width, num);
				Widgets.Label (rect, text);
				if (this.Tooltip != null) {
					TipSignal tipSignal = new TipSignal (() => Translator.Translate (this.Tooltip), this.TooltipId);
					TooltipHandler.TipRegion (rect, tipSignal);
				}
				positionY += num + WidgetDrawer.PreferencePadding.y;
			}
			float num2 = this.Indent ? WidgetDrawer.IndentSize : 0;
			foreach (string current in this.OptionValues) {
				string text = this.OptionTranslated (current);
				if (!string.IsNullOrEmpty (text)) {
					float num = Text.CalcHeight (text, width - MultipleSelectionStringOptionsPreference.LabelMargin - num2);
					Rect rect2 = new Rect (positionX - 4 + num2, positionY - 3, width + 6 - num2, num + 5);
					if (Mouse.IsOver (rect2)) {
						Widgets.DrawHighlight (rect2);
					}
					Rect rect = new Rect (positionX + num2, positionY, width - num2, num);
					bool flag = this.IsOptionSelected (current);
					bool flag2 = flag;
					WidgetDrawer.DrawLabeledCheckbox (rect, text, ref flag);
					if (flag != flag2) {
						this.UpdateOption (current, flag);
					}
					positionY += num + WidgetDrawer.PreferencePadding.y;
				}
			}
			positionY -= WidgetDrawer.PreferencePadding.y;
			GUI.color = Color.white;
		}

		public virtual string OptionTranslated (string optionValue)
		{
			return Translator.Translate (this.OptionValuePrefix + "." + optionValue);
		}

		public void UpdateOption (string option, bool value)
		{
			if (!value) {
				if (this.selectedOptions.Contains (option)) {
					this.selectedOptions.Remove (option);
					this.UpdateSerializedValue ();
					if (this.ValueChanged != null) {
						this.ValueChanged (this.selectedOptions);
					}
				}
			}
			else if (!this.selectedOptions.Contains (option)) {
				this.selectedOptions.Add (option);
				this.UpdateSerializedValue ();
				if (this.ValueChanged != null) {
					this.ValueChanged (this.selectedOptions);
				}
			}
		}

		protected void UpdateSerializedValue ()
		{
			this.stringValue = string.Join (",", this.selectedOptions.ToArray<string> ());
		}

		public event MultipleSelectionStringOptionsPreference.ValueChangedHandler ValueChanged;
	}

	public class PreferenceEnabled : BooleanPreference
	{
		public override bool DefaultValue {
			get {
				return true;
			}
		}

		public override string Group {
			get {
				return "EdB.ColonistBar.Prefs";
			}
		}

		public override string Name {
			get {
				return "EdB.ColonistBar.Prefs.Enable";
			}
		}
	}

	public class PreferenceGroup
	{
		private string name;

		private List<IPreference> preferences = new List<IPreference> ();

		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}

		public int PreferenceCount {
			get {
				return this.preferences.Count;
			}
		}

		public IEnumerable<IPreference> Preferences {
			get {
				return this.preferences;
			}
		}

		public PreferenceGroup (string name)
		{
			this.name = name;
		}

		public void Add (IPreference preference)
		{
			this.preferences.Add (preference);
		}
	}

	public class Preferences
	{
		private static Preferences instance;

		protected bool atLeastOne = false;

		protected List<PreferenceGroup> miscellaneousGroup = new List<PreferenceGroup> ();

		protected Dictionary<string, IPreference> preferenceDictionary = new Dictionary<string, IPreference> ();

		protected Dictionary<string, PreferenceGroup> groupDictionary = new Dictionary<string, PreferenceGroup> ();

		protected List<PreferenceGroup> groups = new List<PreferenceGroup> ();

		public static Preferences Instance {
			get {
				if (Preferences.instance == null) {
					Preferences.instance = new Preferences ();
				}
				return Preferences.instance;
			}
		}

		public bool AtLeastOne {
			get {
				return this.atLeastOne;
			}
		}

		protected string FilePath {
			get {
				return Path.Combine (GenFilePaths.ConfigFolderPath, "EdBInterface.xml");
			}
		}

		public IEnumerable<PreferenceGroup> Groups {
			get {
				IEnumerable<PreferenceGroup> result;
				if (this.miscellaneousGroup [0].PreferenceCount > 0) {
					result = this.groups.Concat (this.miscellaneousGroup);
				}
				else {
					result = this.groups;
				}
				return result;
			}
		}

		public Preferences ()
		{
			this.Reset ();
		}

		public void Add (IPreference preference)
		{
			if (this.preferenceDictionary.ContainsKey (preference.Name)) {
				Log.Warning ("Preference already added to EdB.Interface.Preferences: " + preference.Name);
			}
			else {
				string group = preference.Group;
				PreferenceGroup preferenceGroup;
				if (group == null) {
					preferenceGroup = this.miscellaneousGroup [0];
				}
				else if (this.groupDictionary.ContainsKey (group)) {
					preferenceGroup = this.groupDictionary [group];
				}
				else {
					preferenceGroup = new PreferenceGroup (group);
					this.groups.Add (preferenceGroup);
					this.groupDictionary.Add (group, preferenceGroup);
				}
				preferenceGroup.Add (preference);
				this.preferenceDictionary.Add (preference.Name, preference);
				this.atLeastOne = true;
			}
		}

		public void Load ()
		{
			try {
				XmlDocument xmlDocument = new XmlDocument ();
				try {
					xmlDocument.LoadXml (File.ReadAllText (this.FilePath));
				}
				catch (FileNotFoundException) {
					return;
				}
				foreach (object current in xmlDocument.ChildNodes) {
					if (current is XmlElement) {
						XmlElement xmlElement = current as XmlElement;
						if ("Preferences".Equals (xmlElement.Name)) {
							foreach (object current2 in xmlElement.ChildNodes) {
								if (current2 is XmlElement) {
									XmlElement xmlElement2 = current2 as XmlElement;
									string name = xmlElement2.Name;
									if (this.preferenceDictionary.ContainsKey (name)) {
										IPreference preference = this.preferenceDictionary [name];
										preference.ValueForSerialization = xmlElement2.InnerText;
									}
									else {
										Log.Warning ("Unrecognized EdB Interface preference: " + name);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception arg) {
				Log.Warning ("Exception loading EdB Interface preferences: " + arg);
			}
		}

		public void Reset ()
		{
			this.groups.Clear ();
			this.groupDictionary.Clear ();
			this.preferenceDictionary.Clear ();
			this.miscellaneousGroup.Clear ();
			this.miscellaneousGroup.Add (new PreferenceGroup ("EdB.InterfaceOptions.Prefs.Miscellaneous"));
			this.atLeastOne = false;
		}

		public void Save ()
		{
			try {
				XDocument xDocument = new XDocument ();
				XElement xElement = new XElement ("Preferences");
				xDocument.Add (xElement);
				foreach (PreferenceGroup current in this.Groups) {
					foreach (IPreference current2 in current.Preferences) {
						if (!string.IsNullOrEmpty (current2.ValueForSerialization)) {
							XElement content = new XElement (current2.Name, current2.ValueForSerialization);
							xElement.Add (content);
						}
					}
				}
				xDocument.Save (this.FilePath);
			}
			catch (Exception arg) {
				Log.Warning ("Exception saving EdB Interface preferences: " + arg);
			}
		}
	}

	public class PreferenceSmallIcons : BooleanPreference
	{
		public override bool DefaultValue {
			get {
				return false;
			}
		}

		public override string Group {
			get {
				return "EdB.ColonistBar.Prefs";
			}
		}

		public override bool Indent {
			get {
				return false;
			}
		}

		public override string Name {
			get {
				return "EdB.ColonistBar.Prefs.SmallIcons";
			}
		}

		public override string Tooltip {
			get {
				return "EdB.ColonistBar.Prefs.SmallIcons.Tip";
			}
		}
	}

	public class ScreenSizeMonitor
	{
		public delegate void ScreenSizeChangeHandler (int width, int height);

		protected int width;

		protected int height;

		public int Height {
			get {
				return this.height;
			}
		}

		public int Width {
			get {
				return this.width;
			}
		}

		public ScreenSizeMonitor ()
		{
			this.width = Screen.width;
			this.height = Screen.height;
		}

		public void Update ()
		{
			int num = Screen.width;
			int num2 = Screen.height;
			if (num != this.width || num2 != this.height) {
				this.width = num;
				this.height = num2;
				if (this.Changed != null) {
					this.Changed (num, num2);
				}
			}
		}

		public event ScreenSizeMonitor.ScreenSizeChangeHandler Changed;
	}

	public class SelectorUtility
	{
		protected FieldInfo hostilePawnsField = null;

		protected FieldInfo allPawnsField = null;

		protected List<Pawn> emptyList = new List<Pawn> ();

		protected List<Pawn> visitorPawns = new List<Pawn> (20);

		public IEnumerable<Pawn> ColonyAnimals {
			get {
				return from pawn in Find.MapPawns.PawnsInFaction (Faction.OfColony)
				where !pawn.IsColonist
				select pawn;
			}
		}

		public int HostilePawnCount {
			get {
				Dictionary<Faction, List<Pawn>> dictionary = (Dictionary<Faction, List<Pawn>>)this.hostilePawnsField.GetValue (Find.MapPawns);
				int result;
				if (dictionary == null) {
					result = 0;
				}
				else {
					result = dictionary [Faction.OfColony].Count;
				}
				return result;
			}
		}

		public IEnumerable<Pawn> HostilePawns {
			get {
				Dictionary<Faction, List<Pawn>> dictionary = (Dictionary<Faction, List<Pawn>>)this.hostilePawnsField.GetValue (Find.MapPawns);
				IEnumerable<Pawn> result;
				if (dictionary == null) {
					result = this.emptyList;
				}
				else {
					result = from p in dictionary [Faction.OfColony]
					where !p.InContainer
					select p;
				}
				return result;
			}
		}

		public bool MoreThanOneColonyAnimal {
			get {
				int num = 0;
				bool result;
				foreach (Pawn current in from pawn in Find.MapPawns.PawnsInFaction (Faction.OfColony)
				where !pawn.IsColonist
				select pawn) {
					if (++num > 1) {
						result = true;
						return result;
					}
				}
				result = false;
				return result;
			}
		}

		public bool MoreThanOneHostilePawn {
			get {
				Dictionary<Faction, List<Pawn>> dictionary = (Dictionary<Faction, List<Pawn>>)this.hostilePawnsField.GetValue (Find.MapPawns);
				bool result;
				if (dictionary == null) {
					result = false;
				}
				else {
					int num = 0;
					foreach (Pawn current in from p in dictionary [Faction.OfColony]
					where !p.InContainer
					select p) {
						if (++num > 1) {
							result = true;
							return result;
						}
					}
					result = false;
				}
				return result;
			}
		}

		public bool MoreThanOneVisitorPawn {
			get {
				List<Pawn> list = (List<Pawn>)this.allPawnsField.GetValue (Find.MapPawns);
				bool result;
				if (list == null || list.Count < 2) {
					result = false;
				}
				else {
					int num = 0;
					foreach (Pawn current in from p in list
					where p.Faction != null && p.Faction != Faction.OfColony && !p.IsPrisonerOfColony && !p.Faction.RelationWith (Faction.OfColony, false).hostile && !p.InContainer
					select p) {
						if (++num > 1) {
							result = true;
							return result;
						}
					}
					result = false;
				}
				return result;
			}
		}

		public IEnumerable<Pawn> VisitorPawns {
			get {
				List<Pawn> list = (List<Pawn>)this.allPawnsField.GetValue (Find.MapPawns);
				IEnumerable<Pawn> result;
				if (list == null) {
					result = this.emptyList;
				}
				else {
					result = from p in list
					where p.Faction != null && p.Faction != Faction.OfColony && !p.IsPrisonerOfColony && !p.Faction.RelationWith (Faction.OfColony, false).hostile && !p.InContainer
					select p;
				}
				return result;
			}
		}

		public SelectorUtility ()
		{
			this.hostilePawnsField = typeof(MapPawns).GetField ("pawnsHostileToFaction", BindingFlags.Instance | BindingFlags.NonPublic);
			this.allPawnsField = typeof(MapPawns).GetField ("allPawns", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public void AddToSelection (object o)
		{
			Find.Selector.Select (o, false, false);
		}

		public void ClearSelection ()
		{
			Find.Selector.ClearSelection ();
		}

		public void SelectAllColonists ()
		{
			Selector selector = Find.Selector;
			selector.ClearSelection ();
			foreach (Pawn current in Find.MapPawns.FreeColonists) {
				Find.Selector.Select (current, false, false);
			}
			Find.MainTabsRoot.SetCurrentTab (MainTabDefOf.Inspect, true);
		}

		public void SelectNextColonist ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony) {
				this.SelectThing (Find.MapPawns.FreeColonists.FirstOrDefault<Pawn> (), false);
			}
			else {
				bool flag = false;
				foreach (Pawn current in Find.MapPawns.FreeColonists) {
					if (flag) {
						this.SelectThing (current, false);
						return;
					}
					if (current == selector.SingleSelectedThing) {
						flag = true;
					}
				}
				this.SelectThing (Find.MapPawns.FreeColonists.FirstOrDefault<Pawn> (), false);
			}
		}

		public void SelectNextColonyAnimal ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony || (selector.SingleSelectedThing as Pawn).IsColonist) {
				this.SelectThing (this.ColonyAnimals.FirstOrDefault<Pawn> (), false);
			}
			else {
				bool flag = false;
				foreach (Pawn current in this.ColonyAnimals) {
					if (flag) {
						this.SelectThing (current, false);
						return;
					}
					if (current == selector.SingleSelectedThing) {
						flag = true;
					}
				}
				this.SelectThing (this.ColonyAnimals.FirstOrDefault<Pawn> (), false);
			}
		}

		public void SelectNextEnemy ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony) {
				Pawn thing = this.HostilePawns.FirstOrDefault<Pawn> ();
				this.SelectThing (thing, false);
			}
			else {
				bool flag = false;
				foreach (Pawn current in this.HostilePawns) {
					if (flag) {
						this.SelectThing (current, false);
						return;
					}
					if (current == selector.SingleSelectedThing) {
						flag = true;
					}
				}
				Pawn thing = this.HostilePawns.FirstOrDefault<Pawn> ();
				this.SelectThing (thing, false);
			}
		}

		public void SelectNextPrisoner ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn)) {
				this.SelectThing (Find.MapPawns.PrisonersOfColony.FirstOrDefault<Pawn> (), false);
			}
			else {
				bool flag = false;
				foreach (Pawn current in Find.MapPawns.PrisonersOfColony) {
					if (flag) {
						this.SelectThing (current, false);
						return;
					}
					if (current == selector.SingleSelectedThing) {
						flag = true;
					}
				}
				this.SelectThing (Find.MapPawns.PrisonersOfColony.FirstOrDefault<Pawn> (), false);
			}
		}

		public void SelectNextVisitor ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony) {
				this.SelectThing (this.VisitorPawns.FirstOrDefault<Pawn> (), false);
			}
			else {
				bool flag = false;
				foreach (Pawn current in this.VisitorPawns) {
					if (flag) {
						this.SelectThing (current, false);
						return;
					}
					if (current == selector.SingleSelectedThing) {
						flag = true;
					}
				}
				this.SelectThing (this.VisitorPawns.FirstOrDefault<Pawn> (), false);
			}
		}

		public void SelectPreviousColonist ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony) {
				this.SelectThing (Find.MapPawns.FreeColonists.LastOrDefault<Pawn> (), false);
			}
			else {
				Pawn pawn = null;
				foreach (Pawn current in Find.MapPawns.FreeColonists) {
					if (selector.SingleSelectedThing == current) {
						if (pawn != null) {
							this.SelectThing (pawn, false);
							break;
						}
						this.SelectThing (Find.MapPawns.FreeColonists.LastOrDefault<Pawn> (), false);
						break;
					}
					else {
						pawn = current;
					}
				}
			}
		}

		public void SelectPreviousColonyAnimal ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony || (selector.SingleSelectedThing as Pawn).IsColonist) {
				this.SelectThing (this.ColonyAnimals.LastOrDefault<Pawn> (), false);
			}
			else {
				Pawn pawn = null;
				foreach (Pawn current in this.ColonyAnimals) {
					if (selector.SingleSelectedThing == current) {
						if (pawn != null) {
							this.SelectThing (pawn, false);
							break;
						}
						this.SelectThing (this.ColonyAnimals.LastOrDefault<Pawn> (), false);
						break;
					}
					else {
						pawn = current;
					}
				}
			}
		}

		public void SelectPreviousEnemy ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony) {
				this.SelectThing (this.HostilePawns.LastOrDefault<Pawn> (), false);
			}
			else {
				Pawn pawn = null;
				foreach (Pawn current in this.HostilePawns) {
					if (selector.SingleSelectedThing == current) {
						if (pawn != null) {
							this.SelectThing (pawn, false);
							break;
						}
						this.SelectThing (this.HostilePawns.LastOrDefault<Pawn> (), false);
						break;
					}
					else {
						pawn = current;
					}
				}
			}
		}

		public void SelectPreviousPrisoner ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn)) {
				this.SelectThing (Find.MapPawns.PrisonersOfColony.LastOrDefault<Pawn> (), false);
			}
			else {
				Pawn pawn = null;
				foreach (Pawn current in Find.MapPawns.PrisonersOfColony) {
					if (selector.SingleSelectedThing == current) {
						if (pawn != null) {
							this.SelectThing (pawn, false);
							break;
						}
						this.SelectThing (Find.MapPawns.PrisonersOfColony.LastOrDefault<Pawn> (), false);
						break;
					}
					else {
						pawn = current;
					}
				}
			}
		}

		public void SelectPreviousVisitor ()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony) {
				this.SelectThing (this.VisitorPawns.LastOrDefault<Pawn> (), false);
			}
			else {
				Pawn pawn = null;
				foreach (Pawn current in this.VisitorPawns) {
					if (selector.SingleSelectedThing == current) {
						if (pawn != null) {
							this.SelectThing (pawn, false);
							break;
						}
						this.SelectThing (this.VisitorPawns.LastOrDefault<Pawn> (), false);
						break;
					}
					else {
						pawn = current;
					}
				}
			}
		}

		public void SelectThing (Thing thing, bool addToSelection)
		{
			if (thing != null) {
				if (!addToSelection) {
					Find.Selector.ClearSelection ();
				}
				Find.Selector.Select (thing, true, true);
				Find.MainTabsRoot.SetCurrentTab (MainTabDefOf.Inspect, true);
			}
		}
	}

	public abstract class StringOptionsPreference : IPreference
	{
		public delegate void ValueChangedHandler (string value);

		public static float LabelMargin = StringOptionsPreference.RadioButtonWidth + StringOptionsPreference.RadioButtonMargin;

		public static float RadioButtonMargin = 18;

		public static float RadioButtonWidth = 24;

		private string stringValue;

		private string setValue = null;

		public int tooltipId = 0;

		public abstract string DefaultValue {
			get;
		}

		public virtual bool Disabled {
			get {
				return false;
			}
		}

		public virtual bool DisplayInOptions {
			get {
				return true;
			}
		}

		public abstract string Group {
			get;
		}

		public virtual bool Indent {
			get {
				return false;
			}
		}

		public virtual string Label {
			get {
				return Translator.Translate (this.Name);
			}
		}

		public abstract string Name {
			get;
		}

		public abstract string OptionValuePrefix {
			get;
		}

		public abstract IEnumerable<string> OptionValues {
			get;
		}

		public virtual string Tooltip {
			get {
				return null;
			}
		}

		protected virtual int TooltipId {
			get {
				int result;
				if (this.tooltipId == 0) {
					this.tooltipId = Translator.Translate (this.Tooltip).GetHashCode ();
					result = this.tooltipId;
				}
				else {
					result = 0;
				}
				return result;
			}
		}

		public virtual string Value {
			get {
				string defaultValue;
				if (this.setValue != null) {
					defaultValue = this.setValue;
				}
				else {
					defaultValue = this.DefaultValue;
				}
				return defaultValue;
			}
			set {
				string text = this.setValue;
				this.setValue = value;
				this.stringValue = value.ToString ();
				if ((text == null || text != value) && this.ValueChanged != null) {
					this.ValueChanged (value);
				}
			}
		}

		public virtual string ValueForDisplay {
			get {
				string defaultValue;
				if (this.setValue != null) {
					defaultValue = this.setValue;
				}
				else {
					defaultValue = this.DefaultValue;
				}
				return defaultValue;
			}
		}

		public string ValueForSerialization {
			get {
				return this.stringValue;
			}
			set {
				this.stringValue = value;
				this.setValue = value;
			}
		}

		public StringOptionsPreference ()
		{
		}

		public void OnGUI (float positionX, ref float positionY, float width)
		{
			bool disabled = this.Disabled;
			if (disabled) {
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			float num = this.Indent ? WidgetDrawer.IndentSize : 0;
			foreach (string current in this.OptionValues) {
				string text = Translator.Translate (this.OptionValuePrefix + "." + current);
				float num2 = Text.CalcHeight (text, width - StringOptionsPreference.LabelMargin - num);
				Rect rect = new Rect (positionX - 4 + num, positionY - 3, width + 6 - num, num2 + 5);
				if (Mouse.IsOver (rect)) {
					Widgets.DrawHighlight (rect);
				}
				Rect rect2 = new Rect (positionX + num, positionY, width - StringOptionsPreference.LabelMargin - num, num2);
				GUI.Label (rect2, text);
				if (this.Tooltip != null) {
					TipSignal tipSignal = new TipSignal (() => Translator.Translate (this.Tooltip), this.TooltipId);
					TooltipHandler.TipRegion (rect2, tipSignal);
				}
				string valueForDisplay = this.ValueForDisplay;
				bool flag = valueForDisplay == current;
				if (Widgets.RadioButton (new Vector2 (positionX + width - StringOptionsPreference.RadioButtonWidth, positionY - 3), flag) && !disabled) {
					this.Value = current;
				}
				positionY += num2 + WidgetDrawer.PreferencePadding.y;
			}
			positionY -= WidgetDrawer.PreferencePadding.y;
			GUI.color = Color.white;
		}

		public event StringOptionsPreference.ValueChangedHandler ValueChanged;
	}

	public static class ThingDefExtensions
	{
		public static bool BelongsToCategory (this ThingDef def, string category)
		{
			bool result;
			if (def.thingCategories != null) {
				result = (def.thingCategories.FirstOrDefault ((ThingCategoryDef d) => category.Equals (d.defName)) != null);
			}
			else {
				result = false;
			}
			return result;
		}

		public static bool DeploysFromEscapePod (this ThingDef def)
		{
			return def.apparel != null || (def.weaponTags != null && def.weaponTags.Count > 0) || def.BelongsToCategory ("FoodMeals") || (def == ThingDefOf.Medicine || def.defName.Equals ("GlitterworldMedicine"));
		}
	}

	public class TrackedColonist
	{
		private Pawn pawn;

		private Faction capturingFaction;

		private Corpse corpse;

		private int missingTimestamp;

		private bool dead;

		private bool missing;

		private bool cryptosleep;

		public bool Captured {
			get {
				return this.capturingFaction != null;
			}
		}

		public Faction CapturingFaction {
			get {
				return this.capturingFaction;
			}
			set {
				this.capturingFaction = value;
			}
		}

		public Pawn Carrier {
			get {
				Pawn result;
				if (this.pawn.holder != null && this.pawn.holder.owner != null) {
					Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
					if (pawn_CarryTracker != null) {
						result = pawn_CarryTracker.pawn;
						return result;
					}
				}
				result = null;
				return result;
			}
		}

		public bool Controllable {
			get {
				return !this.Missing && !this.Dead && !this.Captured && !this.Incapacitated && !this.InMentalState && !this.Cryptosleep;
			}
		}

		public Corpse Corpse {
			get {
				return this.corpse;
			}
			set {
				this.corpse = value;
			}
		}

		public bool Cryptosleep {
			get {
				return this.cryptosleep;
			}
			set {
				this.cryptosleep = value;
			}
		}

		public bool Dead {
			get {
				return this.dead;
			}
			set {
				this.dead = value;
			}
		}

		public bool Drafted {
			get {
				return !this.dead && this.pawn.Drafted;
			}
		}

		public float HealthPercent {
			get {
				float result;
				if (this.pawn.health != null && this.pawn.health.summaryHealth != null) {
					result = this.pawn.health.summaryHealth.SummaryHealthPercent;
				}
				else {
					result = 0;
				}
				return result;
			}
		}

		public bool Incapacitated {
			get {
				return this.pawn.health != null && this.pawn.health.Downed;
			}
		}

		public bool InMentalState {
			get {
				return this.MentalState != null;
			}
		}

		public int MentalBreakWarningLevel {
			get {
				int result;
				if (this.pawn.mindState != null && this.pawn.mindState.mentalStateStarter != null && !this.pawn.Downed && !this.pawn.Dead) {
					if (this.pawn.mindState.mentalStateStarter.HardMentalStateImminent) {
						result = 2;
						return result;
					}
					if (this.pawn.mindState.mentalStateStarter.MentalStateApproaching) {
						result = 1;
						return result;
					}
				}
				result = 0;
				return result;
			}
		}

		public MentalStateDef MentalState {
			get {
				MentalStateDef result;
				if (this.pawn.InMentalState) {
					result = this.pawn.MentalStateDef;
				}
				else {
					result = null;
				}
				return result;
			}
		}

		public bool Missing {
			get {
				return this.missing;
			}
			set {
				this.missing = value;
			}
		}

		public int MissingTimestamp {
			get {
				return this.missingTimestamp;
			}
			set {
				this.missingTimestamp = value;
			}
		}

		public Pawn Pawn {
			get {
				return this.pawn;
			}
			set {
				this.pawn = value;
			}
		}

		public TrackedColonist ()
		{
		}

		public TrackedColonist (Pawn pawn)
		{
			this.pawn = pawn;
			this.dead = false;
			this.missing = false;
			this.missingTimestamp = 0;
			this.corpse = null;
			this.capturingFaction = null;
			this.cryptosleep = false;
		}
	}

	public static class WidgetDrawer
	{
		public static LazyLoadTexture RadioButOnTex = new LazyLoadTexture ("UI/Widgets/RadioButOn");

		public static float LabelMargin = WidgetDrawer.CheckboxWidth + WidgetDrawer.CheckboxMargin;

		public static float CheckboxMargin = 18;

		public static float CheckboxHeight = 30;

		public static float CheckboxWidth = 24;

		public static Vector2 PreferencePadding = new Vector2 (8, 6);

		public static LazyLoadTexture RadioButOffTex = new LazyLoadTexture ("UI/Widgets/RadioButOff");

		public static Color DisabledControlColor = new Color (1, 1, 1, 0.5f);

		public static float SectionPadding = 14;

		public static float IndentSize = 16;

		public static float DrawLabeledCheckbox (Rect rect, string labelText, ref bool value)
		{
			return WidgetDrawer.DrawLabeledCheckbox (rect, labelText, ref value, false);
		}

		public static float DrawLabeledCheckbox (Rect rect, string labelText, ref bool value, bool disabled)
		{
			Text.Anchor = TextAnchor.UpperLeft;
			float num = rect.width - WidgetDrawer.LabelMargin;
			float num2 = Text.CalcHeight (labelText, num);
			Rect rect2 = new Rect (rect.x, rect.y, num, num2);
			Color color = GUI.color;
			if (disabled) {
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			Widgets.Label (rect2, labelText);
			GUI.color = color;
			Widgets.Checkbox (new Vector2 (rect.x + num + WidgetDrawer.CheckboxMargin, rect.y - 1), ref value, 24, disabled);
			return (num2 < WidgetDrawer.CheckboxHeight) ? WidgetDrawer.CheckboxHeight : num2;
		}

		public static bool DrawLabeledRadioButton (Rect rect, string labelText, bool chosen, bool playSound)
		{
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect2 = new Rect (rect.x, rect.y - 2, rect.width, rect.height);
			Widgets.Label (rect2, labelText);
			Text.Anchor = anchor;
			bool flag = Widgets.InvisibleButton (rect);
			if (playSound && flag && !chosen) {
				SoundStarter.PlayOneShotOnCamera (SoundDefOf.RadioButtonClicked);
			}
			Vector2 topLeft = new Vector2 (rect.x + rect.width - 32, rect.y + rect.height / 2 - 16);
			WidgetDrawer.DrawRadioButton (topLeft, chosen);
			return flag;
		}

		public static void DrawRadioButton (Vector2 topLeft, bool chosen)
		{
			Texture2D texture;
			if (chosen) {
				texture = WidgetDrawer.RadioButOnTex.Texture;
			}
			else {
				texture = WidgetDrawer.RadioButOffTex.Texture;
			}
			Rect rect = new Rect (topLeft.x, topLeft.y, 24, 24);
			GUI.DrawTexture (rect, texture);
		}
	}
}
