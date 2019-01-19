using System;
using System.Linq;
using osu.Framework.Design.Tests.Helpers;
using osu.Framework.Design.Workspaces;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class WorkspaceTests
    {
        [Fact]
        public void TestCreateDocument()
        {
            //Given
            var content = "Test document content";

            using (var space = new MockWorkspace())
            {
                //When
                var doc = space.CreateDocument("MyClass.cs");

                using (var writer = doc.OpenWriter())
                    writer.Write(content);

                //Then
                doc = space.GetDocument("MyClass.cs");

                using (var reader = doc.OpenReader())
                    Assert.Equal(content, reader.ReadToEnd());
            }
        }

        [Fact]
        public void TestDeleteDocument()
        {
            //Given
            using (var space = new MockWorkspace())
            {
                var doc = space.CreateDocument("MyClass.cs");

                //When
                space.DeleteDocument(doc.FullName);

                //Then
                Assert.False(doc.Exists);
                Assert.Throws<InvalidOperationException>(() => doc.OpenRead());
            }
        }

        [Fact]
        public void TestDocumentAddedEvent()
        {
            //Given
            var addedItem = (Document)null;

            using (var space = new MockWorkspace())
            {
                space.Documents.ItemsAdded += d => addedItem = d.Single();

                //When
                space.CreateDocument("MyClass.cs");

                //Then
                Assert.Equal("MyClass.cs", addedItem.Name);
            }
        }

        [Fact]
        public void TestDocumentRemovedEvent()
        {
            //Given
            var removedItem = (Document)null;

            using (var space = new MockWorkspace())
            {
                space.CreateDocument("MyClass.cs");
                space.Documents.ItemsRemoved += d => removedItem = d.Single();

                //When
                space.DeleteDocument("MyClass.cs");

                //Then
                Assert.Equal("MyClass.cs", removedItem.Name);
            }
        }
    }
}