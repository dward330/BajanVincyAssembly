using BajanVincyAssembly.Models;
using BajanVincyAssembly.Models.ComputerArchitecture;
using BajanVincyAssembly.Services.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Services.Processor
{
    /// <summary>
    /// Processes all BV Instructions
    /// </summary>
    public class Processor : IProcessor
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="Processor" class/>
        /// </summary>
        /// <param name="instructions"> instructions to process </param>
        /// <param name="hardwareForwardingAvailable"> Indicates if hardware forwarding is available </param>
        public Processor(IEnumerable<Instruction> instructions, bool hardwareForwardingAvailable = false)
        {
            this._Registry = new Registry();
            this._HardwareForwardingAvailable = hardwareForwardingAvailable;
            foreach (Instruction instruction in instructions)
            {
                this._Instructions.Add(instruction);
                this._ProcessorPipelineState.Add(instruction.InstructionAddress, new InstructionPipelineState() {
                    Instruction = instruction
                });
            }
        }

        /// <summary>
        /// Registry Management System
        /// </summary>
        private IRegistry<Register> _Registry;

        /// <summary>
        /// Collection of Instructions Processed
        /// </summary>
        private List<Instruction> _Instructions = new List<Instruction>();

        /// <summary>
        /// Program Instruction Pointer
        /// </summary>
        private int _ProgramInstructionPointer = 0;

        /// <summary>
        /// Updates Program Instruction Pointer
        /// </summary>
        /// <param name="instructionNumberToProcessNext">Instruction Number to processes next</param>
        private void SetProgramInstructionPointer(int instructionNumberToProcessNext)
        {
            this._ProgramInstructionPointer = instructionNumberToProcessNext;
        }

        /// <summary>
        /// Finds and returns Index of Jump Label Instruction
        /// </summary>
        /// <param name="jumpLabel"></param>
        /// <returns>integer</returns>
        private int FindIndexOfJumpLabelInstruction(string jumpLabel)
        {
            int instructionCounter = this._Instructions.ToList().FindIndex((instruct) => instruct.Operation == BVOperation.JUMPLABEL 
                                                                                         && string.Equals(instruct.JumpLabel, jumpLabel, StringComparison.InvariantCultureIgnoreCase));

            if (instructionCounter == -1)
            {
                throw new Exception($"Unknown Jump Label Found! -> ${jumpLabel}");
            }

            return instructionCounter;
        }

        /// <summary>
        /// Indicates if hardware forwarding is available
        /// </summary>
        private readonly bool _HardwareForwardingAvailable = false;

        /// <summary>
        /// Indicated if timing diagram analysis is all that needs to run
        /// </summary>
        private readonly bool _RunTimingDiagramAnalysisOnly = false;

        /// <summary>
        /// Current Pipeline State of the Processor
        /// </summary>
        private Dictionary<int, InstructionPipelineState> _ProcessorPipelineState = new Dictionary<int, InstructionPipelineState>();

        /// <inheritdoc cref="IProcessor"/>
        public IEnumerable<Register> GetRegisters()
        {
            return this._Registry.GetRegisters();
        }

        /// <inheritdoc cref="IProcessor"/>
        public IEnumerable<Instruction> GetInstructions()
        {
            var instructionsCopy = this._Instructions.DeepClone();

            return instructionsCopy;
        }

        /// <inheritdoc cref="IProcessor"/>
        public bool HasAnotherInstructionToProcess()
        {
            return this._ProgramInstructionPointer < this._Instructions.Count();
        }

        /// <inheritdoc cref="IProcessor"/>
        public void ProcessNextInstruction()
        {
            if (this._Instructions.Any())
            {
                Instruction instructionToProcess = this.GetNextInstruction();
                this.RunInstructionThroughPipeline(instructionToProcess);
            }
        }

        /// <inheritdoc cref="IProcessor"/>
        public void GenerateTimingAnalysisForInstructions()
        {
            var deepCopyOfAllInstructions = this._Instructions.DeepClone();

            int cycleCounter = 0;

            while (this._Instructions.Any())
            {
                IEnumerable<Instruction> instructions = this._Instructions.ToList().OrderBy(x => x.InstructionAddress);

                // Go to the next clock cycle
                foreach (Instruction currentInstruction in instructions)
                {
                    // Am I suppose to come into the pipeline now?
                    var previousInstruction = this.GetPreviousInstruction(currentInstruction);
                    if (previousInstruction != null && this._ProcessorPipelineState[previousInstruction.InstructionAddress].CurrentPipelineStage < PipelineStage.ID)
                    {
                        this._ProcessorPipelineState[currentInstruction.InstructionAddress].AddStallCycle();
                        continue;
                    }

                    // Remove this instruction from the pipeline if it was already at the end
                    if (this._ProcessorPipelineState[currentInstruction.InstructionAddress].CurrentPipelineStage == PipelineStage.WB)
                    {
                        // Mark instruction as processed
                        this._ProcessorPipelineState[currentInstruction.InstructionAddress].CurrentPipelineStage = PipelineStage.Processed;

                        // Find Intruction to remove from list of instructions still in the pipeline
                        var instructionToRemove = this._Instructions.Single(x => x.InstructionAddress == currentInstruction.InstructionAddress);
                        
                        // Removed the Instruction because it has been fully processed now
                        this._Instructions.Remove(instructionToRemove);

                        // Go to the next instruction
                        continue;
                    }

                    bool earlierInstructionExistInPipeline = instructions.ToList().Exists(x => x.InstructionAddress < currentInstruction.InstructionAddress);

                    if (!earlierInstructionExistInPipeline)
                    {
                        // Progress to the next stage of the pipeline
                        this._ProcessorPipelineState[currentInstruction.InstructionAddress].MoveToNextPipelineStage();
                    }
                    else // I am not the first or only instruction
                    {
                        // Are my needs met to proceed to the next pipeline stage?
                        bool dependencyNeedsMet = this.InstructionDataDependencyNeedsAreMetToMoveToNextCycle(currentInstruction);

                        if (!dependencyNeedsMet)
                        {
                            this._ProcessorPipelineState[currentInstruction.InstructionAddress].AddStallCycle();
                        }
                        else
                        {
                            this._ProcessorPipelineState[currentInstruction.InstructionAddress].MoveToNextPipelineStage();
                        }
                    }                    
                }

                cycleCounter++;
            }

            var finished = 0;
        }

        /// <inheritdoc cref="IProcessor"/>
        public Instruction GetNextInstruction()
        {
            Instruction instruction;

            instruction = this._Instructions.Skip(this._ProgramInstructionPointer).Take(1).FirstOrDefault();

            return instruction;
        }

        /// <summary>
        /// Returns the current processor pipeline state
        /// </summary>
        /// <returns>Dictionary of Processor Intruction State</returns>
        public Dictionary<int, InstructionPipelineState> GetProcessorPipelineState()
        {
            return this._ProcessorPipelineState.DeepClone();
        }

        /// <summary>
        /// Runs the supplied instruction through this pipelined processor
        /// </summary>
        /// <param name="instruction"></param>
        private void RunInstructionThroughPipeline(Instruction instruction)
        {
            Register destinationRegister;
            Register operandARegister;
            Register operandBRegister;
            int operandIntermediate;
            int instructionCounter;

            switch (instruction.Operation)
            {
                case BVOperation.ADDNS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value + operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.ADDCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value + operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.ADDPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value + operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SUBNS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value - operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SUBCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value - operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SUBPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value - operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LOGICAND:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value & operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LOGICANDCOSNT:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value & operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LOGICOR:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value & operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LOGICORCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value | operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SHIFTLEFT:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value << operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SHIFTLEFTPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value << operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SHIFTLEFTCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value << operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SHIFTRIGHT:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value >> operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SHIFTRIGHTPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value >> operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SHIFTRIGHTCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value >> operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.FROMEMEM:

                    break;
                case BVOperation.FROMMEMCONST:

                    break;
                case BVOperation.FROMCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.TOMEM:

                    break;
                case BVOperation.TOMEMCONST:

                    break;
                case BVOperation.TOCONST:

                    break;
                case BVOperation.TOCONSTCONST:

                    break;
                case BVOperation.COPY:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.COPYERASE:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    operandARegister.Base10Value = 0;
                    this._Registry.SaveRegister(operandARegister);
                    break;
                case BVOperation.LESSTHEN:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value < operandBRegister.Base10Value ? 1: 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LESSTHENPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value < operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LESSTHENCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value < operandIntermediate ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LESSTHENEQ:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value <= operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LESSTHENEQPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value <= operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.LESSTHENEQCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value <= operandIntermediate ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.MORETHEN:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value > operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.MORETHENPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value > operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.MORETHENCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value > operandIntermediate ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.MORETHENEQ:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value >= operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.MORETHENEQPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value >= operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.MORETHENEQCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value >= operandIntermediate ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.XOR:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value ^ operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.XORCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value ^ operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SAVEADDRESS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    destinationRegister.Base10Value = instruction.InstructionAddress;
                    destinationRegister.Word = instruction.JumpLabel;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.GOTO:
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    instructionCounter = this.FindIndexOfJumpLabelInstruction(operandARegister.Word);
                    this._ProgramInstructionPointer = instructionCounter + 1;                    
                    break;
                case BVOperation.EQ:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value == operandBRegister.Base10Value ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.EQCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value == operandIntermediate ? 1 : 0;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.GOTOEQ:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    if (operandARegister.Base10Value == operandBRegister.Base10Value)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTOEQCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    if (operandARegister.Base10Value == operandIntermediate)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTONOEQ:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    if (operandARegister.Base10Value != operandBRegister.Base10Value)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTONOEQCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    if (operandARegister.Base10Value != operandIntermediate)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTOMORETHEN:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    if (operandARegister.Base10Value >= operandBRegister.Base10Value)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTOMORETHENCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    if (operandARegister.Base10Value >= operandIntermediate)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTOLESSTHEN:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    if (operandARegister.Base10Value < operandBRegister.Base10Value)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTOLESSTHENCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    if (operandARegister.Base10Value < operandIntermediate)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
            }

            // Decide if we need to increment Instruction Pointer
            switch (instruction.Operation)
            {
                case BVOperation.GOTO:
                case BVOperation.GOTOEQ:
                case BVOperation.GOTOEQCONST:
                case BVOperation.GOTONOEQ:
                case BVOperation.GOTONOEQCONST:
                case BVOperation.GOTOMORETHEN:
                case BVOperation.GOTOMORETHENCONST:
                case BVOperation.GOTOLESSTHEN:
                case BVOperation.GOTOLESSTHENCONST:
                    break;
                default:
                    this._ProgramInstructionPointer++;
                    break;
            }
        }

        /// <summary>
        /// Indicates if an instructions data dependency needs are met to move to next cycle
        /// </summary>
        /// <param name="currentInstruction"> current instruction </param>
        /// <returns>Indicator if the Instruction as all its needs met at this point</returns>
        private bool InstructionDataDependencyNeedsAreMetToMoveToNextCycle(Instruction currentInstruction)
        {
            bool needsMet = true;

            if (currentInstruction.DataDependencyNeedsIHave.WhatStageINeedMyDependencyNeedsMet_NoForwarding == (this._ProcessorPipelineState[currentInstruction.InstructionAddress].CurrentPipelineStage + 1))
            {
                foreach (string register in currentInstruction.DataDependencyNeedsIHave.RegisterNames)
                {
                    // Find the latest instruction before this instruction that has this register as a data dependency for others
                    Instruction latestEarlierInstruction = null;
                    foreach (var instruction in this._ProcessorPipelineState)
                    {
                        if (instruction.Key >= currentInstruction.InstructionAddress)
                        {
                            continue;
                        }
                        else if ((latestEarlierInstruction == null || instruction.Key > latestEarlierInstruction.InstructionAddress)
                                 && string.Equals(instruction.Value.Instruction.DataDependencyHazardForOthers.RegisterName, register, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // We found an earlier and later instruction with our data dependency
                            latestEarlierInstruction = instruction.Value.Instruction;
                        }
                    }

                    // No earlier instruction with our data dependency was found
                    if (latestEarlierInstruction == null)
                    {
                        needsMet = needsMet && true;
                    }
                    else
                    {
                        var stateOflatestEarlierInstructionState = this._ProcessorPipelineState[latestEarlierInstruction.InstructionAddress];
                        var whenDataIsAvailableWithForwarding = stateOflatestEarlierInstructionState.Instruction.DataDependencyHazardForOthers.StageAvailibity_WithForwarding;
                        var whenDataIsAvailableWithNoForwarding = stateOflatestEarlierInstructionState.Instruction.DataDependencyHazardForOthers.StageAvailibity_NoForwarding;
                        var currentInstructionState = this._ProcessorPipelineState[currentInstruction.InstructionAddress];
                        var whenINeedDataAvailableWithForwarding = currentInstructionState.Instruction.DataDependencyNeedsIHave.WhatStageINeedMyDependencyNeedsMet_WithForwarding;
                        var whenINeedDataAvailableWithNoForwarding = currentInstructionState.Instruction.DataDependencyNeedsIHave.WhatStageINeedMyDependencyNeedsMet_NoForwarding;
                        needsMet = needsMet &&
                                    (
                                    (stateOflatestEarlierInstructionState.CurrentPipelineStage >=
                                        (this._HardwareForwardingAvailable ? whenDataIsAvailableWithForwarding : whenDataIsAvailableWithNoForwarding)) ||
                                        !((currentInstructionState.CurrentPipelineStage + 1) == (this._HardwareForwardingAvailable ? whenINeedDataAvailableWithForwarding : whenINeedDataAvailableWithNoForwarding))
                                    );
                    }
                }
            }

            return needsMet;
        }

        /// <summary>
        /// Get my previous instruction
        /// </summary>
        /// <param name="currentInstruction"></param>
        /// <returns></returns>
        private Instruction GetPreviousInstruction(Instruction currentInstruction)
        {
            // Find the latest instruction before this instruction that has this register as a data dependency for others
            Instruction latestEarlierInstruction = null;
            foreach (var instruction in this._ProcessorPipelineState)
            {
                if (instruction.Key >= currentInstruction.InstructionAddress)
                {
                    continue;
                }
                else if (latestEarlierInstruction == null || instruction.Key > latestEarlierInstruction.InstructionAddress)
                {
                    // We found an earlier and later instruction with our data dependency
                    latestEarlierInstruction = instruction.Value.Instruction;
                }
            }

            return latestEarlierInstruction;
        }
    }
}
