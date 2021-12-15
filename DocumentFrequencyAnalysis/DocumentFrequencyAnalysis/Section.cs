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
        public string StrTitle;
        public string Content;
        public List<string> WordsList;

        // ------------------------- 객체 초기화 ----------------------------------------------------------------------------------------------------------------------------------

        public Section(string _Title, string _Content)
        {
            Title = _Title;
            Content = _Content;
            WordsList = new List<string>();
        }

        public Section()
        {
            Title = "";
            Content = "";
            WordsList = new List<string>();
        }
    }
}
