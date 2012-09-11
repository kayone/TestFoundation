using System;
using System.IO;
using AutoMoq;
using Moq;
using NUnit.Framework;

namespace Kayone.TestFoundation
{
    public abstract class TestCore : TestCore<object>
    {
        protected override object Subject
        {
            get
            {
               throw new InvalidOperationException("Subject is not available in none-generic implementation of TestCore");
            }
        } 
    }


    public abstract class TestCore<TSubject> : LoggingTest where TSubject : class
    {
        private AutoMoqer _mocker;
        protected AutoMoqer Mocker
        {
            get
            {
                if (_mocker == null)
                {
                    _mocker = new AutoMoqer();
                }

                return _mocker;
            }
        }

        [SetUp]
        public void TestCoreSetup()
        {

        }

        [TearDown]
        public void TestCoreTearDown()
        {
            _mocker = null;
            _subject = null;
        }


        private TSubject _subject;

        protected virtual TSubject Subject
        {
            get
            {
                if (_subject == null)
                {
                    _subject = Mocker.Resolve<TSubject>();
                }

                return _subject;
            }
        }

        public static string GetLongString(int lenght)
        {
            return new String('x', lenght);
        }

        public static string TempFolder
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "temp"); }
        }
    }
}