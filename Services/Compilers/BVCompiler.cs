using BajanVincyAssembly.Models;
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
    /// Compiler for BV Assembly Code
    /// </summary>
    public class BVCompiler : ICompile<Instruction>
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="BVCompiler" class/>
        /// </summary>
        public BVCompiler()
        {

        }

        /// <summary>
        /// Line Delimitter for assembly code
        /// </summary>
        public static readonly string[] LineDelimitter = { "\r", "\n" };

        /// <summary>
        /// Comment Signature
        /// </summary>
        public static readonly string CommentSignarture = "//";

        /// <inheritdoc cref="ICompile{T}"/>
        public IEnumerable<Instruction> Compile(string code, bool onlyMipsInstructions = false)
        {
            IEnumerable<Instruction> instructions = new List<Instruction>();
            ValidationInfo codeValidationInfo = this.ValidateCode(code);

            if (codeValidationInfo.IsValid || (BVOperationValidationChecks.OnlyValidationMessagesAreMipsCodeDetections(codeValidationInfo) && onlyMipsInstructions))
            {
                IEnumerable<string> linesOfCode = this.GetLinesOfCodeWithNoComments(code);

                // Build Instructions from code and return them
                BVInstructionBuilder bvInstructionBuilder = new BVInstructionBuilder();
                instructions = linesOfCode.Select(lineOfCode => bvInstructionBuilder.BuildInstruction(lineOfCode));

                if (instructions.Any() && onlyMipsInstructions)
                {
                    instructions = instructions.Where(instruction => BVOperationInfo.MipsOperations.Exists(operation => operation == instruction.Operation));
                }
            }

            return instructions;
        }

        /// <inheritdoc cref="ICompile{T}"/>
        public ValidationInfo ValidateCode(string code)
        {
            ValidationInfo validationInfo = new ValidationInfo();

            IEnumerable<string> linesOfCode = this.GetLinesOfCodeWithNoComments(code);

            if (linesOfCode.Any())
            {
                BVOperationValidationChecks bvOperationValidationChecks = new BVOperationValidationChecks(linesOfCode);
                validationInfo = bvOperationValidationChecks.ValidationInfo.DeepClone();
            }

            return validationInfo;
        }

        /// <summary>
        /// Gets Lines of Code with no comments
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns>Collection of strings</returns>
        private IEnumerable<string> GetLinesOfCodeWithNoComments(string code)
        {
            IEnumerable<string> linesOfCode = new List<string>();

            if (!string.IsNullOrEmpty(code))
            {
                linesOfCode = code.Split(BVCompiler.LineDelimitter, StringSplitOptions.RemoveEmptyEntries);

                if (linesOfCode.Any())
                {
                    // Removes comment lines
                    linesOfCode = linesOfCode.Where(line =>
                    {
                        var lineTrimmed = line.Trim().ToLower();

                        var first2Characters = lineTrimmed.Substring(0, 2);

                        var lineIsAComment = string.Equals(first2Characters, BVCompiler.CommentSignarture, StringComparison.InvariantCultureIgnoreCase);

                        if (lineIsAComment)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }).ToList();

                    // Trims Spaces around code lines
                    if (linesOfCode.Any())
                    {
                        linesOfCode = linesOfCode.Select(line => line.Trim());
                    }
                }
            }

            return linesOfCode;
        }
    }
}