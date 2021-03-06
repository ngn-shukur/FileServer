﻿using System.Collections.Generic;
using FileServer.Utilites;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FileServer.Extensions
{
    public static class MultipartReaderExtentions
    {
        public static IEnumerable<MultipartSection>
            ReadFileContent(this MultipartReader reader)
        {
            var section = reader.ReadNextSectionAsync().Result;
            while (section != null)
            {
                if (ContentDispositionHeaderValue
                    .TryParse(section.ContentDisposition,
                        out var contentDisposition))
                    if (MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                        yield return section;
                // TODO: Invalid opration exception when already read
                section = reader.ReadNextSectionAsync().Result;
            }
        }
    }
}