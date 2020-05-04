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
        private IEnumerable<Instruction> _Instructions;

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

        }
    }
}
