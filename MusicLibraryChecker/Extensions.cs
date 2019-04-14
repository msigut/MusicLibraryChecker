using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicLibraryChecker
{
    public static class Extensions
    {
		#region SizeSuffix

		private static readonly string[] _sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(this long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                _sizeSuffixes[mag]);
        }

		#endregion

		#region Directory

		/// <summary>
		/// music file extensions
		/// </summary>
		public static readonly string[] MUSIC_EXTS = new[] { ".flac", ".mp3" };
		/// <summary>
		/// ID3 tags file extensions
		/// </summary>
		public static readonly string[] TAG_EXTS = new[] { ".flac" };

		/// <summary>
		/// finds all directories contains any of given file extensions
		/// </summary>
		internal static IReadOnlyCollection<DirectoryInfo> DirectoriesContainsExtensions(this IReadOnlyCollection<string> paths, bool recursive, string[] findExtensions = null)
		{
			var result = new List<DirectoryInfo>();

			// default extensions
			if (findExtensions == null)
			{
				findExtensions = MUSIC_EXTS;
			}

			foreach (var path in paths)
			{
				// directory info
				var pathInfo = new DirectoryInfo(path);
				if (!pathInfo.Exists)
				{
					Console.WriteLine($"ERR {path} doesn't exist.");
					continue;
				}

				var temp = pathInfo.GetDirectories("*", (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

				// only directories contains any of given extensions
				result.AddRange(temp.Where(x => x.GetFiles().Any(y => findExtensions.Contains(y.Extension.ToLowerInvariant()))));
			}

			return result.OrderBy(x => x.FullName).ToList();
		}

		/// <summary>
		/// finds all files with given extensions
		/// </summary>
		internal static IReadOnlyCollection<FileInfo> FilesWithExtensions(this DirectoryInfo directory, string[] findExtensions = null)
		{
			// default extensions
			if (findExtensions == null)
			{
				findExtensions = MUSIC_EXTS;
			}

			return directory.GetFiles().Where(x => findExtensions.Contains(x.Extension.ToLowerInvariant())).ToList();
		}

		#endregion
	}
}
