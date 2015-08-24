using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

using AutoCadPlugin.ViewModel;
using AutoCadPlugin.Util;

using AcadC = Autodesk.AutoCAD.Colors;

namespace AutoCadPlugin.View
{
    public partial class PluginWindow : UserControl
    {
        private LayersViewModel _layersViewModel;

        public LayersViewModel LayersDataContext
        {
            get
            {
                return _layersViewModel;
            }
        }

        // Конструктор.
        public PluginWindow()
        {
            _layersViewModel = new LayersViewModel();
            DataContext = this;

            InitializeComponent();
        }

        // Перезагрузка объектов документа.
        private void reload_Click(object sender, RoutedEventArgs e)
        {
            LayersDataContext.Reload();
        }

        // Сохранение изменений в документ.
        private void update_Click(object sender, RoutedEventArgs e)
        {
            LayersDataContext.Update();
        }

        // Открытие диалогового окна изменения цвета слоя.
        private void color_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LayersDataContext.SelectedLayer.ChangeColorByDlg();
        }       
    }  
}
