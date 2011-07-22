using System;
using N2.Collections;

namespace N2.Tests.Web.Items
{
	[PageDefinition]
	public class PageItem : ContentItem
	{
		public override string Url
		{
			get { return "/" + Name + Extension; }
		}

		[Obsolete]
		public override string RewrittenUrl
		{
			get { return TemplateUrl.TrimStart('~') + "?page=" + ID; }
		}

		public override ItemList GetChildren()
		{
			return GetChildren(new ItemFilter[0]);
		}

		public override ItemList GetChildren(string childZoneName)
		{
			return GetChildren(new ZoneFilter(childZoneName));
		}
	}
}