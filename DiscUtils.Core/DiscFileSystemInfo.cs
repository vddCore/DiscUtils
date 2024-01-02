using System;
using System.IO;
using DiscUtils.Core.Internal;

namespace DiscUtils.Core
{
    /// <summary>
    /// Provides the base class for both <see cref="DiscFileInfo"/> and <see cref="DiscDirectoryInfo"/> objects.
    /// </summary>
    public class DiscFileSystemInfo
    {
        internal DiscFileSystemInfo(DiscFileSystem fileSystem, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            FileSystem = fileSystem;
            Path = path.Trim('/');
        }

        /// <summary>
        /// Gets or sets the <see cref="FileAttributes"/> of the current <see cref="DiscFileSystemInfo"/> object.
        /// </summary>
        public virtual FileAttributes Attributes
        {
            get => FileSystem.GetAttributes(Path);
            set => FileSystem.SetAttributes(Path, value);
        }

        /// <summary>
        /// Gets or sets the creation time (in local time) of the current <see cref="DiscFileSystemInfo"/> object.
        /// </summary>
        public virtual DateTime CreationTime
        {
            get => CreationTimeUtc.ToLocalTime();
            set => CreationTimeUtc = value.ToUniversalTime();
        }

        /// <summary>
        /// Gets or sets the creation time (in UTC) of the current <see cref="DiscFileSystemInfo"/> object.
        /// </summary>
        public virtual DateTime CreationTimeUtc
        {
            get => FileSystem.GetCreationTimeUtc(Path);
            set => FileSystem.SetCreationTimeUtc(Path, value);
        }

        /// <summary>
        /// Gets a value indicating whether the file system object exists.
        /// </summary>
        public virtual bool Exists => FileSystem.Exists(Path);

        /// <summary>
        /// Gets the extension part of the file or directory name.
        /// </summary>
        public virtual string Extension
        {
            get
            {
                string name = Name;
                int sepIdx = name.LastIndexOf('.');
                if (sepIdx >= 0)
                {
                    return name.Substring(sepIdx + 1);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the file system the referenced file or directory exists on.
        /// </summary>
        public DiscFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the full path of the file or directory.
        /// </summary>
        public virtual string FullName => Path;

        /// <summary>
        /// Gets or sets the last time (in local time) the file or directory was accessed.
        /// </summary>
        /// <remarks>Read-only file systems will never update this value, it will remain at a fixed value.</remarks>
        public virtual DateTime LastAccessTime
        {
            get => LastAccessTimeUtc.ToLocalTime();
            set => LastAccessTimeUtc = value.ToUniversalTime();
        }

        /// <summary>
        /// Gets or sets the last time (in UTC) the file or directory was accessed.
        /// </summary>
        /// <remarks>Read-only file systems will never update this value, it will remain at a fixed value.</remarks>
        public virtual DateTime LastAccessTimeUtc
        {
            get => FileSystem.GetLastAccessTimeUtc(Path);
            set => FileSystem.SetLastAccessTimeUtc(Path, value);
        }

        /// <summary>
        /// Gets or sets the last time (in local time) the file or directory was written to.
        /// </summary>
        public virtual DateTime LastWriteTime
        {
            get => LastWriteTimeUtc.ToLocalTime();
            set => LastWriteTimeUtc = value.ToUniversalTime();
        }

        /// <summary>
        /// Gets or sets the last time (in UTC) the file or directory was written to.
        /// </summary>
        public virtual DateTime LastWriteTimeUtc
        {
            get => FileSystem.GetLastWriteTimeUtc(Path);
            set => FileSystem.SetLastWriteTimeUtc(Path, value);
        }

        /// <summary>
        /// Gets the name of the file or directory.
        /// </summary>
        public virtual string Name => Utilities.GetFileFromPath(Path);

        /// <summary>
        /// Gets the <see cref="DiscDirectoryInfo"/> of the directory containing the current <see cref="DiscFileSystemInfo"/> object.
        /// </summary>
        public virtual DiscDirectoryInfo Parent
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return null;
                }

                return new DiscDirectoryInfo(FileSystem, Utilities.GetDirectoryFromPath(Path));
            }
        }

        /// <summary>
        /// Gets the path to the referenced file.
        /// </summary>
        protected string Path { get; }

        /// <summary>
        /// Deletes a file or directory.
        /// </summary>
        public virtual void Delete()
        {
            if ((Attributes & FileAttributes.Directory) != 0)
            {
                FileSystem.DeleteDirectory(Path);
            }
            else
            {
                FileSystem.DeleteFile(Path);
            }
        }

        /// <summary>
        /// Indicates if <paramref name="obj"/> is equivalent to this object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is equivalent, else <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            DiscFileSystemInfo asInfo = obj as DiscFileSystemInfo;
            if (obj == null)
            {
                return false;
            }

            return string.Compare(Path, asInfo.Path, StringComparison.Ordinal) == 0 &&
                   Equals(FileSystem, asInfo.FileSystem);
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Path.GetHashCode() ^ FileSystem.GetHashCode();
        }
    }
}