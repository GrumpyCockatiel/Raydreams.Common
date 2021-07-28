using System;

namespace Raydreams.Common.Model
{
    /// <summary>Dictionary of test data to use in the randomizer</summary>
    public static class TestData
    {
        static TestData() { }

        private static string[] _fnames = new string[] { "Sid", "Alex", "Sally", "Bubba", "Gladys", "Frank", "Albert", "Samantha", "Marsha", "Rocky", "Han", "Jenny", "James", "Luke" };

        private static string[] _lnames = new string[] { "Ceasar", "Watson", "May", "Jones", "Brown", "Foxy", "Capone", "Blackwell", "White", "Washington", "Skylark", "Frank", "Jefferson", "Liu", "Smith" };

        /// <summary></summary>
        public static string[] FirstNames
        {
            get { return _fnames; }
        }

        /// <summary></summary>
        public static string[] LastNames
        {
            get { return _lnames; }
        }
    }
}

//var bdays = ["1/1/2000", "2/2/1980", "3/3/1971", "4/4/1970", "12/25/1960"];
