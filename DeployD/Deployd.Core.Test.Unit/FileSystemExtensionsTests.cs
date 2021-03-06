﻿using System.IO.Abstractions;
using Moq;
using NUnit.Framework;

namespace Deployd.Core.Test.Unit
{
    [TestFixture]
    public class FileSystemExtensionsTests
    {
        private Mock<IFileSystem> _fs;
        private string _path;

        [SetUp]
        public void SetUp()
        {
            _fs = new Mock<IFileSystem>();
            _path = "path";
        }

        [Test]
        public void EnsureDirectoryExists_DirectoryExists_Returns()
        {
            _fs.Setup(x => x.Directory.Exists(_path)).Returns(true);

            _fs.Object.EnsureDirectoryExists(_path);

            _fs.Verify(x=>x.Directory.CreateDirectory(_path), Times.Never());
        }

        [Test]
        public void EnsureDirectoryExists_DirectoryDoesNotExist_CreatesDirectory()
        {
            _fs.Setup(x => x.Directory.Exists(_path)).Returns(false);

            _fs.Object.EnsureDirectoryExists(_path);

            _fs.Verify(x=>x.Directory.CreateDirectory(_path), Times.Once());
        }

        [Test]
        public void MapVirtualPath_WhenPathContainsVirtualPart_ReplacesWithCurrentDirectory()
        {
            _path = "~\\something";
            _fs.Setup(x => x.Directory.GetCurrentDirectory()).Returns("stub:\\directory");

            var result = _fs.Object.MapVirtualPath(_path);

            Assert.That(result, Is.EqualTo("stub:\\directory\\something"));
        }

        [Test]
        public void MapVirtualPath_WhenPathDoesNotContainsVirtualPart_ReturnedUnchanged()
        {
            _path = "something";

            var result = _fs.Object.MapVirtualPath(_path);

            Assert.That(result, Is.EqualTo("something"));
        }
    }
}
