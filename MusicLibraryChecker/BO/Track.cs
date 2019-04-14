using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibraryChecker.BO
{
	public class Track
	{
		public string FullName { get; set; }

		public string Title { get; set; }
		public string AlbumArtist { get; set; }
		public string Composer { get; set; }
		public string Conductor { get; set; }
		public string Album { get; set; }
		public string Comment { get; set; }
		public string Copyright { get; set; }
		public string Gengre { get; set; }
		public string Date { get; set; }
		public short? TrackNumber { get; set; }
		public short? TrackTotal { get; set; }
		public short? DiskNumber { get; set; }
		public short? DiskTotal { get; set; }

		#region Seller, Barcode, Lyrics, DR

		public string Seller { get; set; }
		public string Barcode { get; set; }
		public string Lyrics { get; set; }
		public short? AlbumDynamicRange { get; set; }

		#endregion

		#region SampleRate, BitsPerSample, Channels, Duration

		public int? SampleRateHz { get; set; }
		public short? BitsPerSample { get; set; }
		public short? Channels { get; set; }
		public int? Duration { get; set; }

		#endregion

		#region Cover URL, size, MIME

		public string CoverURL { get; set; }
		public string CoverMIME { get; set; }
		public short? CoverWidth { get; set; }
		public short? CoverHeight { get; set; }

		#endregion

		#region MusicBrainz

		public string MusicBrainzAlbumURL { get; set; }
		public string MusicBrainzArtistURL { get; set; }
		public string MusicBrainzAlbumArtistURL { get; set; }
		public string MusicBrainzTrackID { get; set; }
		public string MusicBrainzAlbumID { get; set; }
		public string MusicBrainzArtistID { get; set; }
		public string MusicBrainzAlbumArtistID { get; set; }

		#endregion

		#region Constructor

		public Track()
		{
		}

		public Track(string fullName) : this()
		{
			FullName = fullName;
		}

		#endregion
	}
}
