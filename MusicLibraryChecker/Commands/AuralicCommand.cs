using FlacLibSharp;
using FlacLibSharp.Exceptions;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicLibraryChecker.Commands
{
    internal class AuralicCommand
    {
        public static void Execute(IReadOnlyCollection<string> paths, bool recursive, bool fix, int minWidth = 500, int minHeight = 500, bool bookletWarning = false)
        {
			// only directories with music content
			var dirs = paths.DirectoriesContainsExtensions(recursive);
			foreach (var dir in dirs)
			{
				// icon files (from Mac)
				var icons = dir.GetFiles("Icon*", SearchOption.TopDirectoryOnly);
				foreach (var icon in icons.Where(x => x.Length == 0))
				{
					Console.WriteLine($"+0ICON   {icon.FullName}");

					if (fix)
					{
						File.Delete(icon.FullName);
					}
				}

				// hidden Linux files
				var hiddens = dir.GetFiles(".*", SearchOption.TopDirectoryOnly);
				foreach (var hidden in hiddens.Where(x => x.Length <= (10 * 1024)))
				{
					Console.WriteLine($"+HIDDEN  {hidden.FullName}");

					if (fix)
					{
						File.Delete(hidden.FullName);
					}
				}

				// FLAC files
				var flacs = dir.GetFiles("*.flac", SearchOption.TopDirectoryOnly);

				// missing folder.jpg|png file
				var folderImages = dir.GetFiles("folder.*", SearchOption.TopDirectoryOnly);
				var folderImagesCount = folderImages.Count(x => x.Extension.ToLowerInvariant() == ".png" || x.Extension.ToLowerInvariant() == ".jpg");
				// folder image is missing, try find other
				if (folderImagesCount < 1)
				{
					// folder.jpg file
					var folderImage = Path.Combine(dir.FullName, "folder.jpg");

					// cover file
					var coverFiles = dir.GetFiles("cover.jpg", SearchOption.TopDirectoryOnly);
					if (coverFiles.Any())
					{
						// fix: rename: cover -> folder.jpg
						var folderCover = coverFiles.First().FullName;
						Console.WriteLine($"+PICTURE {dir.FullName}: {Path.GetFileName(folderCover)} -> {Path.GetFileName(folderImage)}");

						if (fix)
						{
							File.Move(folderCover, folderImage);
						}
					}
					// try get folder.jpg from first FLAC file
					else if (flacs.Any())
					{

						try
						{
							using (var file = new FlacFile(flacs.First().FullName))
							{
								var picture = file.GetAllPictures().FirstOrDefault();
								if (picture != null)
								{
									Console.WriteLine($"+PICTURE {dir.FullName}: FLAC MIME: {picture.MIMEType} -> {Path.GetFileName(folderImage)}");

									if (fix)
									{
										using (var input = new MemoryStream(picture.Data))
										using (var output = File.OpenWrite(folderImage))
										{
											var f = Image.Load(input);
											f.SaveAsJpeg(output);
										}
									}
								}
							}
						}
						catch (FlacLibSharpInvalidFormatException)
						{
						}
					}
					else
					{
						Console.WriteLine($"_PICTURE {dir.FullName}: folder.jpg is missing");
					}
				}

				// folder.jpg sizes
				folderImages = dir.GetFiles("folder.*", SearchOption.TopDirectoryOnly).Where(x => x.Extension.ToLowerInvariant() == ".jpg" || x.Extension.ToLowerInvariant() == ".png").ToArray();
				if (folderImages.Any())
				{
					var fi = new FileInfo(folderImages.First().FullName);
					var f = Image.Load(folderImages.First().FullName);

					if (f.Width < minWidth || f.Height < minHeight)
					{
						Console.WriteLine($"!PICTURE {folderImages.First().FullName}: [{f.Width}x{f.Height}] {fi.Length.SizeSuffix()}");
					}
				}

				// exist Art
				var folderArts = dir.GetDirectories("art*", SearchOption.TopDirectoryOnly);
				if (folderArts.Any())
				{
					var folderArt = folderArts.First();

					var files = folderArt.GetFiles();
					foreach (var file in files)
					{
						Console.WriteLine($"!ART     {file.FullName} {file.Length.SizeSuffix()}");

						if (fix)
						{
							if (file.Name.ToLowerInvariant() == "thumbs.db")
							{
								try
								{
									File.Delete(file.FullName);
								}
								catch (IOException ex)
								{
									Console.WriteLine($"!ERROR   {ex.Message}");
								}
							}
						}
					}

					if (fix)
					{
						if (folderArt.GetFiles().Length == 0)
						{
							Directory.Delete(folderArt.FullName);
						}
					}
				}

				// booklet.pdf file
				var bookletFile = Path.Combine(dir.FullName, "booklet.pdf");
				// booklet
				var booklets = dir.GetFiles("*.pdf", SearchOption.AllDirectories);
				if (booklets.Length >= 2)
				{
					foreach (var booklet in booklets)
					{
						var fi = new FileInfo(booklet.FullName);
						Console.WriteLine($"+BOOKLET {booklet.FullName}: {fi.Length.SizeSuffix()}");
					}
				}
				else if (booklets.Length == 1)
				{
					var booklet = booklets.First();

					if (fix)
					{
						if (booklet.Directory.FullName.ToLowerInvariant() != dir.FullName.ToLowerInvariant())
						{
							Console.WriteLine($"+BOOKLET {booklet.FullName} -> {Path.GetFileName(bookletFile)}");
							File.Move(booklet.FullName, bookletFile);

							var files = booklet.Directory.GetFiles();
							if (files.Length == 0)
							{
								Directory.Delete(booklet.DirectoryName);
							}
							else if (files.Length >= 1)
							{
								foreach (var file in files)
								{
									Console.WriteLine($"!BOOKLET {file.FullName}");
								}
							}
						}
					}
				}
				else
				{
					if (bookletWarning)
					{
						Console.WriteLine($"!BOOKLET {bookletFile} not found");
					}
				}
			}
        }
	}
}
