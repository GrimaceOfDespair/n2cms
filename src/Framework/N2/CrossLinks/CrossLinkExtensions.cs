using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using N2.Collections;
using N2.Definitions;

namespace N2.CrossLinks
{
    public static class CrossLinkExtensions
    {
        public static IList<T> GetCrossLinks<T>(this ContentItem contentItem, string childZoneName) where T : ContentItem
        {
            var children = contentItem.GetChildren(
                new ZoneFilter(childZoneName),
                new AccessFilter(),
                new CrossLinkFilter<T>());
             
            return children.Select(c => (T)c.GetDetail("ContentItem")).ToList();
        }

        public static ContentItem AddCrossLink<TSource,TTarget>(this TSource parentItem, TTarget childItem, Expression<Func<TSource, IList<TTarget>>> expression)
            where TSource : ContentItem
            where TTarget : ContentItem
        {
            var crossLinkType = GetCrossLinkType<TTarget>();
            var crossLink = (ContentItem)Activator.CreateInstance(crossLinkType);

            crossLink.Parent = parentItem;
            crossLink.Title = childItem.Title;
            crossLink.SetDetail("ContentItem", childItem);

            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var editableCrossLinks = Context.Current.Resolve<AttributeExplorer>()
                .Find<EditableCrossLinksAttribute>(typeof (TSource))
                .Where(a =>
                       a.Name == propertyName).First();

            crossLink.ZoneName = editableCrossLinks.ChildZone;

            return crossLink;
        }

        public static Type GetCrossLinkType<T>()
        {
            return GetCrossLinkType(typeof(T));
        }

        public static Type GetCrossLinkType(this Type linkedType)
        {
            return Context.Current.Resolve<CrossLinkTypesRepository>().Get(linkedType);
        }
    }
}
