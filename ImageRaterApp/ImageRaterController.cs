using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageRaterApp
{
    public class ImageRaterController
    {
        string currentDirectory;
        List<string> files;
        int currentFileIndex = 0;
        int numberOfFiles => files.Count;
        StreamWriter? outputFile;

        // file : rating
        Dictionary<string, int> Ratings = new Dictionary<string, int>();
        // file : rated
        Dictionary<string, bool> Rated = new Dictionary<string, bool>();


        public ImageRaterController(string directory)
        {
            currentDirectory = directory;
            LoadRatings();
            LoadFiles();
            CheckRated();
            outputFile = new StreamWriter(File.Open(Path.Combine(currentDirectory, "image_rating.csv"), FileMode.Create));
        }

        // Destructor to close file
        ~ImageRaterController()
        {
            outputFile?.Close();
            outputFile?.Dispose();
        }

        public void Close()
        {
            outputFile?.Close();
            outputFile?.Dispose();
        }

        public void SaveRatings()
        {
            foreach (var file in files)
            {
                if (Rated[file])
                {
                    outputFile?.WriteLine(file + Ratings[file]);
                }
            }
            outputFile?.Flush();
        }

        void CheckRated()
        {
            foreach(string file in files)
            {
                if(Ratings.ContainsKey(file)) {
                    Rated[file] = true;
                }
                else
                {
                    Rated[file] = false;
                }
            }
        }

        bool CheckForRatings(string directory) {
            
            return File.Exists(Path.Combine(directory, "image_rating.csv"));
        }
        static string[] filetypes = { ".bmp", ".jpg", ".gif", ".png", ".jpeg" };
        void LoadFiles() { 
            files = Directory.EnumerateFiles(currentDirectory)
                .Where((s) => filetypes.Any((ft) => s.Contains(ft, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        

        void LoadRatings()
        {
            if(!CheckForRatings(currentDirectory)) { return; }

            Ratings = File.ReadLines(Path.Combine(currentDirectory, "image_rating.csv"))
                .ToDictionary(
                    (s) => s.Substring(0, s.IndexOf(',')), 
                    (s) => int.Parse(s.AsSpan(s.IndexOf(',')))
                    );
        }

        public bool IsRated()
        {
            return Rated[files[currentFileIndex]];
        }

        public int GetCurrentRating()
        {
            return Ratings.GetValueOrDefault(files[currentFileIndex], 2);
        }

        public void SetRating(int rating)
        {
            Rated[files[currentFileIndex]] = true;
            Ratings[files[currentFileIndex]] = rating;
        }

        // returns true when reached end of files
        public bool NextFile()
        {
            currentFileIndex = (currentFileIndex+1) % numberOfFiles;
            return currentFileIndex == 0;
        }
        public void PreviousFile()
        {
            currentFileIndex = (currentFileIndex - 1 + numberOfFiles) % numberOfFiles;
        }

        public string StringProgress()
        {
            return $"Progress: [{currentFileIndex}/{numberOfFiles}] {100 * currentFileIndex / numberOfFiles}%";
        }

        public string StringCurrentFile()
        {
            return $"Current: {Path.GetFileName(files[currentFileIndex])}";
        }

        public string StringCurrentDirectory()
        {
            return $"Current Directory: {currentDirectory}";
        }
        public string GetCurrentFile()
        {
            return files[currentFileIndex];
        }
        
    }
}
