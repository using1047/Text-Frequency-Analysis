using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentFrequencyAnalysis
{
    public class Document
    {
        /// <summary>
        /// 데이터 문서 위치
        /// </summary>
        string DataFilePath;
        /// <summary>
        /// 삭제할 단어들이 있는 문서 위치
        /// </summary>
        string RemoveWordsPath;
        /// <summary>
        /// 문서명
        /// </summary>
        string DocumentName;

        /// <summary>
        /// 이 문서에 포함되어 있는 섹션들
        /// </summary>
        List<Section> sections;

        /// <summary>
        /// 타이틀이 포함되어 있는 전체 문장 패턴
        /// </summary>
        Regex ExtractionTitleLine = new Regex("[0-9][.][0-9]+[.][0-9]+[A-Z]");
        /// <summary>
        /// 타이틀이 포함되어 있는 전체 문장에서 문자열만 추출하는 패턴
        /// </summary>
        Regex ExtractionTitleStr = new Regex("[0-9]+[.][0-9]+[.][0-9]+");
        /// <summary>
        /// 메인 타이틀이 포함되어 있는지 콘텐츠에서 추출하기 위한 패턴
        /// </summary>
        Regex ExtractionMainTitle = new Regex("[0-9]+[.][0-9]+[A-Z]");
        /// <summary>
        /// 마침표가 포함된 단어를 추출하는 패턴
        /// </summary>
        Regex ExtractionRMDotStr = new Regex("[a-z0-9][.]");
        /// <summary>
        /// 마침표가 포함된 숫자가 들어있는 단어를 추출하는 패턴
        /// </summary>
        Regex ExtractionFloat = new Regex("[^0-9][.][0-9]");

        /// <summary>
        /// 삭제될 단어들
        /// </summary>
        List<string> RemoveWordsList;

        // ------------------------- 객체 초기화 ----------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 최소 초기화
        /// </summary>
        public Document()
        {
            sections = new List<Section>();
            RemoveWordsList = new List<string>();
        }

        /// <summary>
        /// 파일 지정 초기화
        /// </summary>
        /// <param name="DataFilePath">데이터 파일 위치</param>
        /// <param name="RemoveWordsPath">삭제할 단어들이 포함된 파일 위치</param>
        public Document(string DataFilePath, string RemoveWordsPath)
        {
            sections = new List<Section>();
            RemoveWordsList = new List<string>();

            this.DataFilePath = DataFilePath;
            this.RemoveWordsPath = RemoveWordsPath;
            // 4 : .txt 를 뜻함
            this.DocumentName = ExtractionFileName(DataFilePath);

            // 기본 설정이 되어있으므로, 바로 읽어서 변수 할당
            Read();

            ExtractionWord();
        }

        // ------------------------- 객체 함수 -------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 파일을 읽고, 모든 변수들에 값 할당
        /// </summary>
        /// <returns>처리 결과</returns>
        bool Read()
        {
            try
            {
                ReadRemoveWords();

                // 디버그 콘솔 문구
                Debug.WriteLine(DocumentName +"의 타이틀 출력 상태 : " + ExtractionTitle());
                Debug.WriteLine(DocumentName + "의 콘텐츠 출력 상태 : " + ExtractionContent());
                RemoveMainTitle();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 삭제할 단어가 포함된 파일을 읽어오기 및
        /// RemoveWordsList에 단어들 추가
        /// </summary>
        /// <returns>처리 결과</returns>
        bool ReadRemoveWords()
        {
            try
            {
                RemoveWordsList.AddRange(File.ReadAllLines(RemoveWordsPath));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 출력할 타이틀의 섹션 선택하기
        /// </summary>
        public void Print_SelectTitle()
        {
            Console.WriteLine(DocumentName + " 파일의 Section 번호를 입력해주세요.( 0 ~ " + (sections.Count - 1) + " )" + "\n");

            string Number = Console.ReadLine();
            Console.WriteLine(SelectedTitle(Number));
        }

        /// <summary>
        /// 출력할 콘텐츠의 섹션 선택하기
        /// </summary>
        public void Print_SelectContent()
        {
            Console.WriteLine(DocumentName + " 파일의 Section 번호를 입력해주세요.( 0 ~ " + (sections.Count - 1) + " )" + "\n");
            
            string Number = Console.ReadLine();
            Console.WriteLine(SelectedContent(Number));
        }

        /// <summary>
        /// 출력할 단어 목록의 섹션 선택하기
        /// </summary>
        public void Print_SelectWordsList()
        {
            Console.WriteLine(DocumentName + " 파일의 Section 번호를 입력해주세요.( 0 ~ " + (sections.Count - 1) + " )" + "\n");

            string Number = Console.ReadLine();
            Console.WriteLine(SelectedWordsList(Number));
        }

        /// <summary>
        /// 선택한 섹션의 타이틀을 가져오는 함수
        /// </summary>
        /// <param name="Sel">입력한 번호</param>
        /// <returns>타이틀</returns>
        string SelectedTitle(string Sel)
        {
            try
            {
                string Str = "";
                int Number = int.Parse(Sel.Replace("\n", ""));
                if (Number == 0)
                {
                    foreach (var section in sections)
                    {
                        Str += section.Title + "\n\n";
                    }
                }
                else
                {
                    Str = sections[Number].Title + "\n";
                }

                return Str;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString() + "\n");
                return "null";
            }
        }

        /// <summary>
        /// 선택한 섹션의 콘텐츠를 가져오는 함수
        /// </summary>
        /// <param name="Sel">입력한 번호</param>
        /// <returns>콘텐츠</returns>
        string SelectedContent(string Sel)
        {
            try
            {
                string Str = "";
                int Number = int.Parse(Sel.Replace("\n", ""));
                if(Number == 0)
                {
                    foreach(var section in sections)
                    {
                        Str += section.Content + "\n\n";
                    }
                }
                else
                {
                    Str = sections[Number].Content + "\n";
                }

                return Str;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString() + "\n");
                return "null";
            }
        }

        /// <summary>
        /// 선택한 섹션의 단어 목록을 가져오는 함수
        /// </summary>
        /// <param name="Sel">입력한 번호</param>
        /// <returns>단어 목록</returns>
        string SelectedWordsList(string Sel)
        {
            try
            {
                string Str = "";
                int Number = int.Parse(Sel.Replace("\n", ""));

                if (Number == 0)
                {
                    foreach (var section in sections)
                    {
                        foreach(var Word in section.WordsList)
                        {
                            Str += Word + "\n";
                        }
                        Str += "\n";
                    }
                }
                else
                {
                    foreach (var Word in sections[Number].WordsList)
                    {
                        Str += Word + "\n";
                    }
                }

                return Str;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString() + "\n");
                return "null";
            }
        }

        // ------------------------- 문자열 추출 함수들 --------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 타이틀만 추출하는 함수
        /// </summary>
        /// <param name="Str">데이터 파일의 전체 텍스트</param>
        /// <returns>처리 결과</returns>
        bool ExtractionTitle()
        {
            try
            {
                string[] Str = File.ReadAllLines(DataFilePath);
                // 한 문장 씩 읽어서 섹션 타이틀 찾아내기
                foreach (var Sentence in Str)
                {
                    if (IsSection(Sentence))
                    {
                        Section section = new Section();
                        section.Title = Sentence;
                        section.StrTitle = ExtractionStrTitle(Sentence);

                        sections.Add(section);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 타이틀에서 숫자를 제거한 문자열만 추출하는 함수
        /// </summary>
        /// <param name="Str">기존 타이틀</param>
        /// <returns>정제된 타이틀</returns>
        string ExtractionStrTitle(string Str)
        {
            Match m = ExtractionTitleStr.Match(Str);
            return Str.Replace(m.Value, "");
        }

        /// <summary>
        /// 특정 타이틀에 해당하는 내용만 추출하는 함수
        /// </summary>
        /// <returns>처리 결과</returns>
        bool ExtractionContent()
        {
            try
            {
                // 타이틀 개수만큼
                for (int Index = 0; Index < sections.Count; Index++)
                {
                    string AllText = File.ReadAllText(DataFilePath);
                    int Section_StartIndex = AllText.IndexOf(sections[Index].Title) + sections[Index].Title.Length + 2; // 왜 2지...??

                    // 마지막 섹션인지 확인
                    if (Index + 2 > sections.Count)
                    {
                        string RefinedStr = RemoveSpecialWords(AllText.Substring(Section_StartIndex));
                        sections[Index].Content = RefinedStr;
                    }
                    // 마지막 섹션이 아니라면
                    else
                    {
                        int Section_EndIndex = AllText.IndexOf(sections[Index + 1].Title);
                        int Length = Section_EndIndex - Section_StartIndex;

                        string RefinedStr = RemoveSpecialWords(AllText.Substring(Section_StartIndex, Length));
                        sections[Index].Content = RefinedStr;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 모든 콘텐츠에서 단어들을 추출하는 함수
        /// </summary>
        /// <returns>처리 결과</returns>
        bool ExtractionWord()
        {
            try
            {
                int S = 0;
                foreach (var section in sections)
                {
                    string Content = section.Content;

                    // 띄어쓰기 기준으로 단어 분리
                    string[] Words = Content.Split(' ');

                    foreach (var Word in Words)
                    {
                        if (!IsRemoveWord(Word))
                        {// 두 글자 이상이여야함
                            if (Word.Length > 2)
                            {
                                // 공백이 아니고
                                if (!IsWhiteWord(Word))
                                {
                                    // 숫자가 아니고
                                    if (!IsNumber(Word))
                                    {
                                        // 마침표 포함
                                        if (IsLastWord(Word))
                                        {
                                            string word = Word.Replace(".", "");
                                            if (!section.WordsList.Contains(word.ToLower()))
                                            {
                                                section.FrequencyList.Add(word.ToLower(), 1);
                                                section.WordsList.Add(word.ToLower());
                                            }
                                            else
                                            {
                                                section.FrequencyList[word.ToLower()]++;
                                            }
                                        }
                                        // 마침표 미포함
                                        else
                                        {
                                            if (!section.WordsList.Contains(Word.ToLower()))
                                            {
                                                section.FrequencyList.Add(Word.ToLower(), 1);
                                                section.WordsList.Add(Word.ToLower());
                                            }
                                            else
                                            {
                                                section.FrequencyList[Word.ToLower()]++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    S++;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 파일 이름만 추출하기
        /// </summary>
        /// <param name="Str">파일의 위치</param>
        /// <returns>파일명</returns>
        string ExtractionFileName(string Str)
        {
            string[] Directory = Str.Split('\\');

            string FileName = Directory[Directory.Length - 1].Replace(".txt", "");
            return FileName;
        }

        // ------------------------- 문자열 형식 함수들 --------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 섹션의 제목 패턴과 일치하는지 확인하는 함수
        /// </summary>
        /// <param name="Str">문장</param>
        /// <returns>결과</returns>
        bool IsSection(string Str)
        {
            if (ExtractionTitleLine.IsMatch(Str)) return true;
            else return false;
        }

        /// <summary>
        /// 숫자인지 판단하는 함수
        /// </summary>
        /// <param name="Str">검사할 문자열</param>
        /// <returns>결과</returns>
        bool IsNumber(string Str)
        {
            try
            {
                // E+, E- 포함 문구는 무조건 숫자로 처리
                if (Str.Contains("e+") || Str.Contains("e-")) return true;

                // 소수 처리
                if(Str.Contains("."))
                {
                    var Number = float.Parse(Str);
                    return true;
                }
                // 정수 처리
                else
                {
                    var Number = int.Parse(Str);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 마침표가 들어가있는 단어인지 판단하는 함수
        /// </summary>
        /// <param name="Str">검사할 문자열</param>
        /// <returns>결과</returns>
        bool IsLastWord(string Str)
        {
            if (!Str.Contains(".")) return false;

            if (ExtractionFloat.IsMatch(Str.ToLower())) return false;
            if (ExtractionRMDotStr.IsMatch(Str.ToLower())) return true;
            else return false;
        }

        /// <summary>
        /// 단어가 공백으로만 이루어져있는지 확인
        /// </summary>
        /// <param name="Str">검사할 문자열</param>
        /// <returns>결과</returns>
        bool IsWhiteWord(string Str)
        {
            if (Str == "" || Str == " " || Str == "\t") return true;
            Str = Str.Trim();

            if (Str == "") return true;
            else return false;
        }

        /// <summary>
        /// 삭제할 단어인지 확인
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        bool IsRemoveWord(string Str)
        {
            return RemoveWordsList.Contains(Str);
        }

        // ------------------------- 문자열 변형 함수들 --------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 특수 문자 제거
        /// </summary>
        /// <param name="Str">문자열</param>
        /// <returns>정제된 단어</returns>
        string RemoveSpecialWords(string Str)
        {
            string[] SpecialWords = {"[", "]", "(", ")", ":", ";", "%", "|"};
            string[] WhiteWords = {"\r\n", "\r", "\n"};

            foreach(var Word in SpecialWords)
            {
                Str = Str.Replace(Word, "");
            }

            foreach(var Word in WhiteWords)
            {
                Str = Str.Replace(Word, " ");
            }

            Str = Str.Replace("  ", " ");

            return Str;
        }

        /// <summary>
        /// 콘텐츠에서 메인 타이틀이 포함되어 있으면 정제하기
        /// </summary>
        void RemoveMainTitle()
        {
            foreach(var section in sections)
            {
                if(ExtractionMainTitle.IsMatch(section.Content))
                {
                    Match m = ExtractionMainTitle.Match(section.Content);
                    int EndSectionIndex = section.Content.IndexOf(m.Value);
                    section.Content = section.Content.Substring(0, EndSectionIndex);
                }
            }
        }
    }
}
