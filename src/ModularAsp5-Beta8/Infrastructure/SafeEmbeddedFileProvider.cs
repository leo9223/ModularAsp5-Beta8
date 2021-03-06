﻿using Microsoft.AspNet.FileProviders;
using Microsoft.Framework.Primitives;
using System.Reflection;


namespace ModularAsp5_Beta8.Infrastructure
{
    public class SafeEmbeddedFileProvider : IFileProvider
    {
        // unfortunately (as of beta3) the EmbeddedFileProvider throws an ArgumentException (from Assembly.GetManifestResourceInfo) when:
        //  - baseNamespace is ""
        //  - subpath is "" or "/"
        // This class works around that for now :-)

        private readonly string _baseNamespace;
        private readonly EmbeddedFileProvider _inner;

        public SafeEmbeddedFileProvider(Assembly assembly)
        {
            _baseNamespace = "";
            _inner = new EmbeddedFileProvider(assembly);
        }
        public SafeEmbeddedFileProvider(Assembly assembly, string baseNamespace)
        {
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (_baseNamespace == "" && (subpath == "" || subpath == "/"))
            {
                return null;
            }
            return _inner.GetFileInfo(subpath);
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _inner.GetDirectoryContents(subpath);
        }

        public IChangeToken Watch(string pattern)
        {
            return _inner.Watch(pattern);
        }


    }
}
