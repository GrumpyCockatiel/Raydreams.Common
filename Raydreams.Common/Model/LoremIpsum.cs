using System;

namespace Raydreams.Common.Model
{
    /// <summary>A dictionary of fake Latin words</summary>
    public static class LoremIpsum
    {
        private static string[] _words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "ut", "laoreet", "dolore", "magna", "aliquam", "erat", "etiam", "tincidunt", "viverra", "enim", "quis", "scelerisque", "donec", "tortor", "vel", "odio", "convallis", "id", "mauris", "lacus", "nulla", "placerat", "eu", "consequat", "egestas", "calicus", "ego", "nevo", "perspiciatis", "unde", "omnis", "iste", "natus", "error", "voluptatem", "accusantium", "doloremque", "laudantium", "totam", "rem", "aperiam", "eaque", "ipsa", "quae", "ab", "illo", "inventore", "veritatis", "quasi", "architecto", "beatae", "vitae", "dicta", "explicabo", "nemo", "ipsam", "voluptas", "aspernatur", "odit", "fugit", "consequuntur", "magni", "eos", "ratione", "sequi", "nesciunt", "neque", "porro", "quisquam", "est", "qui", "do", "consectetur", "adipisci", "quia", "non", "numquam", "eius", "modi", "tempora", "incidunt", "labore", "magnam", "quaerat", "ad", "minima", "veniam", "nostrum", "exercitationem", "ullam", "corporis", "suscipit", "laboriosam", "nisi", "aliquid", "ex", "ea", "commodi", "consequatur", "autem", "eum", "iure", "reprehenderit", "in", "voluptate", "velit", "esse", "quam", "pubus", "illum", "dolorem", "fugiat", "quo", "pariatur", "at", "vero", "et", "accusamus", "iusto", "dignissimos", "ducimus", "blanditiis", "praesentium", "voluptatum", "deleniti", "atque", "corrupti", "quos", "dolores", "quas", "molestias", "excepturi", "obcaecati", "cupiditate", "provident", "similique", "sunt", "culpa", "officia", "deserunt", "mollitia", "animi", "laborum", "fuga", "harum", "quidem", "rerum", "facilis", "expedita", "distinctio", "nam", "libero", "tempore", "soluta", "nobis", "eligendi", "optio", "cumque", "nihil", "impedit", "minus", "quod", "maxime", "placeat", "facere", "possimus", "assumenda", "repellendus", "temporibus", "autlis", "quibusdam", "aut", "officiis", "debitis", "necessitatibus", "saepe", "eveniet", "voluptates", "repudiandae", "sint", "molestiae", "recusandae", "itaque", "earum", "hic", "tenetur", "a", "sapiente", "delectus", "reiciendis", "voluptatibus", "maiores", "alias", "perferendis", "doloribus", "asperiores", "repellat", "minim", "nostrud", "exercitation", "ullamco", "laboris", "aliquip", "commodo", "duis", "aute","officius", "orcus", "polius", "incididunt", "domo", "nostro", "pulvinar", "varius", "cras", "gravitas", "uso", "oric", "rasparda", "fermentum", "diaz", "tobias", "cucneum" };

        static LoremIpsum() { }

        /// <summary>Returns the entire dictionary</summary>
        public static string[] Values
        {
            get { return _words; }
        }
    }
}
