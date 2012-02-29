﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Edit.FileSystem.Items;
using N2.Edit.Web;
using N2.Resources;
using N2.Web.Drawing;

namespace N2.Edit.FileSystem
{
    public partial class File1 : EditPage
    {
		protected IEnumerable<ContentItem> ancestors;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Page.StyleSheet("{ManagementUrl}/Files/Css/Files.css");

			var config = Engine.Resolve<EditSection>();
			if (config.FileSystem.IsTextFile(SelectedFile.Url))
			{
				btnEdit.Visible = true;
			}
			if (ImagesUtility.IsImagePath(Selection.SelectedItem.Url))
			{
				var size = config.Images.GetImageSize(SelectedFile.Url);
				if (size != null && size.Mode == ImageResizeMode.Fill)
					hlCrop.NavigateUrl = "../Crop.aspx?selected=" + Selection.SelectedItem.Path;
				else
					hlCrop.Visible = false;
			}
			ancestors = Find.EnumerateParents(Selection.SelectedItem, null, true).Where(a => a is AbstractNode).Reverse();

			DataBind();

			LoadSizes();

			Refresh(Selection.SelectedItem, ToolbarArea.Navigation, force: false);
		}

		private void LoadSizes()
		{
			var imageConfig = Engine.Resolve<Configuration.EditSection>().Images;
			string baseImagePath, imageSize;
			ImagesUtility.SplitImageAndSize(Selection.SelectedItem.Url, imageConfig.Sizes.GetSizeNames(), out baseImagePath, out imageSize);
			foreach (var size in imageConfig.Sizes.AllElements.Where(s => s.Announced))
			{
				var hl = new HyperLink();
				hl.ID = size.Name + "Size";
				var path = ImagesUtility.GetExistingImagePath(Engine.Resolve<IFileSystem>(), baseImagePath, size.Name);
				hl.NavigateUrl = "File.aspx?selected=" + path;
				hl.Text = Utility.GetResourceString("ImageSizes", size.Name + ".Text") ?? (string.IsNullOrEmpty(size.Description) ? size.Name : size.Description);
				hl.Text += GetSizeText(size.Width, size.Height);
				hl.CssClass = "command";
				if (path == Selection.SelectedItem.Url)
					omSizes.Controls.AddAt(0, hl);
				else
					omSizes.Controls.Add(hl);
			}
			omSizes.Visible = omSizes.Controls.Count > 1;
		}

		private static string GetSizeText(int w, int h)
		{
			if (w == 0 && h == 0)
				return " &infin;";
			else if (w == 0)
				return string.Format(" &infin;x{0}", w);
			else if (h == 0)
				return string.Format(" {0}x&infin;", w);
			else
				return string.Format(" {0}x{1}", w, h);
		}

		protected override void RegisterToolbarSelection()
		{
			string script = GetToolbarSelectScript("preview");
			Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
		}

        protected File SelectedFile
        {
            get { return Selection.SelectedItem as File; }
        }

    	protected void OnDownloadCommand(object sender, CommandEventArgs e)
    	{
			Response.ContentType = "application/octet-stream";
			Response.AppendHeader("Content-disposition", "attachment; filename=" + SelectedFile.Name);
    		SelectedFile.TransmitTo(Response.OutputStream);
			Response.End();
		}

		protected void OnEditCommand(object sender, CommandEventArgs e)
		{
			txtContent.Text = SelectedFile.ReadFile();
			btnEdit.Visible = false;
			btnDownload.Visible = false;
			txtContent.Visible = true;
			btnSave.Visible = true;
			btnCancel.Visible = true;
		}

		protected void OnSaveCommand(object sender, CommandEventArgs e)
		{
			SelectedFile.WriteFile(txtContent.Text);
			OnCancelCommand(sender, e);
		}

		protected void OnCancelCommand(object sender, CommandEventArgs e)
		{
			btnEdit.Visible = true;
			btnDownload.Visible = true;
			txtContent.Visible = false;
			btnSave.Visible = false;
			btnCancel.Visible = false;
		}

		const double GB = 1024 * 1024 * 1024;
		const double MB = 1024 * 1024;
		const double kB = 1024;

		protected string GetFileSize(long size)
		{
			if (size > GB) return string.Format("{0:0.0} GB", size / GB);
			if (size > MB) return string.Format("{0:0.0} MB", size / MB);
			if (size > kB) return string.Format("{0:0.0} kB", size / kB);
			return string.Format("{0} B", size);
		}
    }
}
