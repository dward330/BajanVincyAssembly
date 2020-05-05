using System.Collections.Generic;

namespace BajanVincyAssembly.Services.Registers
{
    /// <summary>
    /// Interface defintion for register operations
    /// </summary>
    public interface IRegistry<T>
    {
        /// <summary>
        /// Indicates if there exists a register with the supplied Name
        /// </summary>
        /// <param name="registerName">Register Name</param>
        /// <returns></returns>
        bool Exists(string registerName);

        /// <summary>
        /// Get Register with supplied Name
        /// </summary>
        /// <param name="registerName">Register Name</param>
        /// <returns>Register</returns>
        T GetRegister(string registerName);

        /// <summary>
        /// Save Register into Registry Cache
        /// </summary>
        /// <param name="register"> Register to save</param>
        /// <returns>Register</returns>
        T SaveRegister(T register);

        /// <summary>
        /// Resets a register
        /// </summary>
        /// <param name="registerName">Register Name</param>
        /// <returns>Register</returns>
        T ClearRegister(string registerName);

        /// <summary>
        /// Gets Registers
        /// </summary>
        /// <returns>Collection of Registers</returns>
        IEnumerable<T> GetRegisters();

        /// <summary>
        /// Resets all registers in Registry Cache
        /// </summary>
        /// <returns>Collection of Registers</returns>
        IEnumerable<T> ResetRegisters();
    }
}
