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

namespace dcsdbeditor
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

        private void aircraftTagEnter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                (DataContext as MainVM).AddTagToAircraft.Execute(tagTextAircraft.Text);
                tagTextAircraft.Text = "";
            }
        }

        private void weaponTagEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (DataContext as MainVM).AddTagToWeapon.Execute(tagTextWeapon.Text);
                tagTextWeapon.Text = "";
            }
        }

        private void tabpressed(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Tab)
            {
                e.Handled = true;
                var d = DataContext as MainVM;
                d.SelectNextWeaponData();
                (sender as TextBox).SelectAll();
            }
        }
    }
}
