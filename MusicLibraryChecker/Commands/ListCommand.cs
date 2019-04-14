using System;
using System.Collections.Generic;
using System.Linq;
using FlacLibSharp;
using FlacLibSharp.Exceptions;
using MusicLibraryChecker.BO;

namespace MusicLibraryChecker.Commands
{
	internal class ListCommand
    {
		public static void Execute(IReadOnlyCollection<string> paths, bool recursive)
		{
			// only directories with music content
			var dirs = paths.DirectoriesContainsExtensions(recursive, Extensions.TAG_EXTS);
			foreach (var dir in dirs)
			{
				var album = new Album();

				var files = dir.FilesWithExtensions(Extensions.TAG_EXTS).Take(1);
				foreach (var file in files)
				{
					var track = new Track(file.FullName);

					try
					{
						using (var flac = new FlacFile(file.FullName))
						{
							if (flac.VorbisComment != null)
							{
								P(track, flac.VorbisComment.Title, (t, s) => t.Title = s);
								P(track, flac.VorbisComment.Artist, (t, s) => t.AlbumArtist = s);
								P(track, flac.VorbisComment.Album, (t, s) => t.Album = s);
								P(track, flac.VorbisComment.Genre, (t, s) => t.Gengre = s);
								P(track, flac.VorbisComment.Date, (t, s) => t.Date = s);
							}

							P(track, flac, "COMPOSER", (t, s) => t.Composer = s);
							P(track, flac, "CONDUCTOR", (t, s) => t.Conductor = s);
							P(track, flac, "COMMENT", (t, s) => t.Comment = s);
							P(track, flac, "COPYRIGHT", (t, s) => t.Copyright = s);
							PS(track, flac, "TRACKNUMBER", (t, n) => t.TrackNumber = n);
							PS(track, flac, "TRACKTOTAL", (t, n) => t.TrackTotal = n);
							PS(track, flac, "DISCNUMBER", (t, n) => t.DiskNumber = n);
							PS(track, flac, "DISCTOTAL", (t, n) => t.DiskTotal = n);
							P(track, flac, "SELLER", (t, s) => t.Seller = s);
							P(track, flac, "BARCODE", (t, s) => t.Barcode = s);
							PS(track, flac, "ALBUM DYNAMIC RANGE", (t, s) => t.AlbumDynamicRange = s);
							P(track, flac, "LYRICS", (t, s) => t.Lyrics = s);

							if (flac.StreamInfo != null)
							{
								track.SampleRateHz = (int)flac.StreamInfo.SampleRateHz;
								track.BitsPerSample = flac.StreamInfo.BitsPerSample;
								track.Channels = flac.StreamInfo.Channels;
								track.Duration = flac.StreamInfo.Duration;
							}
						}
					}
					catch (FlacLibSharpInvalidFormatException)
					{
					}

					album.Tracks.Add(track);
				}

				if (album.Tracks.Any())
				{
					var track = album.Tracks.First();

					if (string.IsNullOrEmpty(track.Album))
					{
						Console.WriteLine($"[empty] {dir.FullName} ({track.FullName})");
					}
					else
					{
						var tech = $"[{(track.SampleRateHz / 1000),3}/{track.BitsPerSample}/{(track.AlbumDynamicRange.HasValue ? track.AlbumDynamicRange.ToString() : "-"),2}]";
						var seller = string.IsNullOrEmpty(track.Seller) ? "" : $" (seller: {track.Seller})";
						Console.WriteLine($"{track.Gengre,20} {tech} {track.Album}: {track.AlbumArtist}{seller}");
					}
				}
			}
		}

		#region Helpers

		/// <summary>
		/// parsing data for Tags
		/// </summary>
		private static void P(Track t, VorbisCommentValues value, Action<Track, string> todo)
		{
			if (value != null && !string.IsNullOrEmpty(value.Value))
				todo(t, value.Value);
		}
		private static void P(Track t, FlacFile file, string key, Action<Track, string> todo)
		{
			var item = file.VorbisComment[key];
			if (item != null && !string.IsNullOrEmpty(item.Value))
				todo(t, item.Value);
		}
		private static void PS(Track t, FlacFile file, string key, Action<Track, short> todo)
		{
			var item = file.VorbisComment[key];
			if (item != null && !string.IsNullOrEmpty(item.Value))
			{
				if (short.TryParse(item.Value, out var num))
					todo(t, num);
			}
		}

		#endregion
	}
}
