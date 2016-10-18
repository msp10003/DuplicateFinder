using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DuplicateFinder;
using System.Text.RegularExpressions;

namespace DuplicateFinderGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           // System.Windows.Threading.Dispatcher.Run();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Alert_Text.Visibility = Visibility.Hidden;

                if (!ValidateColumns())
                {
                    Alert_Text.Text = "Columns must be single capital letters";
                    Alert_Text.Foreground = Brushes.Red;
                    Alert_Text.Visibility = Visibility.Visible;
                    return;
                }

                if (!ValidateOutputPath())
                {
                    Alert_Text.Text = "Output file must be an .xlsx file";
                    Alert_Text.Foreground = Brushes.Red;
                    Alert_Text.Visibility = Visibility.Visible;
                    return;
                }

                if (!ValidateOverwrite())
                {
                    Alert_Text.Text = "The output file matches the input file, please select a different output file to avoid overwriting";
                    Alert_Text.Foreground = Brushes.Red;
                    Alert_Text.Visibility = Visibility.Visible;
                    return;
                }

                String nameCol = Name_Column.Text;
                String dateCol = Date_Column.Text;
                String descCol = Description_Column.Text;
                int numCols = 0;
                Int32.TryParse(NumCols.Text, out numCols);

                Executor executor = new Executor();
                executor.execute(Source_Spreadsheet_Path.Text, Destination_Spreadsheet_Path.Text, nameCol, dateCol, descCol, 
                    numCols, NamePrecisionSlider.Value, Scan_Dates_Checkbox.IsChecked, Scan_Descriptions_Checkbox.IsChecked, DatePrecisionSlider.Value, DescriptionPrecisionSlider.Value, AutoSearchCheckbox.IsChecked);

                Alert_Text.Text = "Successfully scanned file for duplicates! Check the new file created for results!";
                Alert_Text.Foreground = Brushes.LimeGreen;
                Alert_Text.Visibility = Visibility.Visible;
            }
            catch(Exception exception)
            {
                Alert_Text.Text = exception.Message + "!";
                Alert_Text.Foreground = Brushes.Red;
                Alert_Text.Visibility = Visibility.Visible;
            }
        }

        private void Tolerance_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Source_Spreadsheet_Path_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Destination_Spreadsheet_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            Source_Spreadsheet_Path.Text = null;
            Destination_Spreadsheet_Path.Text = null;
            Name_Column.Text = null;
            Date_Column.Text = null;
            Description_Column.Text = null;
            Alert_Text.Text = null;
            Alert_Text.Visibility = Visibility.Hidden;
        }

        private bool ValidateColumns()
        {
            if(!Regex.IsMatch(Name_Column.Text, @"^[A-Z]+$")
                || !Regex.IsMatch(Date_Column.Text, @"^[A-Z]+$")
                || !Regex.IsMatch(Description_Column.Text, @"^[A-Z]+$")){
                return false;
            }
            return true;
        }

        private bool ValidateOutputPath()
        {
            //TODO: don't hardcode in 5 indexes back, should look at wheret the period is instead
            String input = Destination_Spreadsheet_Path.Text;
            String fileExtension = input.Substring(input.Length - 5, 5);
            if(fileExtension != ".xlsx")
            {
                return false;
            }
            return true;
        }

        private bool ValidateOverwrite()
        {
            if (Destination_Spreadsheet_Path.Text == Source_Spreadsheet_Path.Text)
            {
                return false;
            }
            return true;
        }

        private void File_Browser_Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xlsx";
            //dlg.Filter = "XLSX Files (*.xlsx)"; 

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                Source_Spreadsheet_Path.Text = filename;
            }

        }

        private void Scan_Progress_Bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void NumCols_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DatePrecisionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Scan_Dates_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoSearchCheckbox == null)
            {
                return;
            }
            else if (AutoSearchCheckbox.IsChecked == false)
            {
                DatePrecisionSlider.IsEnabled = true;
            }
        }

        private void Scan_Dates_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AutoSearchCheckbox == null)
            {
                return;
            }
            else if (AutoSearchCheckbox.IsChecked == false)
            {
                DatePrecisionSlider.IsEnabled = false;
            }
        }

        private void DescriptionPrecisionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Scan_Descriptions_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoSearchCheckbox == null)
            {
                return;
            }
            else if (AutoSearchCheckbox.IsChecked == false)
            {
                DescriptionPrecisionSlider.IsEnabled = true;
            }
        }

        private void Scan_Descriptions_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AutoSearchCheckbox == null)
            {
                return;
            }
            else if (AutoSearchCheckbox.IsChecked == false)
            {
                DescriptionPrecisionSlider.IsEnabled = false;
            }
        }

        private void NamePrecisionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void AutoSearch_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NamePrecisionSlider.IsEnabled = false;
            DatePrecisionSlider.IsEnabled = false;
            DescriptionPrecisionSlider.IsEnabled = false;
        }

        private void AutoSearch_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            NamePrecisionSlider.IsEnabled = true;
            
            if (Scan_Dates_Checkbox.IsChecked == true)
            {
                DatePrecisionSlider.IsEnabled = true;
            }
            if(Scan_Descriptions_Checkbox.IsChecked == true)
            {
                DescriptionPrecisionSlider.IsEnabled = true;
            }
        }

    }
}
