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

namespace RubikCubeSolverApp
{
    /// <summary>
    /// Logika interakcji dla klasy RubikFieldControl.xaml
    /// </summary>
    public partial class RubikFieldControl : UserControl
    {
        public object SelectedValue 
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
            nameof(SelectedValue), 
            typeof(object), 
            typeof(RubikFieldControl),
            new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public RubikFieldControl()
        {
            InitializeComponent();
        }
    }
}
