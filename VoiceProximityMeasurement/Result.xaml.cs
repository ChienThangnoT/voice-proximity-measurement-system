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

        private void LoadData()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var filePath = "W:\\Ses7\\PRN221\\VoiceProximityMeasurementSystem\\VoiceProximityMeasurement\\bin\\Debug\\net7.0-windows\\Answer.xlsx";
            var data = new List<ResultData>();

            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
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
                            answerWords.Remove(word); // Loại bỏ từ đã khớp để không tính lại
                        }
                    }

                    var matchPercentage = (double)matchingWords / questionWords.Length * 100;

                    data.Add(new ResultData
                    {
                        PictureQuestion = question,
                        PictureAnswer = answer,
                        MatchingElements = $"{matchingWords} / {questionWords.Length}",
                        MatchPercentage = $"{matchPercentage:N2}%"
                    });
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
        }

        private void CloseResult(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

    }
}
