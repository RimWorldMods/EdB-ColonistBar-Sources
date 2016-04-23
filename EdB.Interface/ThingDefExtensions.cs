using RimWorld;
using System;
using System.Linq;
using Verse;

namespace EdB.Interface
{
	public static class ThingDefExtensions
	{
		public static bool DeploysFromEscapePod(this ThingDef def)
		{
			return def.apparel != null || (def.weaponTags != null && def.weaponTags.Count > 0) || def.BelongsToCategory("FoodMeals") || (def == ThingDefOf.Medicine || def.defName.Equals("GlitterworldMedicine"));
		}

		public static bool BelongsToCategory(this ThingDef def, string category)
		{
			bool result;
			if (def.thingCategories != null)
			{
				result = (def.thingCategories.FirstOrDefault((ThingCategoryDef d) => category.Equals(d.defName)) != null);
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
