﻿using System;
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

                String nameCol = Name_Column.Text;
                String dateCol = Date_Column.Text;
                String descCol = Description_Column.Text;
                Executor executor = new Executor();
                executor.execute(Source_Spreadsheet_Path.Text, Destination_Spreadsheet_Path.Text, nameCol, dateCol, descCol);

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
            String input = Destination_Spreadsheet_Path.Text;
            String fileExtension = input.Substring(input.Length - 5, 5);
            if(fileExtension != ".xlsx")
            {
                return false;
            }
            return true;
        }
    }
}
