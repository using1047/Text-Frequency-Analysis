using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DocumentFrequencyAnalysis
{
    class Program
    {
        static string DataDirectoryPath = @"C:\Users\82105\Desktop\회사자료\UNCIENT\CasData\Data";
        static string RemoveDirectoryPath = @"C:\Users\82105\Desktop\회사자료\UNCIENT\CasData\Remove";

        static DirectoryInfo DataDi;
        static DirectoryInfo RemoveDi;
        static FileInfo[] fi;
        static Document[] Doc;

        static void Main(string[] args)
        {
            try
            {
                DataDi = new DirectoryInfo(DataDirectoryPath);
                RemoveDi = new DirectoryInfo(RemoveDirectoryPath);

                fi = DataDi.GetFiles("*.txt");
                string RemovePath = RemoveDi.GetFiles("*.txt")[0].FullName;
                Doc = new Document[fi.Length];

                for (int fileNum = 0; fileNum < Doc.Length; fileNum++)
                {
                    Doc[fileNum] = new Document(fi[fileNum].FullName, RemovePath);
                    //Doc[fileNum].Print_SelectWordsList();

                    Console.WriteLine(Doc[fileNum].MaxLength);
                    //Console.ReadKey();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static string MakeNameField(string Name, int MaxLength, int Sort)
        {
            string Value = "";
            switch(Sort)
            {
                // 왼쪽 정렬
                case 1:
                    if(Name.Length >= MaxLength)
                    {
                        Value = Name.Substring(0, MaxLength);
                    }
                    else
                    {
                        Value = Name;
                        for (int i = Name.Length; i < MaxLength; i++)
                        {
                            Value += " ";
                        }
                    }
                    break;

                // 가운데 정렬
                case 2:
                    int Mid = MaxLength - Name.Length;
                    // 5 - 1 = 4
                    if(Mid % 2 == 0)
                    {
                        for (int i = 0; i < Mid / 2; i++)
                        {
                            Value += " ";
                        }
                        Value += Name;
                        for (int i = 0; i < Mid / 2; i++)
                        {
                            Value += " ";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (Mid / 2) + 1; i++)
                        {
                            Value += " ";
                        }
                        Value += Name;
                        for (int i = 0; i < (Mid / 2); i++)
                        {
                            Value += " ";
                        }
                    }

                    break;
                // 오른쪽 정렬
                case 3:
                   
                    break;
            }

            return Value;
        }


    }
}
