using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using N2.Collections;

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
             
            return children.OfType<T>().ToList();
        }

        public static ContentItem AddCrossLink<TSource,TTarget>(this TSource parentItem, TTarget childItem, Expression<Func<TSource, IList<TTarget>>> expression)
            where TSource : ContentItem
            where TTarget : ContentItem
        {
            var crossLinkType = typeof(TSource).GetCrossLinkType();
            var crossLink = (ContentItem)Activator.CreateInstance(crossLinkType);
            crossLink.Title = childItem.Title;
            crossLink.ZoneName = ExpressionHelper.GetExpressionText(expression);

            return crossLink;
        }

        public static Type GetCrossLinkType(this Type childType)
        {
            return typeof(CrossLink<>)
                .MakeGenericType(childType);
        }

        //public static IEnumerable<Type> GetLinkType(this IEnumerable<Type> linkedTypes)
        //{
        //    return linkedTypes.Select(linkedType =>

        //        linkedType.GetLinkType());
        //}

        //public static Type GetLinkType(this Type linkedType)
        //{
        //    return linkedType;
        //}
    }
}
