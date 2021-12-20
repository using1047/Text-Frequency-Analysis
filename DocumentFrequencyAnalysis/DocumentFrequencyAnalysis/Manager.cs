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

        /// <summary>
        /// 모든 섹션의 이름 출력하기
        /// </summary>
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

        /// <summary>
        /// 콘솔 출력 시 테두리 출력하기
        /// </summary>
        /// <param name="Length1">좌측</param>
        /// <param name="Length2">우측</param>
        private void ShowBoundary(int Length1, int Length2)
        {
            for (int i = 0; i < Length1; i++) Console.Write("-");
            Console.Write("┼");
            for (int i = 0; i < Length2; i++) Console.Write("-");
            Console.WriteLine();
        }

        /// <summary>
        /// 특정 섹션이 문서에 있는지 출력
        /// </summary>
        /// <param name="SectionName"></param>
        public void ShowContainSection(string SectionName)
        {
            Console.WriteLine($"{"  파일명",-67} │ {"   포함 여부",-15}");
            ShowBoundary(" 파일명".Length + 67, "    포함 여부".Length + 15);

            foreach (var document in Documents)
            {
                (bool, int) P = document.FindSection(SectionName);
                Console.WriteLine($"{document.NDocumentName(), -71} | {P.Item1, -15}");
            }
            Console.WriteLine();
        }


        public void Metrics(string SectionName)
        {
            Console.WriteLine($"{SectionName} 의 매트릭스 생성 준비...");

            string MectricsFileName = @"C:\Users\82105\Desktop\회사자료\UNCIENT\CasData\Metrics\" + SectionName + "_Metrics.csv";
            string FrequencyFileName = @"C:\Users\82105\Desktop\회사자료\UNCIENT\CasData\Metrics\" + SectionName + "_Frequency.csv";

            List<string> Words = new List<string>();
            List<string> Sentences = new List<string>();
            List<string> AllWord = new List<string>();
            foreach (var Doc in Documents)
            {
                (bool, int) Point = Doc.FindSection(SectionName);

                if (Point.Item1)
                {
                    List<string> MoonJang = Doc.ContentINMoonJang(Point.Item2);

                    for (int i = 0; i < MoonJang.Count; i++)
                    {
                        MoonJang[i] = MoonJang[i].ToLower();
                    }

                    var TWords = Doc.SelectedSectionWordList(Point.Item2);
                    AllWord.AddRange(TWords);
                    Words.AddRange(TWords);
                    Sentences.AddRange(MoonJang);
                }
            }

            var words = Words.GroupBy(x => x)
                                          .ToList();

            Words.Clear();
            foreach(var word in words)
            {
                Words.Add(word.Key);
            }

            Console.WriteLine($"{SectionName} 의 문장 분리 완성...");

            int[][] Metrics = new int[Words.Count][];

            for (int x = 0; x < Words.Count; x++)
            {
                Metrics[x] = new int[Words.Count];
            }


            foreach(var Sentence in Sentences)
            {
                for (int y = 0; y < Words.Count; y++)
                {
                    for (int x = 0; x < Words.Count; x++)
                    {
                        if(x != y && Sentence.Contains(Words[x]) && Sentence.Contains(Words[y]))
                        {
                            Metrics[x][y]++;
                            Metrics[y][x]++;
                        }
                    }
                }
            }

            Console.WriteLine($"{SectionName} 의 단어 {Words.Count}개 계산완료...");

            string SectionWords = "Words,";
            for (int i = 0; i < Words.Count; i++) SectionWords += Words[i] + ",";
            // , 없애기
            SectionWords = SectionWords.Substring(0, SectionWords.Length - 1) + "\n";

            for (int y = 0; y < Words.Count; y++)
            {
                SectionWords += Words[y] + ",";
                for (int x = 0; x < Words.Count; x++)
                {
                    SectionWords += (Metrics[y][x] / 2) + ",";
                    //Console.Write($"{Metrics[y][x], -3}");
                }
                SectionWords = SectionWords.Substring(0, SectionWords.Length - 1) + "\n";
                //Console.WriteLine();
            }

            Console.WriteLine($"{SectionName} 의 매트릭스 의미망 파일 생성 중...");

            if (File.Exists(MectricsFileName)) File.Delete(MectricsFileName);
            File.WriteAllText(MectricsFileName, SectionWords);

            Dictionary<string, int> WordBook = new Dictionary<string, int>();



            foreach (var Word in Words)
            {
                if (!WordBook.ContainsKey(Word))
                {
                    var Item = WordCount(AllWord, Word);
                    WordBook.Add(Item.Item1, Item.Item2);
                }
                else WordBook[Word]++;
            }

            SectionWords = "Words,Frequency\n";

            foreach(var word in WordBook)
            {
                SectionWords += word.Key + "," + word.Value + "\n";
            }

            Console.WriteLine($"{SectionName} 의 매트릭스 빈도수 파일 생성 중...");

            if (File.Exists(FrequencyFileName)) File.Delete(FrequencyFileName);
            File.WriteAllText(FrequencyFileName, SectionWords, Encoding.UTF8);
        }

        /// <summary>
        /// 특정 섹션의 중복 단어들 출력하기
        /// </summary>
        /// <param name="SectionName">섹션 이름</param>
        public void ShowSelDuplicateWords(string SectionName)
        {
            List<string> Words = new List<string>();

            // 모든 문서에서 같은 섹션의 이름을 찾기
            foreach (var Doc in Documents)
            {
                (bool, int) Point = Doc.FindSection(SectionName);

                if (Point.Item1)
                    Words.AddRange(Doc.SelectedSectionWordList(Point.Item2));
            }

            Console.WriteLine($"{$"  섹션 [{SectionName}] 의 주요 단어",-62} │ {"   개수",-15}");
            ShowBoundary($"섹션 [] 의 주요 단어".Length + 57, "    개수".Length + 15);

            Dictionary<string, int> WordBook = new Dictionary<string, int>();
            foreach (var Word in Words)
            {
                if (!WordBook.ContainsKey(Word))
                {
                    var Item = WordCount(Words, Word);
                    WordBook.Add(Item.Item1, Item.Item2);
                }    
            }

            var Book = WordBook.OrderByDescending(w => w.Value);

            foreach(var Word in Book)
            {
                Console.WriteLine($"{Word.Key, -69} | {Word.Value,  -15}");
            }
            Console.WriteLine();
        }

        private (string, int) WordCount(List<string> All, string SearchWord)
        {
            int Count = 0;
            foreach(var Word in All)
            {
                if (Word == SearchWord) Count++;
            }

            return (SearchWord, Count);
        }

        public void ShowMoonjang(string SectionName)
        {
            foreach (var Doc in Documents)
            {
                (bool, int) Point = Doc.FindSection(SectionName);

                if (Point.Item1)
                {
                    List<string> Str = Doc.ContentINMoonJang(Point.Item2);

                    Console.WriteLine(Doc.sections[Point.Item2].Title);
                    foreach(var str in Str)
                    {
                        Console.WriteLine(str);
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
