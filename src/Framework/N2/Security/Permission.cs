using System;

namespace N2.Security
{
	[Flags]
	public enum Permission
	{
		None = 0,
		Read = 1,
		Write = 2,
		Publish = 4,
		Administer = 8,
    AddTo = 16,
    DeleteFrom = 32,
		ReadWrite = Read | Write,
		ReadWritePublish = Read | Write | Publish,
		Full = Read | Write | Publish | Administer | AddTo | DeleteFrom
	}
}
