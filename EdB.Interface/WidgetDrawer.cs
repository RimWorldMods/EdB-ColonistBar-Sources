using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EdB.Interface
{
	public static class WidgetDrawer
	{
		public static LazyLoadTexture RadioButOnTex = new LazyLoadTexture("UI/Widgets/RadioButOn");

		public static LazyLoadTexture RadioButOffTex = new LazyLoadTexture("UI/Widgets/RadioButOff");

		public static Color DisabledControlColor = new Color(1f, 1f, 1f, 0.5f);

		public static float SectionPadding = 14f;

		public static float IndentSize = 16f;

		public static Vector2 PreferencePadding = new Vector2(8f, 6f);

		public static float CheckboxWidth = 24f;

		public static float CheckboxHeight = 30f;

		public static float CheckboxMargin = 18f;

		public static float LabelMargin = WidgetDrawer.CheckboxWidth + WidgetDrawer.CheckboxMargin;

		public static bool DrawLabeledRadioButton(Rect rect, string labelText, bool chosen, bool playSound)
		{
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect2 = new Rect(rect.x, rect.y - 2f, rect.width, rect.height);
			Widgets.Label(rect2, labelText);
			Text.Anchor = anchor;
			bool flag = Widgets.InvisibleButton(rect);
			if (playSound && flag && !chosen)
			{
				SoundDefOf.RadioButtonClicked.PlayOneShotOnCamera();
			}
			Vector2 topLeft = new Vector2(rect.x + rect.width - 32f, rect.y + rect.height / 2f - 16f);
			WidgetDrawer.DrawRadioButton(topLeft, chosen);
			return flag;
		}

		public static void DrawRadioButton(Vector2 topLeft, bool chosen)
		{
			Texture2D texture;
			if (chosen)
			{
				texture = WidgetDrawer.RadioButOnTex.Texture;
			}
			else
			{
				texture = WidgetDrawer.RadioButOffTex.Texture;
			}
			Rect position = new Rect(topLeft.x, topLeft.y, 24f, 24f);
			GUI.DrawTexture(position, texture);
		}

		public static float DrawLabeledCheckbox(Rect rect, string labelText, ref bool value)
		{
			return WidgetDrawer.DrawLabeledCheckbox(rect, labelText, ref value, false);
		}

		public static float DrawLabeledCheckbox(Rect rect, string labelText, ref bool value, bool disabled)
		{
			Text.Anchor = TextAnchor.UpperLeft;
			float num = rect.width - WidgetDrawer.LabelMargin;
			float num2 = Text.CalcHeight(labelText, num);
			Rect rect2 = new Rect(rect.x, rect.y, num, num2);
			Color color = GUI.color;
			if (disabled)
			{
				GUI.color = WidgetDrawer.DisabledControlColor;
			}
			Widgets.Label(rect2, labelText);
			GUI.color = color;
			Widgets.Checkbox(new Vector2(rect.x + num + WidgetDrawer.CheckboxMargin, rect.y - 1f), ref value, 24f, disabled);
			return (num2 < WidgetDrawer.CheckboxHeight) ? WidgetDrawer.CheckboxHeight : num2;
		}
	}
}
