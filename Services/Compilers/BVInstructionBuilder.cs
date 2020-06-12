using BajanVincyAssembly.Models.ComputerArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Services.Compilers
{
    /// <summary>
    /// Transforms BV Assembly into Insructions
    /// </summary>
    public class BVInstructionBuilder
    {
        public BVInstructionBuilder()
        {

        }

        /// <summary>
        /// Builds a BV Instruction from a line of code
        /// </summary>
        /// <param name="lineOfCode"> raw lines of code </param>
        /// <param name="onlyMipsInstructions"> Indicated if we should only build the mips instructions </param>
        /// <returns></returns>
        public Instruction BuildInstruction(string lineOfCode, bool onlyMipsInstructions = false)
        {
            Instruction.InstructionAddressPointer += 32;
            Instruction instruction = new Instruction();

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                throw new Exception($"Invalid Instruction Found: -> ${lineOfCode}");
            }

            string operationFound = operationParts[0].Trim().ToLower();
            bool jumpLabelFound = BVOperationValidationChecks.JumpLabelregex.Match(operationFound).Success;

            if (!jumpLabelFound && !BVOperationInfo.BVOperationLookup.ContainsKey(operationFound))
            {
                throw new Exception( $"Invalid Operation Found: -> ${lineOfCode}");
            }

            BVOperation operation = jumpLabelFound ? BVOperation.JUMPLABEL : BVOperationInfo.BVOperationLookup[operationFound];

            instruction.Operation = operation;

            if (!onlyMipsInstructions)
            {
                switch (operation)
                {
                    case BVOperation.ADDNS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.ADDCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.ADDPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.SUBNS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.SUBCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.SUBPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LOGICAND:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LOGICANDCOSNT:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.LOGICOR:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LOGICORCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.SHIFTLEFT:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.SHIFTLEFTPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.SHIFTLEFTCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.SHIFTRIGHT:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.SHIFTRIGHTPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.SHIFTRIGHTCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.FROMEMEM:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.FROMMEMCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.FROMCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[2]);
                        break;
                    case BVOperation.TOMEM:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.TOMEMCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[3].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[2]);
                        break;
                    case BVOperation.TOCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.TOCONSTCONST:

                        break;
                    case BVOperation.COPY:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        break;
                    case BVOperation.COPYERASE:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        break;
                    case BVOperation.LESSTHEN:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LESSTHENPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LESSTHENCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.LESSTHENEQ:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LESSTHENEQPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.LESSTHENEQCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.MORETHEN:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.MORETHENPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.MORETHENCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.MORETHENEQ:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.MORETHENEQPOS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.MORETHENEQCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.XOR:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.XORCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.SAVEADDRESS:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.JumpLabel = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = instruction.InstructionAddress;
                        break;
                    case BVOperation.GOTO:
                        instruction.OperandARegister = operationParts[1].Replace(",", "").Trim();
                        break;
                    case BVOperation.EQ:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.EQCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.GOTOEQ:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.GOTOEQCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.GOTONOEQ:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.GOTONOEQCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.GOTOMORETHEN:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.GOTOMORETHENCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.GOTOLESSTHEN:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.GOTOLESSTHENCONST:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandImmediate = int.Parse(operationParts[3]);
                        break;
                    case BVOperation.JUMPLABEL:
                        instruction.JumpLabel = operationParts[0].Substring(0, operationParts[0].Length - 1);
                        break;
                }
            }
            else
            {
                switch (operation)
                {
                    case BVOperation.MIPSADD:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.MIPSSUB:
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                        instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                        break;
                    case BVOperation.MIPSLW:
                        string secondOperationPart = operationParts[2].Replace(",", "").Trim();
                        int indexOfFirstParenthesis = secondOperationPart.IndexOf("(");
                        int indexOfSecondParenthesis = secondOperationPart.IndexOf(")");
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = secondOperationPart.Substring(indexOfFirstParenthesis + 1, (indexOfSecondParenthesis - indexOfFirstParenthesis - 1)).Trim();
                        instruction.OperandImmediate = int.Parse(secondOperationPart.Substring(0, indexOfFirstParenthesis).Trim());
                        break;
                    case BVOperation.MIPSSW:
                        string secondOperationPart_ = operationParts[2].Replace(",", "").Trim();
                        int indexOfFirstParenthesis_ = secondOperationPart_.IndexOf("(");
                        int indexOfSecondParenthesis_ = secondOperationPart_.IndexOf(")");
                        instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                        instruction.OperandARegister = secondOperationPart_.Substring(indexOfFirstParenthesis_ + 1, (indexOfSecondParenthesis_ - indexOfFirstParenthesis_ - 1)).Trim();
                        instruction.OperandImmediate = int.Parse(secondOperationPart_.Substring(0, indexOfFirstParenthesis_).Trim());
                        break;
                }
            }

            return instruction;
        }
    }
}
