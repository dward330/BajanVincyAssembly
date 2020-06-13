using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.ComputerArchitecture
{
    /// <summary>
    /// Contains Instruction To Timing Diagram Information
    /// </summary>
    public class InstructionTimingDiagram
    {

        public InstructionTimingDiagram()
        {
            
        }

        /// <summary>
        /// Assembly Statement
        /// </summary>
        public string AssemblyStatement { get; set; }

        /// <summary>
        /// Timing Diagrams
        /// </summary>
        public string TimingDiagram { get; set; }
    }
}
