using System.Web.UI.WebControls;
using N2.Definitions;

namespace N2.Details
{
  public class EditableDetailCollectionAttribute : EditableDropDownAttribute
  {
    protected override ListItem[] GetListItems()
    {
      return new ListItem[0];
    }
  }
}