#region

using EvilTemple.Rules;
using NUnit.Framework;

#endregion

namespace Rules.Tests
{
    public class BaseObjectTest
    {
        [Test]
        public void TestParentChild()
        {
            // Check initial state
            var parent = new BaseObject();
            Assert.IsTrue(parent.Content.IsReadOnly);
            Assert.AreEqual(0, parent.Content.Count);

            // Set the parent, check that the content changed
            var child = new BaseObject {Parent = parent};
            Assert.AreSame(parent, child.Parent);
            Assert.AreEqual(1, parent.Content.Count);
            Assert.AreSame(child, parent.Content[0]);

            // Unset the parent again, check that the content changed
            child.Parent = null;
            Assert.IsNull(child.Parent);
            Assert.AreEqual(0, parent.Content.Count);

            // Add again using the method on the parent
            parent.AddItem(child);
            Assert.AreSame(parent, child.Parent);
            Assert.AreEqual(1, parent.Content.Count);
            Assert.AreSame(child, parent.Content[0]);

            // Remove again using the parent's method
            parent.RemoveItem(child);
            Assert.IsNull(child.Parent);
            Assert.AreEqual(0, parent.Content.Count);
        }
    }
}