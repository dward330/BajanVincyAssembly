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
        public Instruction()
        {

        }

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
    }
}
