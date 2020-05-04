using BajanVincyAssembly.Models.ComputerArchitecture;
using BajanVincyAssembly.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Services.Compilers
{
    /// <summary>
    /// Interface definition for Compilers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ICompile<T>
    {
        /// <summary>
        /// Get Validation Information about supplied code
        /// </summary>
        /// <param name="code">Code to validate</param>
        /// <returns>Validation Info about code</returns>
        ValidationInfo ValidateCode(string code);

        /// <summary>
        /// Compiles Code and Returns collection of instructions
        /// </summary>
        /// <param name="code">Code to compile</param>
        /// <returns>Collection of Instructions</returns>
        IEnumerable<T> Compile(string code);
    }
}
