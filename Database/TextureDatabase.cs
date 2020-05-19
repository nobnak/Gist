using UnityEngine;

namespace nobnak.Gist.Database {

	public class TextureRow : Row {
		public readonly Texture2D tex;
		public readonly int groupId;

		public TextureRow(
			Texture2D tex,
			int groupId = -1) {
			this.tex = tex;
			this.groupId = groupId;
		}

		#region interface
		
		#endregion
	}

	public class TextureDatabase : Database<TextureRow> { 

		#region interface
		#endregion
	}
}