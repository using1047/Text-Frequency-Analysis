using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentFrequencyAnalysis
{
    public class Section
    {
        public string Title;
        public string Content;

        public Section(string _Title, string _Content)
        {
            Title = _Title;
            Content = _Content;
        }

        public Section()
        {
            Title = "";
            Content = "";
        }
    }
}
