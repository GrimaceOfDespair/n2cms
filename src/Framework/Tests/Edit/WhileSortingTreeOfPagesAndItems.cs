﻿using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Edit;
using N2.Persistence;
using N2.Tests.Edit.Items;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class WhileSortingTreeOfPagesAndItems : ItemTestsBase
	{
		List<ContentItem> savedItems = new List<ContentItem>();
		TreeSorter sorter;
		NormalPage root, page1, page3, page5;
		NormalItem item2, item4;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			IPersister persister = mocks.Stub<IPersister>();
			Expect.Call(delegate { persister.Save(null); })
				.IgnoreArguments()
				.Do(new Action<ContentItem>(savedItems.Add))
				.Repeat.Any();

			IEditManager editManager = mocks.Stub<IEditManager>();
			Expect.Call(editManager.GetEditorFilter(null))
				.IgnoreArguments()
				.Return(new PageFilter());

			IWebContext webContext = mocks.Stub<IWebContext>();
			
			mocks.ReplayAll();

			root = CreateOneItem<NormalPage>(1, "root", null);
			page1 = CreateOneItem<NormalPage>(2, "page1", root);
			item2 = CreateOneItem<NormalItem>(3, "item2", root);
			page3 = CreateOneItem<NormalPage>(4, "page3", root);
			item4 = CreateOneItem<NormalItem>(5, "item4", root);
			page5 = CreateOneItem<NormalPage>(6, "page5", root);
			N2.Utility.UpdateSortOrder(root.Children);
			
			sorter = new TreeSorter(persister, editManager, webContext);
		}

		[Test]
		public void PageMovedUp_SkipsNonPages()
		{
			sorter.MoveUp(page3);
			Assert.That(root.Children[0], Is.EqualTo(page3));
		}

		[Test]
		public void PageMovedDown_SkipsNonPages()
		{
			sorter.MoveDown(page3);
			Assert.That(root.Children[4], Is.EqualTo(page3));
		}
	}
}
