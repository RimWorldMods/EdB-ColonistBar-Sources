using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace EdB.Interface
{
	public abstract class StringOptionsPreference : IPreference
	{
		public delegate void ValueChangedHandler(string value);

		private string stringValue;

		private string setValue = null;

		public int tooltipId = 0;

		public static float RadioButtonWidth = 24f;

		public static float RadioButtonMargin = 18f;

		public static float LabelMargin = StringOptionsPreference.RadioButtonWidth + StringOptionsPreference.RadioButtonMargin;

		public event StringOptionsPreference.ValueChangedHandler ValueChanged;

		public abstract string Name
		{
			get;
		}

		public abstract string Group
		{
			get;
		}

		public virtual string Tooltip
		{
			get
			{
				return null;
			}
		}

		public virtual bool DisplayInOptions
		{
			get
			{
				return true;
			}
		}

		protected virtual int TooltipId
		{
			get
			{
				int result;
				if (this.tooltipId == 0)
				{
					this.tooltipId = this.Tooltip.Translate().GetHashCode();
					result = this.tooltipId;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public virtual string Label
		{
			get
			{
				return this.Name.Translate();
			}
		}

		public abstract string DefaultValue
		{
			get;
		}

		public virtual bool Disabled
		{
			get
			{
				return false;
			}
		}

		public abstract IEnumerable<string> OptionValues
		{
			get;
		}

		public abstract string OptionValuePrefix
		{
			get;
		}

		public string ValueForSerialization
		{
			get
			{
				return this.stringValue;
			}
			set
			{
				this.stringValue = value;
				this.setValue = value;
			}
		}

		public virtual string Value
		{
			get
			{
				string defaultValue;
				if (this.setValue != null)
				{
					defaultValue = this.setValue;
				}
				else
				{
					defaultValue = this.DefaultValue;
				}
				return defaultValue;
			}
			set
			{
				string text = this.setValue;
				this.setValue = value;
				this.stringValue = value.ToString();
				if ((text == null || text != value) && this.ValueChanged != null)
				{
					this.ValueChanged(value);
				}
			}
		}

		public virtual string ValueForDisplay
		{
			get
			{
				string defaultValue;
				if (this.setValue != null)
				{
					defaultValue = this.setValue;
				}
				else
				{
					defaultValue = this.DefaultValue;
				}
				return defaultValue;
			}
		}

		public virtual bool Indent
		{
			get
			{
				return false;
			}
		}

		public StringOptionsPreference()
		{
		}

		public void OnGUI(float positionX, ref float positionY, float width)
		{
			bool disabled = this.Disabled;
			if (disabled)
			{
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			float num = this.Indent ? WidgetDrawer.IndentSize : 0f;
			foreach (string current in this.OptionValues)
			{
				string text = (this.OptionValuePrefix + "." + current).Translate();
				float num2 = Text.CalcHeight(text, width - StringOptionsPreference.LabelMargin - num);
				Rect rect = new Rect(positionX - 4f + num, positionY - 3f, width + 6f - num, num2 + 5f);
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				Rect rect2 = new Rect(positionX + num, positionY, width - StringOptionsPreference.LabelMargin - num, num2);
				GUI.Label(rect2, text);
				if (this.Tooltip != null)
				{
					TipSignal tip = new TipSignal(() => this.Tooltip.Translate(), this.TooltipId);
					TooltipHandler.TipRegion(rect2, tip);
				}
				string valueForDisplay = this.ValueForDisplay;
				bool chosen = valueForDisplay == current;
				if (Widgets.RadioButton(new Vector2(positionX + width - StringOptionsPreference.RadioButtonWidth, positionY - 3f), chosen) && !disabled)
				{
					this.Value = current;
				}
				positionY += num2 + WidgetDrawer.PreferencePadding.y;
			}
			positionY -= WidgetDrawer.PreferencePadding.y;
			GUI.color = Color.white;
		}
	}
}
