using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentFrequencyAnalysis
{
    public class Manager
    {
        List<Document> Documents = new List<Document>();

        string DataDirectoryPath;
        string RemovePath;

        // ------------------------- 객체 초기화 ----------------------------------------------------------------------------------------------------------------------------------

        public Manager(string DataPath, string Remove)
        {
            DataDirectoryPath = DataPath;
            RemovePath = Remove;

            Init();
        }     

        /// <summary>
        /// 초기화
        /// </summary>
        private void Init()
        {
            try
            {
                FileInfo[] fi = new DirectoryInfo(DataDirectoryPath).GetFiles("*.txt");

                foreach(var info in fi)
                {
                    Documents.Add(new Document(info.FullName, RemovePath));
                }

                Console.WriteLine(Documents.Count + "개의 파일을 찾았습니다!\n");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // ------------------------- 출력 관련 ------------------------------------------------------------------------------------------------------------------------------------

        public void ShowAllSectionName()
        {
            List<string> sn = new List<string>();

            for (int i = 0; i < Documents.Count; i++)
            {
                List<string> Names = Documents[i].GetAllSectionsNames();
                sn.AddRange(Names);
                Console.WriteLine($"{(i + 1)} 번째 문서에서 {Names.Count}개의 섹션을 추출하였습니다.");
            }

            Console.WriteLine($"\n총{sn.Count}개의 섹션명을 추출하였습니다.\n");

            var List = sn.GroupBy(x => x).ToList();

            Console.WriteLine($"{"  섹션명", -67} │ {"   출현한 문서 개수", -15}");
            ShowBoundary(" 섹션명".Length + 67, "    출현한 문서 개수".Length + 15);

            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine($"{List[i].Key,  -70} │ {List[i].Count()}");
            }

            Console.WriteLine();
        }

        private void ShowBoundary(int Length1, int Length2)
        {
            for (int i = 0; i < Length1; i++) Console.Write("-");
            Console.Write("┼");
            for (int i = 0; i < Length2; i++) Console.Write("-");
            Console.WriteLine();
        }

        public void ShowContainSection(string SectionName)
        {
            Console.WriteLine($"{"  파일명",-67} │ {"   포함 여부",-15}");
            ShowBoundary(" 파일명".Length + 67, "    포함 여부".Length + 15);

            foreach (var document in Documents)
            {
                (bool, int) P = document.FindSection(SectionName);
                Console.WriteLine($"{document.NDocumentName(), -71} : {P.Item1, -15}");
            }
            Console.WriteLine();
        }

        public void ShowSelDuplicateWords(string SectionName)
        {
            List<string> Words = new List<string>();

            foreach(var Doc in Documents)
            {
                (bool, int) Point = Doc.FindSection(SectionName);
                
                if(Point.Item1)
                    Words.AddRange(Doc.SelectedSectionWordList(Point.Item2));
            }

            var DuplicateWords = Words.GroupBy(x => x)
                                           .Where(g => g.Count() > 1)
                                           .Select(y => y.Key)
                                           .OrderBy(x => x)
                                           .ToList();

            Console.WriteLine($"{$"  섹션 [{SectionName}] 의 주요 단어",-62} │ {"   개수",-15}");
            ShowBoundary($"섹션 [] 의 주요 단어".Length + 57, "    개수".Length + 15);
            foreach (var Word in DuplicateWords)
            {
                int Count = 0;
                foreach(var document in Documents)
                {
                    Count += document.WordCount(SectionName, Word);
                }
                Console.WriteLine($"{Word, -69} │ {Count, -15}");
            }
            Console.WriteLine();
        }
    }
}
