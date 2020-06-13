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
        /// Stage at which Register Dependency Needs need to be met, with no forwarding
        /// </summary>
        public PipelineStage WhatStageINeedMyDependencyNeedsMet_NoForwarding { get; set; } = PipelineStage.IF;

        /// <summary>
        /// Stage at which Register Dependency Needs need to be met, with forwarding
        /// </summary>
        public PipelineStage WhatStageINeedMyDependencyNeedsMet_WithForwarding { get; set; } = PipelineStage.IF;
    }
}
