using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VoiceProximityMeasurement
{
    /// <summary>
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        public Result()
        {
            InitializeComponent();
            LoadData();
        }

        private string GetExcelFilePath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = System.IO.Path.Combine(baseDirectory, "Answer.xlsx");

            return filePath;
        }


        private void LoadData()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var filePath = GetExcelFilePath();
            var data = new List<ResultData>();
            var highestPercentage = 0.0;
            var highestPercentageRow = -1;
            var degreesOfMyopia = new double[8] { 6.8, 6.4, 6.0, 5.6, 5.2, 4.8, 4.4, 4.0 };

            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var question = worksheet.Cells[row, 1].Text.Trim();
                        var answer = worksheet.Cells[row, 2].Text.Trim();

                        var questionWords = question.Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                        var answerWords = new List<string>(answer.Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries));

                        int matchingWords = 0;
                        foreach (var word in questionWords)
                        {
                            if (answerWords.Contains(word, StringComparer.OrdinalIgnoreCase))
                            {
                                matchingWords++;
                                answerWords.Remove(word);
                            }
                        }

                        var matchPercentage = (double)matchingWords / questionWords.Length * 100;
                        if (matchPercentage > highestPercentage)
                        {
                            highestPercentage = matchPercentage;
                            highestPercentageRow = rowCount - row;
                        }

                        data.Add(new ResultData
                        {
                            PictureQuestion = question,
                            PictureAnswer = answer,
                            MatchingElements = $"{matchingWords} / {questionWords.Length}",
                            MatchPercentage = $"{matchPercentage:N2}%",
                            DegreeOfMyopia = $"4/{degreesOfMyopia[row - 2]}"

                        });
                    }

                }
                else
                {
                    //MessageBox.Show("Worksheet not found in the Excel file.");
                }
            }

            ResultsDataGrid.ItemsSource = data;

        }



        public class ResultData
        {
            public string PictureQuestion { get; set; }
            public string PictureAnswer { get; set; }
            public string MatchingElements { get; set; }
            public string MatchPercentage { get; set; }
            public string DegreeOfMyopia { get; set; }
        }

        private void CloseResult(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

    }
}
