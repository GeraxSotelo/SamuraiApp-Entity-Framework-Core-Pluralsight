using System;
using System.Diagnostics;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using Xunit;

namespace VirtualFileSystem.Tests
{
    public class DatabaseTests
    {
        [Fact]
        public void CanInsertDirectoryIntoDatabase()
        {
            using (var context = new FileSystemContext())
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var dir = new Directory();
                context.Directories.Add(dir);
                Debug.WriteLine($"Before save: {dir.Id}");

                context.SaveChanges();
                Debug.WriteLine($"After save: {dir.Id}");

                Assert.NotEqual(0, dir.Id);
            }
        }
    }
}
