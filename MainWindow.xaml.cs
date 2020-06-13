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

            this.OuputMessages.Add($"Initializing IDE...");
            this.UpdateViewOfOuputMessages();

            this.Reset();

            this.ListViewOfRegisters.ItemsSource = this.Registers;
            this.ListViewOfTimingDiagram.ItemsSource = this.InstructionTimingDiagrams;
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
        /// Instructions loaded in the processor
        /// </summary>
        public ObservableCollection<Instruction> ProcessorInstructions = new ObservableCollection<Instruction>();

        /// <summary>
        /// Ouput Messages
        /// </summary>
        public ObservableCollection<string> OuputMessages = new ObservableCollection<string>();

        /// <summary>
        /// Instruction Timing Diagrams
        /// </summary>
        public ObservableCollection<InstructionTimingDiagram> InstructionTimingDiagrams = new ObservableCollection<InstructionTimingDiagram>();

        /// <summary>
        /// Compiles BV Assembly Code
        /// </summary>
        /// <param name="linesOfCode"></param>
        public void Button_Click_CompileBVAssemblyCode(object sender, RoutedEventArgs e)
        {
            this.Reset();

            var rawCode = this.TextBox_Code.Text;

            this._Code_ValidationInfo = this._BVCompiler.ValidateCode(rawCode);

            this.UpdateViewOfCompileErrors();

            if (!this._Code_ValidationInfo.IsValid && !BVOperationValidationChecks.OnlyValidationMessagesAreMipsCodeDetections(this._Code_ValidationInfo))
            {
                this.ShowNotificationsWindow(new List<string>() { $"There are compile Issues. Check 'Compile Errors' Tab!" });
            }
            else if (BVOperationValidationChecks.OnlyValidationMessagesAreMipsCodeDetections(this._Code_ValidationInfo))
            {
                // Generate/Build Instructions
                IEnumerable<Instruction> compiledInstructions = this._BVCompiler.Compile(rawCode, true);
                this._Processor = new Processor(compiledInstructions, false);

                this.UpdateLatestSnapshotOfProcessorInstructions();

                if (this.ProcessorInstructions.Any())
                {
                    // Jump onto Main UI Thread
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Button_Compile.IsEnabled = false;
                        this.Button_RunAll.IsEnabled = false;
                        this.Button_Debug.IsEnabled = false;
                        this.Button_DebugNext.IsEnabled = false;
                        this.Button_Stop.IsEnabled = true;
                        this.Button_RunMipsAnalysis_NoForward.IsEnabled = true;
                        this.Button_RunMipsAnalysis_Forward.IsEnabled = true;
                    }));
                }
            }
            else
            {
                // Generate/Build Instructions
                IEnumerable<Instruction> compiledInstructions = this._BVCompiler.Compile(rawCode);
                this._Processor = new Processor(compiledInstructions);

                this.UpdateLatestSnapshotOfProcessorInstructions();

                if (this.ProcessorInstructions.Any())
                {
                    // Jump onto Main UI Thread
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Button_Compile.IsEnabled = false;
                        this.Button_RunAll.IsEnabled = true;
                        this.Button_Debug.IsEnabled = true;
                        this.Button_DebugNext.IsEnabled = true;
                        this.Button_Stop.IsEnabled = true;
                        this.Button_RunMipsAnalysis_NoForward.IsEnabled = true;
                        this.Button_RunMipsAnalysis_Forward.IsEnabled = true;
                    }));
                }
            }
        }

        /// <summary>
        /// Begins Running all the user's code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click_RunAll(object sender, RoutedEventArgs e)
        {
            this.OuputMessages.Add($"Running all code...");
            this.UpdateViewOfOuputMessages();

            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Button_Compile.IsEnabled = false;
                this.Button_RunAll.IsEnabled = false;
                this.Button_Debug.IsEnabled = false;
                this.Button_DebugNext.IsEnabled = false;
                this.Button_RunMipsAnalysis_NoForward.IsEnabled = false;
                this.Button_RunMipsAnalysis_Forward.IsEnabled = false;
                this.Button_Stop.IsEnabled = true;
            }));

            while (this._Processor.HasAnotherInstructionToProcess())
            {
                try
                {
                    Instruction nextInstruction = this._Processor.GetNextInstruction();
                    this.OuputMessages.Add($"Running next Insruction...${Enum.GetName(typeof(BVOperation), nextInstruction.Operation)} -> {nextInstruction.ToString()}");
                    this.UpdateViewOfOuputMessages();
                    this._Processor.ProcessNextInstruction();
                    this.UpdateLatestSnapshotOfRegistryState();
                }
                catch (Exception exception)
                {
                    this.UpdateViewOfRunTimeErrors(new List<string>() { exception.Message });

                    // Jump onto Main UI Thread
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Button_Compile.IsEnabled = true;
                        this.Button_RunAll.IsEnabled = false;
                        this.Button_Debug.IsEnabled = false;
                        this.Button_DebugNext.IsEnabled = false;
                        this.Button_RunMipsAnalysis_NoForward.IsEnabled = false;
                        this.Button_RunMipsAnalysis_Forward.IsEnabled = false;
                        this.Button_Stop.IsEnabled = false;
                    }));

                    this.ShowNotificationsWindow(new List<string>() { $"There are Run Time Issues. Check 'Run Time Errors' Tab!" });
                }
            }

            this.OuputMessages.Add($"Finished Executing all code...");
            this.UpdateViewOfOuputMessages();
        }

        /// <summary>
        /// Begins Debugging Program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click_Debug(object sender, RoutedEventArgs e)
        {
            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Button_Compile.IsEnabled = false;
                this.Button_RunAll.IsEnabled = false;
                this.Button_Debug.IsEnabled = false;
                this.Button_RunMipsAnalysis_NoForward.IsEnabled = false;
                this.Button_RunMipsAnalysis_Forward.IsEnabled = false;
                this.Button_DebugNext.IsEnabled = true;
                this.Button_Stop.IsEnabled = true;
            }));

            if (this._Processor.HasAnotherInstructionToProcess())
            {
                Instruction nextInstruction = this._Processor.GetNextInstruction();
                this.OuputMessages.Add($"Running next Insruction...${Enum.GetName(typeof(BVOperation), nextInstruction.Operation)} -> {nextInstruction.ToString()}");
                this.UpdateViewOfOuputMessages();

                try
                {
                    this._Processor.ProcessNextInstruction();
                    this.UpdateLatestSnapshotOfRegistryState();
                }
                catch (Exception exception)
                {
                    this.UpdateViewOfRunTimeErrors(new List<string>() { exception.Message });

                    // Jump onto Main UI Thread
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Button_Compile.IsEnabled = true;
                        this.Button_RunAll.IsEnabled = false;
                        this.Button_Debug.IsEnabled = false;
                        this.Button_DebugNext.IsEnabled = false;
                        this.Button_Stop.IsEnabled = false;
                        this.Button_RunMipsAnalysis_NoForward.IsEnabled = false;
                        this.Button_RunMipsAnalysis_Forward.IsEnabled = false;
                    }));

                    this.ShowNotificationsWindow(new List<string>() { $"There are Run Time Issues. Check 'Run Time Errors' Tab!" });
                }
            }

            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Button_DebugNext.IsEnabled = this._Processor.HasAnotherInstructionToProcess() ? true : false;
            }));
        }

        /// <summary>
        /// Stops Running Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            this.Reset();
        }

        /// <summary>
        /// Run Mips Analysis (Generate Timing Diagram) for hardware with no forwarding available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click_RunMipsTimeAnalysisWithNoForwarding(object sender, RoutedEventArgs e)
        {
            var rawCode = this.TextBox_Code.Text;

            // Generate/Build Instructions
            IEnumerable<Instruction> compiledInstructions = this._BVCompiler.Compile(rawCode, true);
            this._Processor = new Processor(compiledInstructions, false);

            this.UpdateLatestSnapshotOfProcessorInstructions();

            if (this.ProcessorInstructions.Any())
            {
                this._Processor.GenerateTimingAnalysisForInstructions();
                var processorState = this._Processor.GetProcessorPipelineState();

                // Jump onto Main UI Thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.TextBlock_TimingDiagramHeader.Text = "With Hardware Forwarding:";
                    this.InstructionTimingDiagrams.Clear();
                    foreach (var instructionState in processorState)
                    {
                        this.InstructionTimingDiagrams.Add(new InstructionTimingDiagram()
                        {
                            AssemblyStatement = instructionState.Value.Instruction.AssemblyStatement,
                            TimingDiagram = instructionState.Value.ProcessingTimingDiagram
                        });
                    }
                }));
            }
        }

        /// <summary>
        /// Run Mips Analysis (Generate Timing Diagram) for hardware with forwarding available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click_RunMipsTimeAnalysisWithForwarding(object sender, RoutedEventArgs e)
        {
            var rawCode = this.TextBox_Code.Text;

            // Generate/Build Instructions
            IEnumerable<Instruction> compiledInstructions = this._BVCompiler.Compile(rawCode, true);
            this._Processor = new Processor(compiledInstructions, true);

            this.UpdateLatestSnapshotOfProcessorInstructions();

            if (this.ProcessorInstructions.Any())
            {
                this._Processor.GenerateTimingAnalysisForInstructions();
                var processorState = this._Processor.GetProcessorPipelineState();

                // Jump onto Main UI Thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.TextBlock_TimingDiagramHeader.Text = "With Hardware Forwarding:";
                    this.InstructionTimingDiagrams.Clear();
                    foreach (var instructionState in processorState)
                    {
                        this.InstructionTimingDiagrams.Add(new InstructionTimingDiagram()
                        {
                            AssemblyStatement = instructionState.Value.Instruction.AssemblyStatement,
                            TimingDiagram = instructionState.Value.ProcessingTimingDiagram
                        });
                    }
                }));
            }
        }

        /// <summary>
        /// Resets IDE State
        /// </summary>
        private void Reset()
        {
            this._Processor = new Processor(this.ProcessorInstructions);
            this._Code_ValidationInfo = new ValidationInfo();
            this.OuputMessages = new ObservableCollection<string>();
            this.OuputMessages.Add($"Resetting IDE...");
            this.UpdateLatestSnapshotOfRegistryState();
            this.UpdateViewOfCompileErrors();
            this.UpdateViewOfRunTimeErrors(new List<string>() { string.Empty });
            this.UpdateViewOfOuputMessages();

            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Button_Compile.IsEnabled = true;
                this.Button_RunAll.IsEnabled = false;
                this.Button_Debug.IsEnabled = false;
                this.Button_DebugNext.IsEnabled = false;
                this.Button_Stop.IsEnabled = false;
                this.Button_RunMipsAnalysis_NoForward.IsEnabled = false;
                this.Button_RunMipsAnalysis_Forward.IsEnabled = false;
            }));
        }

        /// <summary>
        /// Gets and saves latest snapshot of registry cache. Updates Registry View
        /// </summary>
        private void UpdateLatestSnapshotOfRegistryState()
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
        private void UpdateViewOfCompileErrors()
        {
            var compileErrors = string.Join($"{string.Join(string.Empty, BVCompiler.LineDelimitter)}", this._Code_ValidationInfo.ValidationMessages);

            this.TextBlock_CompileErrors.Text = compileErrors;
        }

        /// <summary>
        /// Update View of Run Time Errors
        /// </summary>
        /// <param name="runTimeErrors"></param>
        private void UpdateViewOfRunTimeErrors(List<string> runTimeErrors)
        {
            var runTimeErrorsStr = string.Join($"{string.Join(string.Empty, BVCompiler.LineDelimitter)}", runTimeErrors);

            this.TextBlock_RunTimeErrors.Text = runTimeErrorsStr;
        }

        /// <summary>
        /// Update View of Output Messages
        /// </summary>
        private void UpdateViewOfOuputMessages()
        {
            var outputMessages = string.Join($"{string.Join(string.Empty, BVCompiler.LineDelimitter)}", this.OuputMessages);

            this.TextBlock_Ouput.Text = outputMessages;
        }

        /// <summary>
        /// Gets and saves latest snapshot of Processor Instructions. Updates Porcessor Instructions
        /// </summary>
        private void UpdateLatestSnapshotOfProcessorInstructions()
        {
            // Get Latest State Of Processor Instructions
            // Jump onto Main UI Thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.ProcessorInstructions.Clear();

                var processorStateOfInstructions = this._Processor.GetInstructions().ToList();

                if (processorStateOfInstructions.Any())
                {
                    foreach (var instruction in processorStateOfInstructions)
                    {
                        this.ProcessorInstructions.Add(instruction);
                    }
                }
            }));
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
