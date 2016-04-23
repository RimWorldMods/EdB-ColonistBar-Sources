using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace EdB.Interface
{
	public abstract class MultipleSelectionStringOptionsPreference : IPreference
	{
		public delegate void ValueChangedHandler(IEnumerable<string> selectedOptions);

		private string stringValue;

		private string setValue = null;

		public int tooltipId = 0;

		public HashSet<string> selectedOptions = new HashSet<string>();

		public static float RadioButtonWidth = 24f;

		public static float RadioButtonMargin = 18f;

		public static float LabelMargin = MultipleSelectionStringOptionsPreference.RadioButtonWidth + MultipleSelectionStringOptionsPreference.RadioButtonMargin;

		public event MultipleSelectionStringOptionsPreference.ValueChangedHandler ValueChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ValueChanged = (MultipleSelectionStringOptionsPreference.ValueChangedHandler)Delegate.Combine(this.ValueChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ValueChanged = (MultipleSelectionStringOptionsPreference.ValueChangedHandler)Delegate.Remove(this.ValueChanged, value);
			}
		}

		public IEnumerable<string> SelectedOptions
		{
			get
			{
				return this.selectedOptions;
			}
		}

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
				this.selectedOptions.Clear();
				string[] array = this.stringValue.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (this.OptionValues.Contains(text))
					{
						this.selectedOptions.Add(text);
					}
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
				return true;
			}
		}

		public MultipleSelectionStringOptionsPreference()
		{
		}

		public bool IsOptionSelected(string option)
		{
			return this.selectedOptions.Contains(option);
		}

		public void UpdateOption(string option, bool value)
		{
			if (!value)
			{
				if (this.selectedOptions.Contains(option))
				{
					this.selectedOptions.Remove(option);
					this.UpdateSerializedValue();
					if (this.ValueChanged != null)
					{
						this.ValueChanged(this.selectedOptions);
					}
				}
			}
			else if (!this.selectedOptions.Contains(option))
			{
				this.selectedOptions.Add(option);
				this.UpdateSerializedValue();
				if (this.ValueChanged != null)
				{
					this.ValueChanged(this.selectedOptions);
				}
			}
		}

		protected void UpdateSerializedValue()
		{
			this.stringValue = string.Join(",", this.selectedOptions.ToArray<string>());
		}

		public virtual string OptionTranslated(string optionValue)
		{
			return (this.OptionValuePrefix + "." + optionValue).Translate();
		}

		public void OnGUI(float positionX, ref float positionY, float width)
		{
			bool disabled = this.Disabled;
			if (disabled)
			{
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				string text = this.Name.Translate();
				float num = Text.CalcHeight(text, width);
				Rect rect = new Rect(positionX, positionY, width, num);
				Widgets.Label(rect, text);
				if (this.Tooltip != null)
				{
					TipSignal tip = new TipSignal(() => this.Tooltip.Translate(), this.TooltipId);
					TooltipHandler.TipRegion(rect, tip);
				}
				positionY += num + WidgetDrawer.PreferencePadding.y;
			}
			float num2 = this.Indent ? WidgetDrawer.IndentSize : 0f;
			foreach (string current in this.OptionValues)
			{
				string text = this.OptionTranslated(current);
				if (!string.IsNullOrEmpty(text))
				{
					float num = Text.CalcHeight(text, width - MultipleSelectionStringOptionsPreference.LabelMargin - num2);
					Rect rect2 = new Rect(positionX - 4f + num2, positionY - 3f, width + 6f - num2, num + 5f);
					if (Mouse.IsOver(rect2))
					{
						Widgets.DrawHighlight(rect2);
					}
					Rect rect = new Rect(positionX + num2, positionY, width - num2, num);
					bool flag = this.IsOptionSelected(current);
					bool flag2 = flag;
					WidgetDrawer.DrawLabeledCheckbox(rect, text, ref flag);
					if (flag != flag2)
					{
						this.UpdateOption(current, flag);
					}
					positionY += num + WidgetDrawer.PreferencePadding.y;
				}
			}
			positionY -= WidgetDrawer.PreferencePadding.y;
			GUI.color = Color.white;
		}
	}
}
