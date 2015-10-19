using Microsoft.AspNet.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using Microsoft.Framework.Primitives;

namespace ModularAsp5_Beta8.Infrastructure
{
    public class CompositeFileProvider : IFileProvider
    {
        private readonly IEnumerable<IFileProvider> _fileProviders;

        public CompositeFileProvider(IEnumerable<IFileProvider> fileProviders)
        {
            _fileProviders = fileProviders;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            foreach (var provider in _fileProviders)
            {
                var contents = provider.GetDirectoryContents(subpath);
                if (contents != null && contents.Exists)
                {
                    return contents;
                }
            }

            return new NotFoundDirectoryContents();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            foreach (var provider in _fileProviders)
            {
                var fileInfo = provider.GetFileInfo(subpath);
                if (fileInfo != null && fileInfo.Exists)
                {
                    return fileInfo;
                }
            }

            return new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            foreach (var provider in _fileProviders)
            {
                var trigger = provider.Watch(filter);
                if (trigger != null)
                {
                    return trigger;
                }
            }
            return NoopToken.Singleton;
        }
    }

    // copied from FileSystem repo

    class NotFoundFileInfo : IFileInfo
    {
        private readonly string _name;

        public NotFoundFileInfo(string name)
        {
            _name = name;
        }

        public bool Exists
        {
            get { return false; }
        }

        public bool IsDirectory
        {
            get { return false; }
        }

        public DateTimeOffset LastModified
        {
            get { return DateTimeOffset.MinValue; }
        }

        public long Length
        {
            get { return -1; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string PhysicalPath
        {
            get { return null; }
        }

        public Stream CreateReadStream()
        {
            throw new FileNotFoundException(string.Format("The file {0} does not exist.", Name));
        }

        Stream IFileInfo.CreateReadStream()
        {
            throw new NotImplementedException();
        }
    }

    class NotFoundDirectoryContents : IDirectoryContents
    {
        public bool Exists
        {
            get { return false; }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return Enumerable.Empty<IFileInfo>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class NoopToken : IChangeToken
    {
        public static NoopToken Singleton { get; } = new NoopToken();

        public bool ActiveChangeCallbacks
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasChanged
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }
    }

    /*class NoopTrigger : IExpirationTrigger
    {
        public static NoopTrigger Singleton { get; } = new NoopTrigger();

        private NoopTrigger()
        {
        }

        public bool ActiveExpirationCallbacks
        {
            get { return false; }
        }

        public bool IsExpired
        {
            get { return false; }
        }

        public IDisposable RegisterExpirationCallback(Action<object> callback, object state)
        {
            throw new InvalidOperationException("Trigger does not support registering change notifications.");
        }
    }*/
}
