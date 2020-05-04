using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.Validation
{
    /// <summary>
    /// Contains Validation Information
    /// </summary>
    public class ValidationInfo
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="ValidationInfo" class/>
        /// </summary>
        public ValidationInfo()
        { 
        }

        /// <summary>
        /// Gets or sets IsValid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets validation messages
        /// </summary>
        public IEnumerable<string> ValidationMessages { get; set; } = new List<string>();
    }
}
