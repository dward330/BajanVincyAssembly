using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.ComputerArchitecture
{
    /// <summary>
    /// Information Regarding a Data Dependency Hazard I jave
    /// </summary>
    public class DataDependencyNeedIHave
    {
        /// <summary>
        /// Registers that I have a data dependency
        /// </summary>
        public List<string> RegisterNames { get; set; } = new List<string>();

        /// <summary>
        /// Stage at which Register Dependency Needs need to be met
        /// </summary>
        public PipelineStage WhatStageINeedMyDependencyNeedsMet { get; set; } = PipelineStage.IF;
    }
}
