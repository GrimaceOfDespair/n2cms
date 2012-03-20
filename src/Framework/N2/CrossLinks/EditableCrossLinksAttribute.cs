using System;
using System.Web.UI.WebControls;
using N2.Details;

namespace N2.CrossLinks
{
    public class EditableCrossLinksAttribute : EditableListControlAttribute
    {
        public EditableCrossLinksAttribute(Type linkedType)
        {
            LinkedType = linkedType;
        }

        public EditableCrossLinksAttribute(Type linkedType, string title, int sortOrder) : base(title, sortOrder)
        {
            LinkedType = linkedType;
        }

        public Type LinkedType { get; private set; }

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
