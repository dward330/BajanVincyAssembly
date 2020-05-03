﻿namespace BajanVincyAssembly.Models.Infrastructure
{
    /// <summary>
    /// Contains information about a register
    /// </summary>
    public class Register
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="Register" class/>
        /// </summary>
        /// <param name="name"></param>
        public Register(string name)
        {
            this.Name = name;
        }

        public Register()
        {

        }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Base 10 Value
        /// </summary>
        public int Base10Value { get; set; }

        /// <summary>
        /// Gets or sets Hex Value
        /// </summary>
        public string HexValue { get { return $"0X{string.Format("{0:x8}", this.Base10Value)}"; } }

        /// <summary>
        /// Sets Base 10 Value
        /// </summary>
        /// <param name="value"></param>
        public void SetBase10Value (int value)
        {
            this.Base10Value = value;
        }

        /// <summary>
        /// Clears Register
        /// </summary>
        public void Clear()
        {
            this.Base10Value = 0;
        }
    }
}
