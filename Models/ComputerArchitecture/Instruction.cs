using BajanVincyAssembly.Services.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.ComputerArchitecture
{
    /// <summary>
    /// Contains instruction information
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="Instruction" class/>
        /// </summary>
        public Instruction(int instructionAddress)
        {
            this.InstructionAddress = instructionAddress;
        }

        public static int InstructionAddressPointer = 200;

        /// <summary>
        /// Gets or sets Instruction Address
        /// </summary>
        public int InstructionAddress { get; set; }

        /// <summary>
        /// Gets or sets BV Operation
        /// </summary>
        public BVOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets destination register
        /// </summary>
        public string DestinationRegister { get; set; }

        /// <summary>
        /// Gets or sets destination value
        /// </summary>
        public int DestinationValue { get; set; }

        /// <summary>
        /// Operand A Register
        /// </summary>
        public string OperandARegister { get; set; }

        /// <summary>
        /// Gets or sets Operand A
        /// </summary>
        public int OperandA { get; set; }

        /// <summary>
        /// Operand B Register
        /// </summary>
        public string OperandBRegister { get; set; }

        /// <summary>
        /// Gets or sets Operand B
        /// </summary>
        public int OperandB { get; set; }

        /// <summary>
        /// Gets or sets Operand Immediate
        /// </summary>
        public int OperandImmediate { get; set; }

        /// <summary>
        /// Gets or sets Offset
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets jump label
        /// </summary>
        public string JumpLabel { get; set; }

        /// <summary>
        /// Gets or sets Assembly statement
        /// </summary>
        public string AssemblyStatement { get; set; }

        /// <summary>
        /// Data Dependency Hazard other instructions can have on this instruction
        /// </summary>
        public DataDependencyHazardForOthers DataDependencyHazardForOthers { get; set; } = new DataDependencyHazardForOthers();

        /// <summary>
        /// Data Dependency Needs I have
        /// </summary>
        public DataDependencyNeedIHave DataDependencyNeedsIHave { get; set; } = new DataDependencyNeedIHave();

        /// <summary>
        /// Registers this instruction depends on to execute
        /// </summary>
        public List<string> RegistersIDependedOnToExecuteInstruction = new List<string>();

        /// <summary>
        /// Prints Binary Format of Instruction
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{BVOperationInfo.BVOperationCodeLookup[this.Operation]} | { (!string.IsNullOrEmpty(DestinationRegister) ? Registry.registerAddressLookup[DestinationRegister] : Convert.ToString(0, 2).PadLeft(5, '0'))} | { (!string.IsNullOrEmpty(OperandARegister) ? Registry.registerAddressLookup[OperandARegister] : Convert.ToString(0, 2).PadLeft(5, '0'))} | { (!string.IsNullOrEmpty(OperandBRegister) ? Registry.registerAddressLookup[OperandBRegister] : Convert.ToString(0, 2).PadLeft(5, '0'))} | { Convert.ToString(OperandImmediate, 2).PadLeft(16, '0')}";
        }
    }
}
