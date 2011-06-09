﻿using System;

namespace N2.Details
{
	/// <summary>
	/// Represents the value type of a content details using multiple value types.
	/// </summary>
	public interface IMultipleValue
	{
		bool? BoolValue { get; set; }
		DateTime? DateTimeValue { get; set; }
		double? DoubleValue { get; set; }
		int? IntValue { get; set; }
		ContentItem LinkedItem { get; set; }
		object ObjectValue { get; set; }
		string StringValue { get; set; }
	}
}
