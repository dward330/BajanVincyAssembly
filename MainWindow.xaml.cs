using BajanVincyAssembly.Models.Infrastructure;
using BajanVincyAssembly.Services.Registers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BajanVincyAssembly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Registry Management System
        IRegistry<Register> Registry = new Registry();

        // Contain Information about Registers
        public ObservableCollection<Register> Registers = new ObservableCollection<Register>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            this.UpdateLatestSnapshotOfRegistryState();

            this.ListViewOfRegisters.ItemsSource = this.Registers;

            var updatedRegiser = this.Registry.GetRegister("temp0");
            updatedRegiser.SetBase10Value(5);

            this.Registry.SaveRegister(updatedRegiser);

            this.UpdateLatestSnapshotOfRegistryState();
        }

        /// <summary>
        /// Gets and saves latest snapshot of registry cache
        /// </summary>
        public void UpdateLatestSnapshotOfRegistryState()
        {
            // Get Latest State Of Registers
            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Registers.Clear();

                var registerState = this.Registry.GetRegisters().ToList();

                if (registerState.Any())
                {
                    foreach (var register in registerState)
                    {
                        this.Registers.Add(register);
                    }
                }
            }));
            
        }
    }
}
