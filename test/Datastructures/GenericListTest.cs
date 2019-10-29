namespace RJCP.Core.Datastructures
{
    using System;
    using System.Collections;
    using NUnit.Framework;

    [TestFixture(Category = "Collections.GenericList")]
    public class GenericListTest
    {
        [Test]
        public void SimpleInit()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            Assert.That(stringList.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddWrapper_Generic()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList) {
                "Test"
            };
            Assert.That(stringList.Count, Is.EqualTo(1));
            Assert.That(myList.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddWrapper_List()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            Assert.That(stringList.Count, Is.EqualTo(1));
            Assert.That(myList.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddWrapper_FixedSize()
        {
            object[] myList = new object[5];
            GenericList<string> stringList = new GenericList<string>(myList);

            myList[0] = "Test";
            myList[1] = "Test2";
            Assert.That(() => { stringList.Add("Test3"); }, Throws.TypeOf<NotSupportedException>());

            Assert.That(myList[0], Is.EqualTo("Test"));
            Assert.That(myList[1], Is.EqualTo("Test2"));
        }

        [Test]
        public void RemoveWrapper_Generic()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            Assert.That(stringList.Remove("Test"), Is.True);
            Assert.That(stringList.Count, Is.EqualTo(0));
            Assert.That(myList.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveWrapper_List()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Remove("Test");
            Assert.That(stringList.Count, Is.EqualTo(0));
            Assert.That(myList.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveWrapper_FixedSize()
        {
            object[] myList = new object[5];
            GenericList<string> stringList = new GenericList<string>(myList);

            myList[0] = "Test";
            myList[1] = "Test2";
            Assert.That(() => { stringList.Remove("Test"); }, Throws.TypeOf<NotSupportedException>());

            Assert.That(myList[0], Is.EqualTo("Test"));
            Assert.That(myList[1], Is.EqualTo("Test2"));
        }

        [Test]
        public void RemoveWrapperNotFound_Generic()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            Assert.That(stringList.Remove("Test2"), Is.False);
            Assert.That(stringList.Count, Is.EqualTo(1));
            Assert.That(myList.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveWrapperNotFound_List()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Remove("Test2");
            Assert.That(stringList.Count, Is.EqualTo(1));
            Assert.That(myList.Count, Is.EqualTo(1));
        }

        [Test]
        public void Insert_Generic()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Add("Test2");
            stringList.Insert(1, "Test1");
            Assert.That(stringList.Count, Is.EqualTo(3));
            Assert.That(myList.Count, Is.EqualTo(3));

            Assert.That(myList[0], Is.EqualTo("Test"));
            Assert.That(myList[1], Is.EqualTo("Test1"));
            Assert.That(myList[2], Is.EqualTo("Test2"));
        }

        [Test]
        public void Insert_Null()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Add("Test2");
            stringList.Insert(1, null);

            Assert.That(myList[0], Is.EqualTo("Test"));
            Assert.That(myList[1], Is.Null);
            Assert.That(myList[2], Is.EqualTo("Test2"));
        }

        [Test]
        public void Insert_FixedSize()
        {
            object[] myList = new object[5];
            GenericList<string> stringList = new GenericList<string>(myList);

            myList[0] = "Test";
            myList[1] = "Test2";
            Assert.That(() => { stringList.Insert(1, null); }, Throws.TypeOf<NotSupportedException>());

            Assert.That(myList[0], Is.EqualTo("Test"));
            Assert.That(myList[1], Is.EqualTo("Test2"));
        }

        [Test]
        public void RemoveAtWrapper_Generic()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            stringList.RemoveAt(0);
            Assert.That(stringList.Count, Is.EqualTo(0));
            Assert.That(myList.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveAtWrapper_List()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.RemoveAt(0);
            Assert.That(stringList.Count, Is.EqualTo(0));
            Assert.That(myList.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveAtWrapper_FixedSize()
        {
            object[] myList = new object[5];
            GenericList<string> stringList = new GenericList<string>(myList);

            myList[0] = "Test";
            myList[1] = "Test2";
            Assert.That(() => { stringList.RemoveAt(1); }, Throws.TypeOf<NotSupportedException>());

            Assert.That(myList[0], Is.EqualTo("Test"));
            Assert.That(myList[1], Is.EqualTo("Test2"));
        }

        [Test]
        public void ClearWrapper_Generic()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            stringList.Clear();
            Assert.That(stringList.Count, Is.EqualTo(0));
            Assert.That(myList.Count, Is.EqualTo(0));
        }

        [Test]
        public void ClearWrapper_List()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Clear();
            Assert.That(stringList.Count, Is.EqualTo(0));
            Assert.That(myList.Count, Is.EqualTo(0));
        }

        [Test]
        public void ClearWrapper_FixedSize()
        {
            object[] myList = new object[5];
            GenericList<string> stringList = new GenericList<string>(myList);

            myList[0] = "Test";
            myList[1] = "Test2";
            stringList.Clear();
            Assert.That(myList[0], Is.Null);
            Assert.That(myList[1], Is.Null);
            Assert.That(stringList[0], Is.Null);
            Assert.That(stringList[1], Is.Null);
        }

        [Test]
        public void ContainsWrapper()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Add("Test2");

            Assert.That(stringList.Contains("Test"), Is.True);
            Assert.That(stringList.Contains("Test2"), Is.True);
            Assert.That(stringList.Contains(null), Is.False);
            Assert.That(stringList.Contains("Test3"), Is.False);
        }

        [Test]
        public void Enumeration_String()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Add("Test2");

            int count = 0;
            foreach (string value in stringList) {
                switch (count) {
                case 0:
                    Assert.That(value, Is.EqualTo("Test"));
                    break;
                case 1:
                    Assert.That(value, Is.EqualTo("Test2"));
                    break;
                default:
                    Assert.Fail("Too many entries returned");
                    break;
                }
                count++;
            }

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Enumeration_NonString()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add(new object());
            myList.Add(new IntPtr(-1));

            int count = 0;
            foreach (string value in stringList) {
                switch (count) {
                case 0:
                    Assert.That(value, Is.Null);
                    break;
                case 1:
                    Assert.That(value, Is.Null);
                    break;
                default:
                    Assert.Fail("Too many entries returned");
                    break;
                }
                count++;
            }

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Enumeration_MixedString()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add(new object());
            myList.Add("Test");
            myList.Add(new IntPtr(-1));

            int count = 0;
            foreach (string value in stringList) {
                switch (count) {
                case 0:
                    Assert.That(value, Is.Null);
                    break;
                case 1:
                    Assert.That(value, Is.EqualTo("Test"));
                    break;
                case 2:
                    Assert.That(value, Is.Null);
                    break;
                default:
                    Assert.Fail("Too many entries returned");
                    break;
                }
                count++;
            }

            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void CopyTo_String()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add("Test");
            myList.Add("Test2");

            string[] stringArray = new string[3];
            stringList.CopyTo(stringArray, 0);
            Assert.That(stringArray[0], Is.EqualTo("Test"));
            Assert.That(stringArray[1], Is.EqualTo("Test2"));
            Assert.That(stringArray[2], Is.Null);
        }

        [Test]
        public void CopyTo_NonString()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add(new object());
            myList.Add(new IntPtr(-1));

            string[] stringArray = new string[3];
            stringList.CopyTo(stringArray, 0);
            Assert.That(stringArray[0], Is.Null);
            Assert.That(stringArray[1], Is.Null);
            Assert.That(stringArray[2], Is.Null);
        }

        [Test]
        public void CopyTo_MixedString()
        {
            ArrayList myList = new ArrayList();
            GenericList<string> stringList = new GenericList<string>(myList);

            myList.Add(new object());
            myList.Add("Test");
            myList.Add(new IntPtr(-1));

            string[] stringArray = new string[3];
            stringList.CopyTo(stringArray, 0);
            Assert.That(stringArray[0], Is.Null);
            Assert.That(stringArray[1], Is.EqualTo("Test"));
            Assert.That(stringArray[2], Is.Null);
        }
    }
}
