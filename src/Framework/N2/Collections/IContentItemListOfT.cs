﻿namespace N2.Collections
{
	public interface IContentItemList<T> : IContentList<T>, IZonedList<T>, IQueryableList<T> where T : ContentItem
	{
	}
}
