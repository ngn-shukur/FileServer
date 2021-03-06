﻿using System;
using System.Collections.Generic;
using System.IO;
using FileServer.Models;
using Microsoft.AspNetCore.Http;

namespace FileServer.Services
{
    public class FileSystemStorage
        : IFileStorage
    {
        readonly string directoryPath;

        public FileSystemStorage(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException(directoryPath);

            this.directoryPath = directoryPath;
        }

        public FileModel Add(string fileName, Stream stream)
        {
            var fullName = GetFullName(fileName);

            if (File.Exists(fullName))
                throw new ArgumentException(
                    $"The file {fileName} already exists in the file storage.");

            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }

            var fileInfo = new FileInfo(fullName);

            return new FileModel
            {
                Name = fileInfo.Name,
                Length = fileInfo.Length,
                LastModifiedDate = fileInfo.LastAccessTimeUtc
            };
        }

        public Stream Get(string fileName)
        {
            var filePath = GetFullName(fileName);

            if (!File.Exists(filePath)) return null;

            return File.OpenRead(filePath);
        }

        public IEnumerable<FileModel> GetAll()
        {
            var directory = new DirectoryInfo(directoryPath);

            foreach (var fileInfo in directory.GetFiles())
                yield return new FileModel
                {
                    Name = fileInfo.Name,
                    Length = fileInfo.Length,
                    LastModifiedDate = fileInfo.LastAccessTimeUtc
                };
        }

        public FileModel Update(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public FileModel Remove(string fileName)
        {
            var filePath = GetFullName(fileName);

            if (!File.Exists(filePath)) return null;

            var fileInfo = new FileInfo(filePath);

            File.Delete(filePath);

            return new FileModel {Name = fileInfo.Name};
        }

        string GetFullName(string fileName)
        {
            return Path.Combine(directoryPath, fileName);
        }
    }
}