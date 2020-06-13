using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.ComputerArchitecture
{
    /// <summary>
    /// Information Regarding a Data Dependency Hazard I can create for others
    /// </summary>
    public class DataDependencyHazardForOthers
    {
        /// <summary>
        /// Register that can have a data dependency
        /// </summary>
        public string RegisterName { get; set; } = string.Empty;

        /// <summary>
        /// Stage at which Register value will be ready when there is no forwarding
        /// </summary>
        public PipelineStage StageAvailibity_NoForwarding { get; set; } = PipelineStage.WB;

        /// <summary>
        /// Stage at which Register value will be ready when there is forwarding
        /// </summary>
        public PipelineStage StageAvailibity_WithForwarding { get; set; } = PipelineStage.EX;
    }
}
