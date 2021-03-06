﻿using BajanVincyAssembly.Models.ComputerArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Services.Processor
{
    /// <summary>
    /// Interface definition for processor operations
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Gets a snapshot of the current collection of registers
        /// </summary>
        /// <returns>Collection of registers</returns>
        IEnumerable<Register> GetRegisters();

        /// <summary>
        /// Gets all the instructions
        /// </summary>
        /// <returns>Collection of Instructions</returns>
        IEnumerable<Instruction> GetInstructions();

        /// <summary>
        /// Indicates if there is another instruction to process
        /// </summary>
        /// <returns>True if there is another instruction to process</returns>
        bool HasAnotherInstructionToProcess();

        /// <summary>
        /// Processes the next instruction
        /// </summary>
        void ProcessNextInstruction();

        /// <summary>
        /// Gets Next Instruction
        /// </summary>
        /// <returns></returns>
        Instruction GetNextInstruction();

        /// <summary>
        /// Generates Complete Timing Diagram for all Instructions
        /// </summary>
        void GenerateTimingAnalysisForInstructions();

        /// <summary>
        /// Returns the current processor pipeline state
        /// </summary>
        /// <returns>Dictionary of Processor Intruction State</returns>
        Dictionary<int, InstructionPipelineState> GetProcessorPipelineState();

        /// <summary>
        /// Return Dependency Hazards Detected by the Processor
        /// </summary>
        /// <returns></returns>
        List<string> GetDependencyHazards();
    }
}
