using System;
using System.Linq;
using N2.Definitions.Static;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH.Finder;
using N2.Tests.Persistence.Definitions;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Persistence.NH
{
	[TestFixture, Category("Integration")]
	public class PerformanceTests : DatabasePreparingBase
	{
		#region SetUp

		IItemFinder finder;
		ContentItem rootItem;
		ContentItem startPage;
		ContentItem item1;
		ContentItem item2;
		ContentItem item3;
		ContentItem item1_a;
		ContentItem item1_b;
		ContentItem item1_c;
		ContentItem item2_a;
		ContentItem item2_b;
		ContentItem item2_c;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateRootItem();
			SaveVersionAndUpdateRootItem();
			CreateStartPageBelow(rootItem);

			item1 = CreatePageBelow(startPage, 1);
			item1.SortOrder = 10;

			item2 = CreatePageBelow(startPage, 2);
			item2.SortOrder = 30;

			item3 = CreatePageBelow(startPage, 3);
			item3.SortOrder = 20;

			item1_a = CreatePageBelow(item1, 1);
			item1_b = CreatePageBelow(item1, 2);
			item1_c = CreatePageBelow(item1, 3);
			item2_a = CreatePageBelow(item2, 1);
			item2_b = CreatePageBelow(item2, 2);
			item2_c = CreatePageBelow(item2, 3);

			engine.Resolve<IHost>().DefaultSite.RootItemID = rootItem.ID;
			engine.Resolve<IHost>().DefaultSite.StartPageID = startPage.ID;

			finder = new ItemFinder(sessionProvider, new DefinitionMap());

			sessionProvider.OpenSession.Session.Clear();
		}

		#endregion

		[Test]
		public void TypeFiltering_DoesNotSelectNPlus1()
		{
			var items = finder.Where.Type.Eq(typeof(PersistableItem2))
				.Select<PersistableItem2>();
			Assert.AreEqual(9, items.Count);
			EnumerableAssert.Contains(items, item1);
			EnumerableAssert.Contains(items, item2);
			EnumerableAssert.Contains(items, item3);

			// TODO: check why increase
			Assert.That(sessionProvider.SessionFactory.Statistics.QueryExecutionCount, Is.LessThanOrEqualTo(2));
			Assert.That(sessionProvider.SessionFactory.Statistics.GetEntityStatistics("PersistableItem2").FetchCount, Is.EqualTo(0));
		}

		[Test]
		public void ChildSorting()
		{
			HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

			// Load items into second-level cache
			engine.Persister.Get(item1_a.ID);
			var selectedItem = engine.Persister.Get(item1_b.ID);
			engine.Persister.Get(item1_c.ID);
			var item1Children = finder.Where.Parent.Eq(item1).Select<PersistableItem2>().ToList();

			// Put one of the loaded items into the DetailCollection
			var changeItem = engine.Persister.Get(item2_b.ID);
			changeItem.Title = "Changed Item";
			changeItem.GetDetailCollection("SelectedItem", true).Add(selectedItem);
			engine.Persister.Save(changeItem);

			// Verify performance
			Assert.That(sessionProvider.SessionFactory.Statistics.QueryExecutionCount, Is.LessThanOrEqualTo(3));
		}

		#region Helpers

		private ContentItem CreatePageBelow(ContentItem parentPage, int index)
		{
			ContentItem item = CreateOneItem<PersistableItem2>(0, "item" + index, parentPage);

			N2.Details.DetailCollection details = item.GetDetailCollection("DetailCollection", true);
			details.Add(true);
			details.Add(index * 1000 + 555);
			details.Add(index * 1000.0 + 555.55);
			details.Add("string in a collection " + index);
			details.Add(parentPage);
			details.Add(new DateTime(2009 + index, 1, 1));

			engine.Persister.Save(item);
			return item;
		}

		private void CreateStartPageBelow(ContentItem root)
		{
			startPage = CreateOneItem<PersistableItem1>(0, "start page", root);
			startPage.ZoneName = "AZone";
			startPage.SortOrder = 34;
			startPage.Visible = true;
			startPage["IntDetail"] = 45;
			startPage["DoubleDetail"] = 56.66;
			startPage["BoolDetail"] = true;
			startPage["DateDetail"] = new DateTime(2000, 01, 01);
			startPage["StringDetail"] = "actually another string";
			startPage["StringDetail2"] = "just a string";
			startPage["ObjectDetail"] = new string[] { "two", "three", "four" };
			startPage["ItemDetail"] = root;

			engine.Persister.Save(startPage);
		}

		private void SaveVersionAndUpdateRootItem()
		{
			engine.Resolve<IVersionManager>().SaveVersion(rootItem);

			rootItem.Created = DateTime.Today;
			rootItem.Published = new DateTime(2007, 06, 03);
			rootItem.Expires = new DateTime(2017, 06, 03);
			rootItem.ZoneName = "TheZone";
			rootItem.SortOrder = 23;
			rootItem.Visible = false;
			rootItem["IntDetail"] = 43;
			rootItem["DoubleDetail"] = 43.33;
			rootItem["BoolDetail"] = false;
			rootItem["DateDetail"] = new DateTime(1999, 12, 31);
			rootItem["StringDetail"] = "just a string";
			rootItem["StringDetail2"] = "just another string";
			rootItem["ObjectDetail"] = new string[] { "one", "two", "three" };

			engine.Persister.Save(rootItem);
		}

		private void CreateRootItem()
		{
			rootItem = CreateOneItem<PersistableItem1>(0, "root", null);
			rootItem.Created = new DateTime(2007, 06, 01);
			rootItem.Published = new DateTime(2007, 06, 02);
			rootItem.Expires = new DateTime(2017, 06, 02);
			rootItem.ZoneName = "ZaZone";
			rootItem.SortOrder = 12;
			rootItem.Visible = true;
			rootItem["IntDetail"] = 32;
			rootItem["DoubleDetail"] = 32.22;
			rootItem["BoolDetail"] = true;
			rootItem["DateDetail"] = new DateTime(1998, 12, 31);
			rootItem["StringDetail"] = "a string in a version";
			rootItem["StringDetail2"] = "just a string";
			rootItem["ObjectDetail"] = new string[] { "zero", "one", "two" };

			engine.Persister.Save(rootItem);
		}

		#endregion
	}
}