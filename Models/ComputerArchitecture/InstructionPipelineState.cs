using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.ComputerArchitecture
{
    /// <summary>
    /// All Pipeline Stages
    /// </summary>
    public enum PipelineStage
    {
        X = 0,
        IF = 1,
        ID = 2,
        EX = 3,
        MEM = 4,
        WB = 5,
        Processed = 6
    }

    /// <summary>
    /// State of an Instruction in the Pipeline
    /// </summary>
    public class InstructionPipelineState
    {
        /// <summary>
        /// Current Instruction
        /// </summary>
        public Instruction Instruction { get; set; }

        /// <summary>
        /// Current Stage of the pipeline the Instruction is at
        /// </summary>
        public PipelineStage CurrentPipelineStage { get; set; } = PipelineStage.X;

        /// <summary>
        /// Current Timing Diagram of Instruction
        /// </summary>
        public string ProcessingTimingDiagram { get; set; } = "";

        /// <summary>
        /// Updates Meta-data about instruction to reflect it has moved to the next pipeline stage
        /// </summary>
        public void MoveToNextPipelineStage()
        {
            this.CurrentPipelineStage++;
            this.ProcessingTimingDiagram += $"-{Enum.GetName(typeof(PipelineStage), this.CurrentPipelineStage)}";
        }

        /// <summary>
        /// Updates Meta-data about instruction to reflect it has been stalled this cycle
        /// </summary>
        public void AddStallCycle()
        {
            this.ProcessingTimingDiagram += "-X";
        }
    }
}
