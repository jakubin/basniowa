﻿using System;
using Common;
using Common.FileContainers;

namespace Website.Infrastructure.FileContainers
{
    /// <summary>
    /// Provides download URLs for file in physical file container, which are exposed as ASP.NET static files.
    /// </summary>
    /// <seealso cref="Website.Infrastructure.FileContainers.IFileContainerUrlProvider" />
    public class PhysicalStaticFileContainerUrlProvider : IFileContainerUrlProvider
    {
        private readonly string _baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalStaticFileContainerUrlProvider"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        public PhysicalStaticFileContainerUrlProvider(string baseUrl)
        {
            Guard.NotNull(baseUrl, nameof(baseUrl));

            _baseUrl = baseUrl.EndsWith("/")
                ? baseUrl
                : baseUrl + "/";
        }

        /// <inheritdoc/>
        public string GetDownloadUrl(string path)
        {
            Guard.NotNull(path, nameof(path));
            if (!FileContainerPath.IsValid(path))
            {
                throw new ArgumentException(
                    $"Path ({path}) is not valid for file container.",
                    nameof(path));
            }

            return _baseUrl + path;
        }
    }
}