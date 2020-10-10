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
        /// <summary>
        /// Indicates if hardware forwarding is available
        /// </summary>
        private readonly bool _HardwareForwardingAvailable = false;

        public BVInstructionBuilder(bool hardwareForwardingAvailable = false)
        {
            this._HardwareForwardingAvailable = hardwareForwardingAvailable;
        }

        /// <summary>
        /// Builds a BV Instruction from a line of code
        /// </summary>
        /// <param name="lineOfCode"> raw lines of code </param>
        /// <returns></returns>
        public Instruction BuildInstruction(string lineOfCode)
        {
            Instruction.InstructionAddressPointer += 32;
            Instruction instruction = new Instruction(Instruction.InstructionAddressPointer);
            instruction.AssemblyStatement = lineOfCode;

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
                case BVOperation.LESSTHAN:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.LESSTHANPOS:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.LESSTHANCONST:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandImmediate = int.Parse(operationParts[3]);
                    break;
                case BVOperation.LESSTHANEQ:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.LESSTHANEQPOS:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.LESSTHANEQCONST:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandImmediate = int.Parse(operationParts[3]);
                    break;
                case BVOperation.MORETHAN:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.MORETHANPOS:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.MORETHANCONST:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandImmediate = int.Parse(operationParts[3]);
                    break;
                case BVOperation.MORETHANEQ:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.MORETHANEQPOS:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.MORETHANEQCONST:
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
                case BVOperation.GOTOMORETHAN:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.GOTOMORETHANCONST:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandImmediate = int.Parse(operationParts[3]);
                    break;
                case BVOperation.GOTOLESSTHAN:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    break;
                case BVOperation.GOTOLESSTHANCONST:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandImmediate = int.Parse(operationParts[3]);
                    break;
                case BVOperation.JUMPLABEL:
                    instruction.JumpLabel = operationParts[0].Substring(0, operationParts[0].Length - 1);
                    break;
                case BVOperation.MIPSADD:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    instruction.DataDependencyHazardForOthers = new DataDependencyHazardForOthers()
                    {
                        RegisterName = instruction.DestinationRegister,
                        StageAvailibity_NoForwarding = PipelineStage.WB,
                        StageAvailibity_WithForwarding = PipelineStage.EX
                    };
                    instruction.DataDependencyNeedsIHave = new DataDependencyNeedIHave()
                    {
                        RegisterNames = new List<string>() { instruction.OperandARegister, instruction.OperandBRegister },
                        WhatStageINeedMyDependencyNeedsMet_NoForwarding = PipelineStage.ID,
                        WhatStageINeedMyDependencyNeedsMet_WithForwarding = PipelineStage.ID
                    };
                    break;
                case BVOperation.MIPSSUB:
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = operationParts[2].Replace(",", "").Trim();
                    instruction.OperandBRegister = operationParts[3].Replace(",", "").Trim();
                    instruction.DataDependencyHazardForOthers = new DataDependencyHazardForOthers()
                    {
                        RegisterName = instruction.DestinationRegister,
                        StageAvailibity_NoForwarding = PipelineStage.WB,
                        StageAvailibity_WithForwarding = PipelineStage.EX
                    };
                    instruction.DataDependencyNeedsIHave = new DataDependencyNeedIHave()
                    {
                        RegisterNames = new List<string>() { instruction.OperandARegister, instruction.OperandBRegister },
                        WhatStageINeedMyDependencyNeedsMet_NoForwarding = PipelineStage.ID,
                        WhatStageINeedMyDependencyNeedsMet_WithForwarding = PipelineStage.ID
                    };
                    break;
                case BVOperation.MIPSLW:
                    string secondOperationPart = operationParts[2].Replace(",", "").Trim();
                    int indexOfFirstParenthesis = secondOperationPart.IndexOf("(");
                    int indexOfSecondParenthesis = secondOperationPart.IndexOf(")");
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = secondOperationPart.Substring(indexOfFirstParenthesis + 1, (indexOfSecondParenthesis - indexOfFirstParenthesis - 1)).Trim();
                    instruction.OperandImmediate = int.Parse(secondOperationPart.Substring(0, indexOfFirstParenthesis).Trim());
                    instruction.DataDependencyHazardForOthers = new DataDependencyHazardForOthers()
                    {
                        RegisterName = instruction.DestinationRegister,
                        StageAvailibity_NoForwarding = PipelineStage.WB,
                        StageAvailibity_WithForwarding = PipelineStage.MEM
                    };
                    instruction.DataDependencyNeedsIHave = new DataDependencyNeedIHave()
                    {
                        RegisterNames = new List<string>(),
                        WhatStageINeedMyDependencyNeedsMet_NoForwarding = PipelineStage.ID,
                        WhatStageINeedMyDependencyNeedsMet_WithForwarding = PipelineStage.MEM
                    };
                    break;
                case BVOperation.MIPSSW:
                    string secondOperationPart_ = operationParts[2].Replace(",", "").Trim();
                    int indexOfFirstParenthesis_ = secondOperationPart_.IndexOf("(");
                    int indexOfSecondParenthesis_ = secondOperationPart_.IndexOf(")");
                    instruction.DestinationRegister = operationParts[1].Replace(",", "").Trim();
                    instruction.OperandARegister = secondOperationPart_.Substring(indexOfFirstParenthesis_ + 1, (indexOfSecondParenthesis_ - indexOfFirstParenthesis_ - 1)).Trim();
                    instruction.OperandImmediate = int.Parse(secondOperationPart_.Substring(0, indexOfFirstParenthesis_).Trim());
                    instruction.DataDependencyHazardForOthers = new DataDependencyHazardForOthers();
                    instruction.DataDependencyNeedsIHave = new DataDependencyNeedIHave()
                    {
                        RegisterNames = new List<string>() { instruction.DestinationRegister, instruction.OperandARegister },
                        WhatStageINeedMyDependencyNeedsMet_NoForwarding = PipelineStage.ID,
                        WhatStageINeedMyDependencyNeedsMet_WithForwarding = PipelineStage.MEM
                    };
                    break;
            }

            return instruction;
        }
    }
}
