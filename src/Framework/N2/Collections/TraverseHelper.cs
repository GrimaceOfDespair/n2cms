﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Collections;
using N2.Definitions;
using N2.Engine.Globalization;
using N2.Persistence.Finder;
using N2.Web.Mvc.Html;
using N2.Persistence.NH;
using System;
using N2.Engine;
using N2.Web;

namespace N2.Collections
{
	/// <summary>
	/// Simplifies traversing items in the content hierarchy.
	/// </summary>
	public class TraverseHelper
	{
		IEngine engine;
		FilterHelper filter;
		Func<PathData> pathGetter;
		ItemFilter defaultFilter;

		public TraverseHelper(IEngine engine, FilterHelper filter, Func<PathData> pathGetter)
		{
			this.engine = engine;
			this.filter = filter;
			this.pathGetter = pathGetter;
		}

		public ContentItem CurrentItem
		{
			get { return pathGetter().CurrentItem; }
		}

		public ContentItem CurrentPage
		{
			get { return pathGetter().CurrentPage; }
		}

		public ContentItem StartPage
		{
			get 
			{ 
				var page = N2.Find.ClosestOf<IStartPage>(CurrentItem) ?? engine.UrlParser.StartPage;
				var redirect = page as IRedirect;
				if (redirect != null && redirect.RedirectTo != null)
					return redirect.RedirectTo;
				return page;
			}
		}

		public ContentItem RootPage
		{
			get { return N2.Find.ClosestOf<IRootPage>(CurrentItem) ?? engine.Persister.Repository.Get(engine.Resolve<IHost>().CurrentSite.RootItemID); }
		}

		public ILanguage CurrentLanguage
		{
			get { return engine.Resolve<ILanguageGateway>().GetLanguage(CurrentPage); }
		}

		public ItemFilter DefaultFilter
		{
			get { return defaultFilter ?? (defaultFilter = filter.Accessible()); }
			set { defaultFilter = value; }
		}

		public IEnumerable<ILanguage> Translations()
		{
			var lg = engine.Resolve<ILanguageGateway>();
			return lg.FindTranslations(CurrentPage).Select(i => lg.GetLanguage(i));
		}

		public IEnumerable<ContentItem> Ancestors(ContentItem item = null, ItemFilter filter = null)
		{
			return (filter ?? DefaultFilter).Pipe(N2.Find.EnumerateParents(item ?? CurrentItem, StartPage, true));
		}

		public IEnumerable<ContentItem> AncestorsBetween(int startLevel = 0, int stopLevel = 5)
		{
			var ancestors = N2.Find.EnumerateParents(CurrentItem, StartPage, true).ToList();
			ancestors.Reverse();
			if (stopLevel < 0)
				stopLevel = ancestors.Count + stopLevel;

			if (startLevel < stopLevel)
				for (int i = startLevel; i < stopLevel && i < ancestors.Count; i++)
					yield return ancestors[i];
			else
				for (int i = Math.Min(stopLevel, ancestors.Count - 1); i >= startLevel; i--)
					yield return ancestors[i];
		}

		public IEnumerable<ContentItem> Children()
		{
			return Children(null);
		}

		public IEnumerable<ContentItem> Children(ItemFilter filter)
		{
			return Children(CurrentItem, filter ?? DefaultFilter);
		}

		public IEnumerable<ContentItem> Children(ContentItem parent, ItemFilter filter = null)
		{
			if (parent == null) return Enumerable.Empty<ContentItem>();
			
			return parent.GetChildren(filter ?? DefaultFilter);
		}

		public IEnumerable<ContentItem> Descendants()
		{
			return N2.Find.EnumerateChildren(CurrentItem).Where(DefaultFilter);
		}

		public IEnumerable<ContentItem> Descendants(ContentItem ancestor, ItemFilter filter = null)
		{
			return N2.Find.EnumerateChildren(ancestor).Where((filter ?? DefaultFilter).Match);
		}

		public IEnumerable<ContentItem> DescendantPages(ContentItem ancestor, ItemFilter filter = null)
		{
			return N2.Find.EnumerateChildren(ancestor).Where(p => p.IsPage).Where((filter ?? DefaultFilter).Match);
		}

		public IEnumerable<ContentItem> Siblings(ContentItem sibling = null)
		{
			return Siblings(sibling, null);
		}

		public IEnumerable<ContentItem> Siblings(ItemFilter filter = null)
		{
			return Siblings(null, filter);
		}

		public IEnumerable<ContentItem> Siblings(ContentItem item = null, ItemFilter filter = null)
		{
			if (item == null) item = CurrentItem;
			if (item.Parent == null) return Enumerable.Empty<ContentItem>();

			return item.Parent.GetChildren(filter ?? DefaultFilter);
		}

		public ContentItem PreviousSibling(ContentItem item = null)
		{
			if (item == null) item = CurrentItem;

			ContentItem previous = null;
			foreach (var sibling in Siblings(item))
			{
				if (sibling == item)
					return previous;
				
				previous = sibling;
			}
			return null;
		}

		public ContentItem NextSibling(ContentItem item = null)
		{
			if (item == null) item = CurrentItem;

			bool next = false;
			foreach (var sibling in Siblings(item))
			{
				if (next)
					return sibling;
				if (sibling == item)
					next = true;
			}
			return null;
		}

		public int LevelOf(ContentItem item = null)
		{
			return Ancestors(item).Count();
		}

		public ContentItem AncestorAtLevel(int level)
		{
			return Ancestors().Reverse().Skip(level).FirstOrDefault();
		}
		
		public ContentItem Parent()
		{
			return Parent(null);
		}

		public ContentItem Parent(ContentItem item)
		{
			if (item == null) item = CurrentItem;
			if (item == StartPage) return null;

			return item.Parent;
		}

		public ContentItem Path(string path, ContentItem startItem = null)
		{
			return (startItem ?? StartPage).FindPath(path).CurrentItem;
		}
	}
}