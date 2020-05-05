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
        public Processor(IEnumerable<Instruction> instructions)
        {
            this._Registry = new Registry();
            this._Instructions = instructions;
        }

        /// <summary>
        /// Registry Management System
        /// </summary>
        private IRegistry<Register> _Registry;

        /// <summary>
        /// Collection of Instructions Processed
        /// </summary>
        private IEnumerable<Instruction> _Instructions = new List<Instruction>();

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
            int instructionCounter = this._Instructions.ToList().FindIndex((instruct) => string.Equals(instruct.JumpLabel, jumpLabel, StringComparison.InvariantCultureIgnoreCase));

            if (instructionCounter == -1)
            {
                throw new Exception($"Unknown Jump Label Found! -> ${jumpLabel}");
            }

            return instructionCounter;
        }

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
                Instruction instructionToProcess = this._Instructions.Skip(this._ProgramInstructionPointer).Take(1).FirstOrDefault();
                this.RunInstructionThroughPipeline(instructionToProcess);
            }
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
                    destinationRegister.Base10Value = operandARegister.Base10Value + operandBRegister.Base10Value;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SUBCONST:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandIntermediate = instruction.OperandImmediate;
                    destinationRegister.Base10Value = operandARegister.Base10Value + operandIntermediate;
                    this._Registry.SaveRegister(destinationRegister);
                    break;
                case BVOperation.SUBPOS:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    destinationRegister.Base10Value = operandARegister.Base10Value + operandBRegister.Base10Value;
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
                case BVOperation.GOTOMORETHAN:
                    destinationRegister = this._Registry.GetRegister(instruction.DestinationRegister);
                    operandARegister = this._Registry.GetRegister(instruction.OperandARegister);
                    operandBRegister = this._Registry.GetRegister(instruction.OperandBRegister);
                    if (operandARegister.Base10Value >= operandBRegister.Base10Value)
                    {
                        instructionCounter = this.FindIndexOfJumpLabelInstruction(destinationRegister.Word);
                        this._ProgramInstructionPointer = instructionCounter + 1;
                    }
                    break;
                case BVOperation.GOTOMORETHANCONST:
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
                case BVOperation.GOTOMORETHAN:
                case BVOperation.GOTOMORETHANCONST:
                case BVOperation.GOTOLESSTHEN:
                case BVOperation.GOTOLESSTHENCONST:
                    break;
                default:
                    this._ProgramInstructionPointer++;
                    break;
            }
        }

    }
}
