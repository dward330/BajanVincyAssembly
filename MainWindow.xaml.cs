using BajanVincyAssembly.Models.ComputerArchitecture;
using BajanVincyAssembly.Models.Validation;
using BajanVincyAssembly.Services.Compilers;
using BajanVincyAssembly.Services.Processor;
using BajanVincyAssembly.Services.Registers;
using BajanVincyAssembly.UserControls;
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
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            this._Processor = new Processor(new List<Instruction>());

            this.UpdateLatestSnapshotOfRegistryState();

            this.ListViewOfRegisters.ItemsSource = this.Registers;

            /*
            var updatedRegiser = this.Registry.GetRegister("$temp0");
            updatedRegiser.Base10Value = 5;

            this.Registry.SaveRegister(updatedRegiser);
            */

            this.UpdateLatestSnapshotOfRegistryState();
        }

        /// <summary>
        /// Compiler for BV Assembly Code
        /// </summary>
        private ICompile<Instruction> _BVCompiler = new BVCompiler();

        /// <summary>
        /// Validation Info for Code
        /// </summary>
        private ValidationInfo _Code_ValidationInfo = new ValidationInfo();

        /// <summary>
        /// Processor to process BV Assembly Instructions
        /// </summary>
        private IProcessor _Processor;

        /// <summary>
        /// Contain Information about Registers
        /// </summary>
        public ObservableCollection<Register> Registers = new ObservableCollection<Register>();

        /// <summary>
        /// Compiles BV Assembly Code
        /// </summary>
        /// <param name="linesOfCode"></param>
        public void Button_Click_CompileBVAssemblyCode(object sender, RoutedEventArgs e)
        {
            var rawCode = this.TextBox_Code.Text;

            this._Code_ValidationInfo = this._BVCompiler.ValidateCode(rawCode);

            this.UpdateViewOfCompileErrors();

            if (!this._Code_ValidationInfo.IsValid)
            {
                this.ShowNotificationsWindow(new List<string>() { $"There are Compile Issues. Check Compile Errors Output Tab!" });
            }
        }

        /// <summary>
        /// Gets and saves latest snapshot of registry cache. Updates Registry View
        /// </summary>
        public void UpdateLatestSnapshotOfRegistryState()
        {
            // Get Latest State Of Registers
            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Registers.Clear();

                var registerState = this._Processor.GetRegisters().ToList();

                if (registerState.Any())
                {
                    foreach (var register in registerState)
                    {
                        this.Registers.Add(register);
                    }
                }
            }));

        }

        /// <summary>
        /// Update View of Compile Errors
        /// </summary>
        public void UpdateViewOfCompileErrors()
        {
            var compileErrors = string.Join($"{string.Join(string.Empty, BVCompiler.LineDelimitter)}", this._Code_ValidationInfo.ValidationMessages);

            this.TextBlock_CompileErrors.Text = compileErrors;
        }

        /// <summary>
        /// Shows notifications
        /// </summary>
        /// <param name="notifications">Notifications to show</param>
        private void ShowNotificationsWindow(List<string> notifications)
        {
            if (notifications != null)
            {
                NotificationWindow notificationsWindow = new NotificationWindow()
                {
                    Notifications = notifications
                };
                Window window = new Window 
                {
                    Title = "Notifications Window",
                    Content = notificationsWindow,
                    Width = 500,
                    Height = 300,
                    MinHeight = 300,
                    MinWidth = 500,
                    MaxHeight = 300,
                    MaxWidth = 500,
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                window.ShowDialog();
            }
        }
    }
}
