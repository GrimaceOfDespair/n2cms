using N2.Edit.Web;
using N2.Edit;
using N2.Web;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;

namespace N2.Management.Myself
{
	[ToolbarPlugin("HOME", "home", "~/N2/Myself/Root.aspx", ToolbarArea.Navigation, Targets.Preview, "~/N2/Resources/icons/cog.png", -50)]
	public partial class Root : EditPage, IContentTemplate, IItemContainer
    {
		protected override void OnPreInit(System.EventArgs e)
		{
			base.OnPreInit(e);

			if (CurrentItem == null)
			{
				var root = Engine.Persister.Repository.Load(Engine.Resolve<IHost>().CurrentSite.RootItemID);
				Response.Redirect(root.Url);
			}
		}

		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			sc.Visible = Engine.SecurityManager.IsAdmin(User);

			Title = CurrentItem["AlternativeTitle"] as string;
		}

		protected override void OnPreRender(System.EventArgs e)
		{
			base.OnPreRender(e);

			if (ControlPanel.GetState(this) != ControlPanelState.DragDrop)
			{
				HideIfEmpty(c1, Zone2.DataSource);
				HideIfEmpty(c2, Zone3.DataSource);
				HideIfEmpty(c3, Zone4.DataSource);
			}
		}

		private void HideIfEmpty(Control container, IList<ContentItem> items)
		{
			container.Visible = items != null && items.Count > 0;
		}

		#region IContentTemplate Members

		public ContentItem CurrentItem { get; set; }

		#endregion
	}
}