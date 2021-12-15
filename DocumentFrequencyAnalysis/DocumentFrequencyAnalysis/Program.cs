using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DocumentFrequencyAnalysis
{
    class Program
    {
        static string DataDirectoryPath = @"C:\Users\82105\Desktop\회사자료\UNCIENT\CasData\Data";
        static string RemovePath = @"C:\Users\82105\Desktop\회사자료\UNCIENT\CasData\Remove\REMOVE.txt";

        static Manager Manager = new Manager(DataDirectoryPath, RemovePath);

        static void Main(string[] args)
        {
            try
            { 
                Manager.ShowAllSectionName();

                Manager.ShowContainSection("First Aid");
                Manager.ShowSelDuplicateWords("First Aid");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
