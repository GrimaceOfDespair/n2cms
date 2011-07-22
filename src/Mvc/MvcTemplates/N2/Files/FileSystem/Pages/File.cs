﻿using System.IO;
using System.Text;
using System.Web;
using N2.Engine;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Definitions;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("File",
        IconUrl = "{ManagementUrl}/Resources/icons/page_white.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		SortOrder = 2010)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
	[N2.Web.Template("info", "{ManagementUrl}/Files/FileSystem/File.aspx")]
    public class File : AbstractNode, IActiveContent, IInjectable<IEditUrlManager>, IFileSystemFile
    {
		IEditUrlManager managementPaths;

		protected IEditUrlManager ManagementPaths
		{
			get { return managementPaths ?? Context.Current.ManagementPaths; }
		}

		public File()
		{
		}

		public File(FileData file, AbstractDirectory parent)
		{
			Parent = parent;

			NewName = file.Name;
			Name = file.Name;
			Title = file.Name;
			Size = file.Length;
			Updated = file.Updated;
			Created = file.Created;

			url = file.VirtualPath;
		}

		public long Size { get; set; }
		public bool IsIcon { get; set; }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent != null)
                MoveTo(newParent);
        }

		string url;
		public override string Url
		{
			get { return url ?? N2.Web.Url.Combine(Parent.Url, Name); }
		}

		public override bool IsPage
		{
			get { return !IsIcon; }
		}

		public override string IconUrl
		{
			get
			{
				if(iconUrl != null)
					return base.IconUrl;

				string extension = VirtualPathUtility.GetExtension(Name).ToLower();
				switch (extension)
				{
					case ".gif":
					case ".png":
					case ".jpg":
					case ".tif":
					case ".tiff":
					case ".jpeg":
						return IconPath("page_white_picture");
					case ".pdf":
						return IconPath("page_white_acrobat");
					case ".c":
					case ".cpp":
					case ".class":
					case ".java":
					case ".cs":
					case ".vb":
					case ".js":
					case ".dtd":
					case ".m":
					case ".pl":
					case ".py":
						return IconPath("page_white_csharp");
					case ".html":
					case ".htm":
					case ".xml":
					case ".xsd":
					case ".xslt":
					case ".aspx":
					case ".ascx":
					case ".ashx":
					case ".php":
					case ".css":
					case ".sql":
					case ".fla":
						return IconPath("page_white_code");
					case ".zip":
					case ".gz":
					case ".7z":
					case ".rar":
					case ".sit":
					case ".tar":
					case ".pkg":
					case ".msi":
					case ".cab":
						return IconPath("page_white_compressed");
					case ".swf":
						return IconPath("page_white_flash");
					case ".txt":
					case ".log":
						return IconPath("page_white_text");
					case ".csv":
					case ".xls":
					case ".xlsx":
					case ".wks":
						return IconPath("page_white_excel");
					case ".xps":
					case ".ppt":
					case ".pptx":
					case ".pps":
						return IconPath("page_white_powerpoint");
					case ".rtf":
					case ".doc":
					case ".docx":
						return IconPath("page_white_word");
					case ".mpg":
					case ".mpeg":
					case ".avi":
					case ".wmv":
					case ".flv":
					case ".mp4":
					case ".mov":

					case ".3g2":
					case ".3gp":
					case ".asf":
					case ".asx":
					case ".rm":
					case ".vob":

					case ".aif":
					case ".iff":
					case ".m3u":
					case ".m4a":
					case ".mid":
					case ".mp3":
					case ".mpa":
					case ".ra":
					case ".wav":
					case ".wma":
						return IconPath("page_white_dvd");
					default:
						return IconPath("page_white");
				}
			}
		}

		private string IconPath(string iconName)
		{
			return ManagementPaths.ResolveResourceUrl(string.Format("{{ManagementUrl}}/Resources/icons/{0}.png", iconName));
		}

		public bool Exists
		{
			get { return Url != null && FileSystem.FileExists(Url); }
		}

    	public string NewName { get; set; }

		public virtual void TransmitTo(Stream stream)
		{
			FileSystem.ReadFileContents(Url, stream);
		}

		public void WriteToDisk(Stream stream)
		{
			if (!FileSystem.DirectoryExists(Directory.Url))
				FileSystem.CreateDirectory(Directory.Url);

			FileSystem.WriteFile(Url, stream);
		}

		internal void Add(File file)
		{
			Children.Add(file);
		}

		#region IActiveContent Members

		public void Save()
        {
			if (!string.IsNullOrEmpty(NewName))
			{
				FileSystem.MoveFile(Url, Combine(Directory.Url, NewName));
				Name = NewName;
				InvalidateUrl();
			}
        }

        public void Delete()
        {
			FileSystem.DeleteFile(Url);
			InvalidateUrl();
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.MoveFile(Url, to);
        	Parent = d;
			InvalidateUrl();
        }

		public ContentItem CopyTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.CopyFile(Url, to);

			return d.GetChild(Name);
        }

        #endregion

		public string ReadFile()
		{
			using (var fs = FileSystem.OpenFile(Url))
			using (var sr = new StreamReader(fs))
			{
				return sr.ReadToEnd();
			}
		}

		public void WriteFile(string text)
		{
			using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(text)))
			{
				FileSystem.WriteFile(Url, ms);
			}
		}

		protected void InvalidateUrl()
		{
			this.url = null;
		}

		#region IDependentEntity<IEditUrlManager> Members

		public void Set(IEditUrlManager dependency)
		{
			managementPaths = dependency;
		}

		#endregion
	}
}
