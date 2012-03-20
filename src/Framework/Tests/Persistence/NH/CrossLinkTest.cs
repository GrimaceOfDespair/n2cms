using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.CrossLinks;
using N2.Definitions.Static;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;

namespace N2.Tests.Persistence.NH
{
    public class CrossLinkTest : PersistenceAwareBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            base.CreateDatabaseSchema();
        }

        [Test]
        public void CrossLinksArePersisted()
        {
            var childItem = CreateAndSaveItem<ChildItem>("child item", null);
            var parentItem = CreateAndSaveItem<ParentItem>("parent item", null);

            var crossLink = parentItem.AddCrossLink(childItem, item => item.ChildItems);
            engine.Persister.Save(crossLink);

            var parentItemFromDb = engine.Persister.Get<ParentItem>(parentItem.ID);

            Assert.That(parentItemFromDb, Is.Not.Null, "Parent item was not found in database.");
            Assert.That(parentItemFromDb.ChildItems, Is.Not.Null, "No child items were found in database.");
            Assert.That(parentItemFromDb.ChildItems.Count, Is.EqualTo(1), "An invalid number of child items were retrieved.");
            Assert.That(parentItemFromDb.ChildItems[0].Title, Is.EqualTo("child item"), "The child item was retrieved, but contains invalid data");
        }

        [Test]
        public void CrossLinkTypesAreCreated()
        {
            var crossLinkType = Context.Current.Resolve<CrossLinkTypesRepository>().Get<ChildItem>();
            Assert.That(crossLinkType, Is.Not.Null);
            Assert.That(crossLinkType.Name, Is.EqualTo("ChildItemLink"));
        }

        protected T CreateAndSaveItem<T>(string title, ContentItem parent) where T : ContentItem
        {
            var item = CreateOneItem<T>(0, "name", parent);
            item.Title = title;
            engine.Persister.Save(item);

            return item;
        }

    }
}
