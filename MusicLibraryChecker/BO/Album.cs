using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibraryChecker.BO
{
    public class Album
    {
		public string Title { get; set; }
		public string AlbumArtist { get; set; }
		public string Comment { get; set; }
		public string Gengre { get; set; }
		public string Date { get; set; }
		public short? DiskTotal { get; set; }

		public string Seller { get; set; }
		public short? AlbumDynamicRange { get; set; }

		public int? SampleRateHz { get; set; }
		public short? BitsPerSample { get; set; }

		public short? CoverWidth { get; set; }
		public short? CoverHeight { get; set; }

		public List<Track> Tracks { get; private set; }

		#region Constructor

		public Album()
		{
			Tracks = new List<Track>();
		}

		#endregion
	}
}
