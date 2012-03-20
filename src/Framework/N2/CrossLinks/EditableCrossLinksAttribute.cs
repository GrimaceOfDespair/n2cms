using System;
using System.Web.UI.WebControls;
using N2.Details;

namespace N2.CrossLinks
{
    public class EditableCrossLinksAttribute : EditableListControlAttribute
    {
        public EditableCrossLinksAttribute(Type linkedType, string childZone)
        {
            LinkedType = linkedType;
            ChildZone = childZone;
        }

        public EditableCrossLinksAttribute(Type linkedType, string childZone, string title, int sortOrder) : base(title, sortOrder)
        {
            LinkedType = linkedType;
            ChildZone = childZone;
        }

        public Type LinkedType { get; private set; }
        public string ChildZone { get; private set; }
        public string Childitems { get; set; }

        protected override ListControl CreateEditor()
        {
            throw new System.NotImplementedException();
        }

        protected override ListItem[] GetListItems()
        {
            throw new System.NotImplementedException();
        }
    }
}
