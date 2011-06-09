using System.Collections.Generic;

namespace N2.Persistence.Finder
{
	/// <summary>
	/// Classes implementing this interface provides an endpoint for queries 
	/// allowing to select items using current criterias.
	/// </summary>
	public interface IQueryEnd
	{
		/// <summary>Selects items defined by the given criterias.</summary>
		IList<ContentItem> Select();

		/// <summary>Selects items defined by the given criterias and converts result to a strongly typed list.</summary>
		IList<T> Select<T>() where T : ContentItem;

		/// <summary>Selects the number of items matching the query.</summary>
		/// <returns>A number of items.</returns>
		int Count();
	}
}
