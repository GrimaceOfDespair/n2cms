﻿using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Web.UI.WebControls;

namespace N2.Edit.Navigation
{
	public class LoadTree : HelpfulHandler
	{
		public override void ProcessRequest(HttpContext context)
		{
            string target = context.Request["target"] ?? Targets.Preview;

			var selection = new SelectionUtility(context.Request, N2.Context.Current);
			ContentItem selectedNode = selection.SelectedItem;
			
			context.Response.ContentType = "text/plain";

			ItemFilter filter = Engine.EditManager.GetEditorFilter(context.User);
			IContentAdapterProvider adapters = Engine.Resolve<IContentAdapterProvider>();
			var root = new TreeHierarchyBuilder(selectedNode, 2)
				.Children((item) => adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing))
				.Build();

			string selectableTypes = context.Request["selectableTypes"];
			string selectableExtensions = context.Request["selectableExtensions"];
			TreeNode tn = (TreeNode)new N2.Web.Tree(root)
				.LinkWriter((n, w) => Web.UI.Controls.Tree.BuildLink(adapters.ResolveAdapter<NodeAdapter>(n.Current), n.Current, n.Current.Path == selectedNode.Path, target, N2.Edit.Web.UI.Controls.Tree.IsSelectable(n.Current, selectableTypes, selectableExtensions)).WriteTo(w))
				.Filters(filter)
				.ToControl();

			Web.UI.Controls.Tree.AppendExpanderNodeRecursive(tn, filter, target, adapters, selectableTypes, selectableExtensions);

			RenderControls(tn.Controls, context.Response.Output);
		}

		private static void RenderControls(IEnumerable controls, TextWriter output)
		{
			using (HtmlTextWriter writer = new HtmlTextWriter(output))
			{
				foreach (Control childNode in controls)
				{
					childNode.RenderControl(writer);
				}
			}
		}
	}
}
