using DWOS.Shared.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Shared.Tests.Utilities
{
    [TestClass]
    public sealed class CircularBufferTests
    {
        [TestMethod]
        public void EnqueueTest()
        {
            var buffer = new CircularBuffer<string>(3);
            Assert.AreEqual(0, buffer.Length);

            buffer.Enqueue("test1");
            Assert.AreEqual(1, buffer.Length);

            buffer.Enqueue("test2");
            Assert.AreEqual(2, buffer.Length);

            buffer.Enqueue("test3");
            Assert.AreEqual(3, buffer.Length);

            buffer.Enqueue("test4");
            Assert.AreEqual(3, buffer.Length);
        }

        [TestMethod]
        public void ClearTest()
        {
            var buffer = new CircularBuffer<string>(3);

            buffer.Enqueue("test");
            buffer.Enqueue("test");
            buffer.Enqueue("test");

            Assert.AreEqual(3, buffer.Length);

            buffer.Clear();
            Assert.AreEqual(0, buffer.Length);
        }

        [TestMethod]
        public void DequeueTest()
        {
            var buffer = new CircularBuffer<string>(3);

            // Normal Case
            buffer.Enqueue("test1");
            buffer.Enqueue("test2");
            buffer.Enqueue("test3");

            Assert.AreEqual(3, buffer.Length);
            Assert.AreEqual("test1", buffer.Dequeue());
            Assert.AreEqual("test2", buffer.Dequeue());
            Assert.AreEqual("test3", buffer.Dequeue());
            Assert.AreEqual(0, buffer.Length);

            // Overwrite Case
            buffer.Enqueue("test1");
            buffer.Enqueue("test2");
            buffer.Enqueue("test3");
            buffer.Enqueue("test4");

            Assert.AreEqual(3, buffer.Length);
            Assert.AreEqual("test2", buffer.Dequeue());
            Assert.AreEqual("test3", buffer.Dequeue());
            Assert.AreEqual("test4", buffer.Dequeue());
            Assert.AreEqual(0, buffer.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DequeueExceptionTest()
        {
            var buffer = new CircularBuffer<string>(1);

            // Normal Case
            buffer.Enqueue("test1");

            try
            {
                buffer.Dequeue();
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("Dequeue should have worked.");
            }

            buffer.Dequeue();
        }

        [TestMethod]
        public void DequeueExceptionRecoveryTest()
        {
            var buffer = new CircularBuffer<string>(1);

            // Normal Case
            buffer.Enqueue("test1");
            buffer.Dequeue();

            var caughtException = false;
            try
            {
                buffer.Dequeue();
            }
            catch (InvalidOperationException)
            {
                caughtException = true;
            }

            Assert.IsTrue(caughtException);
            Assert.AreEqual(0, buffer.Length);
        }

        [TestMethod]
        public void PopTest()
        {
            var buffer = new CircularBuffer<string>(3);

            // Normal Case
            buffer.Enqueue("test1");
            buffer.Enqueue("test2");
            buffer.Enqueue("test3");

            Assert.AreEqual(3, buffer.Length);
            Assert.AreEqual("test3", buffer.Pop());
            Assert.AreEqual("test2", buffer.Pop());
            Assert.AreEqual("test1", buffer.Pop());
            Assert.AreEqual(0, buffer.Length);

            // Overwrite Case
            buffer.Enqueue("test1");
            buffer.Enqueue("test2");
            buffer.Enqueue("test3");
            buffer.Enqueue("test4");

            Assert.AreEqual(3, buffer.Length);
            Assert.AreEqual("test4", buffer.Pop());
            Assert.AreEqual("test3", buffer.Pop());
            Assert.AreEqual("test2", buffer.Pop());
            Assert.AreEqual(0, buffer.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PopExceptionTest()
        {
            var buffer = new CircularBuffer<string>(1);

            // Normal Case
            buffer.Enqueue("test1");

            try
            {
                buffer.Pop();
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("Pop should have worked.");
            }
            buffer.Pop();
        }

        [TestMethod]
        public void PopExceptionRecoveryTest()
        {
            var buffer = new CircularBuffer<string>(1);

            // Normal Case
            buffer.Enqueue("test1");
            buffer.Dequeue();

            var caughtException = false;
            try
            {
                buffer.Pop();
            }
            catch (InvalidOperationException)
            {
                caughtException = true;
            }

            Assert.IsTrue(caughtException);
            Assert.AreEqual(0, buffer.Length);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var buffer = new CircularBuffer<string>(3);

            // No elements
            CollectionAssert.AreEqual(new List<string>(), buffer.GetAll().ToList());

            // Only one element
            buffer.Enqueue("test1");
            CollectionAssert.AreEqual(new List<string> { "test1" }, buffer.GetAll().ToList());

            // Normal case
            buffer.Enqueue("test2");
            buffer.Enqueue("test3");
            CollectionAssert.AreEqual(new List<string> { "test1", "test2", "test3" }, buffer.GetAll().ToList());

            // Overflow
            buffer.Enqueue("test4");
            CollectionAssert.AreEqual(new List<string> { "test2", "test3", "test4" }, buffer.GetAll().ToList());

            buffer.Enqueue("test5");
            buffer.Enqueue("test6");
            CollectionAssert.AreEqual(new List<string> { "test4", "test5", "test6" }, buffer.GetAll().ToList());

            // After dequeue
            buffer.Dequeue();
            buffer.Dequeue();
            CollectionAssert.AreEqual(new List<string> { "test6" }, buffer.GetAll().ToList());

            // After pop
            buffer.Enqueue("test7");
            buffer.Enqueue("test8");
            buffer.Pop();
            CollectionAssert.AreEqual(new List<string> { "test6", "test7" }, buffer.GetAll().ToList());
        }
    }
}
