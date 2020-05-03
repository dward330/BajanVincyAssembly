using BajanVincyAssembly.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Services.Registers
{
    /// <summary>
    /// Registry System
    /// </summary>
    public class Registry : IRegistry<Register>
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="Registry" class/>
        /// </summary>
        public Registry()
        {
            this.BuildRegistry();
        }

        /// <summary>
        /// Builds new Registry Cache
        /// </summary>
        private void BuildRegistry()
        {
            this.Registers = new Dictionary<string, Register>();

            this.Registers.Add("temp0", new Register("temp0"));
            this.Registers.Add("temp1", new Register("temp1"));
            this.Registers.Add("temp2", new Register("temp2"));
            this.Registers.Add("temp3", new Register("temp3"));
            this.Registers.Add("temp4", new Register("temp4"));
            this.Registers.Add("temp5", new Register("temp5"));
            this.Registers.Add("temp6", new Register("temp6"));
            this.Registers.Add("temp7", new Register("temp7"));
            this.Registers.Add("temp8", new Register("temp8"));
            this.Registers.Add("temp9", new Register("temp9"));
            this.Registers.Add("bv0", new Register("bv0"));
            this.Registers.Add("bv1", new Register("bv1"));
            this.Registers.Add("bv2", new Register("bv2"));
            this.Registers.Add("bv3", new Register("bv3"));
            this.Registers.Add("bv4", new Register("bv4"));
            this.Registers.Add("bv5", new Register("bv5"));
            this.Registers.Add("bv6", new Register("bv6"));
            this.Registers.Add("bv7", new Register("bv7"));
            this.Registers.Add("bv8", new Register("bv8"));
            this.Registers.Add("bv9", new Register("bv9"));
        }

        /// <summary>
        /// Gets or Sets Registers
        /// </summary>
        private Dictionary<string, Register> Registers;

        /// <inheritdoc cref="IRegistry{T}"/>
        public Register ClearRegister(string registerName)
        {
            Register register = null;

            if (this.Exists(registerName))
            {
                register = this.Registers[registerName];
                register.Clear();
                register = register.DeepClone();
            }

            return register;
        }

        /// <inheritdoc cref="IRegistry{T}"/>
        public bool Exists(string registerName)
        {
            bool registerExists = !string.IsNullOrEmpty(registerName) && this.Registers.ContainsKey(registerName);

            return registerExists;
        }

        /// <inheritdoc cref="IRegistry{T}"/>
        public Register GetRegister(string registerName)
        {
            Register register = null;

            if (this.Exists(registerName))
            {
                register = this.Registers[registerName];
                register = register.DeepClone();
            }

            return register;
        }

        /// <inheritdoc cref="IRegistry{T}"/>
        public IEnumerable<Register> GetRegisters()
        {
            var listOfRegisters = this.Registers.Values.ToList();
            var deepCloneOfRegisters = listOfRegisters.DeepClone();

            return deepCloneOfRegisters;
        }

        /// <inheritdoc cref="IRegistry{T}"/>
        public IEnumerable<Register> ResetRegisters()
        {
            foreach (var register in this.Registers)
            {
                register.Value.Clear();
            }

            var listOfRegisters = this.Registers.Values.ToList();
            var deepCloneOfRegisters = listOfRegisters.DeepClone();

            return deepCloneOfRegisters;
        }

        /// <inheritdoc cref="IRegistry{T}"/>
        public Register SaveRegister(Register register)
        {
            Register savedRegister = null;

            if (register != null
                && this.Exists(register.Name))
            {
                this.Registers[register.Name] = register;
                savedRegister = this.Registers[register.Name];
                savedRegister = savedRegister.DeepClone();
            }

            return savedRegister;
        }
    }
}
