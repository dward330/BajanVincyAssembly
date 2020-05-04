using BajanVincyAssembly.Models;
using BajanVincyAssembly.Models.ComputerArchitecture;
using BajanVincyAssembly.Models.Validation;
using BajanVincyAssembly.Services.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Services.Compilers
{
    /// <summary>
    /// Runs BV Operation Validation Checks
    /// </summary>
    public class BVOperationValidationChecks
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="BVOperationValidationChecks" class/>
        /// </summary>
        /// <param name="linesOfCode">Lines of Code</param>
        public BVOperationValidationChecks(IEnumerable<string> linesOfCode)
        {
            this._LinesOfCode = linesOfCode.DeepClone();

            this.ValidateLinesOfCode(this._LinesOfCode);
        }

        /// <summary>
        /// Lines of Code to validate
        /// </summary>
        private readonly IEnumerable<string> _LinesOfCode;

        /// <summary>
        /// Register Cache
        /// </summary>
        private readonly IRegistry<Register> _Registry = new Registry();

        /// <summary>
        /// Validation Info for all lines of code
        /// </summary>
        public ValidationInfo ValidationInfo { get; private set; } = new ValidationInfo();

        /// <summary>
        /// Validates Lines of Code and Updates Validation Info
        /// </summary>
        /// <param name="linesOfCode">Lines of Code to Validate</param>
        private void ValidateLinesOfCode(IEnumerable<string> linesOfCode)
        {
            if (!linesOfCode.Any())
            {
                return;
            }

            foreach (string lineOfCode in linesOfCode)
            {
                string[] operationPartsSplitter = { " " };
                var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

                if (!operationParts.Any())
                {
                    this.UpdateValidationInfo(false, new List<string>() { $"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}" });
                    continue;
                }

                string operationFound = operationParts[0].Trim().ToLower();

                if (!BVOperationInfo.BVOperationLookup.ContainsKey(operationFound))
                {
                    this.UpdateValidationInfo(false, new List<string>() { $"Invalid Operation Found: -> ${lineOfCode}" });
                    continue;
                }

                BVOperation operation = BVOperationInfo.BVOperationLookup[operationFound];

                ValidationInfo lineOfCode_ValidationInfo = new ValidationInfo();

                switch (operation)
                {
                    case BVOperation.ADDNS:
                        lineOfCode_ValidationInfo = this.RunADDNSInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.ADDCONST:
                        lineOfCode_ValidationInfo = this.RunADDConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.ADDPOS:
                        lineOfCode_ValidationInfo = this.RunADDPOSInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SUBNS:
                        lineOfCode_ValidationInfo = this.RunSUBNSInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SUBCONST:
                        lineOfCode_ValidationInfo = this.RunSubConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SUBPOS:
                        lineOfCode_ValidationInfo = this.RunSUBPOSInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICAND:
                        lineOfCode_ValidationInfo = this.RunLogicAndInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICANDCOSNT:
                        lineOfCode_ValidationInfo = this.RunLogicAndConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICOR:
                        lineOfCode_ValidationInfo = this.RunLogicOrInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICORCONST:
                        lineOfCode_ValidationInfo = this.RunLogicOrConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTLEFT:
                        lineOfCode_ValidationInfo = this.RunShiftLeftInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTLEFTPOS:
                        lineOfCode_ValidationInfo = this.RunShiftLeftPosInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTLEFTCONST:
                        lineOfCode_ValidationInfo = this.RunShiftLeftConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTRIGHT:
                        lineOfCode_ValidationInfo = this.RunShiftRightInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTRIGHTPOS:
                        lineOfCode_ValidationInfo = this.RunShiftRightPosInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTRIGHTCONST:
                        lineOfCode_ValidationInfo = this.RunShiftRightConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.FROMEMEM:
                        lineOfCode_ValidationInfo = this.RunFromMemInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.FROMMEMCONST:
                        lineOfCode_ValidationInfo = this.RunFromMemConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.FROMCONST:
                        lineOfCode_ValidationInfo = this.RunFromConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.TOMEM:
                        lineOfCode_ValidationInfo = this.RunToMemInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.TOMEMCONST:
                        lineOfCode_ValidationInfo = this.RunToMemConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.TOCONST:
                        lineOfCode_ValidationInfo = this.RunToConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.TOCONSTCONST:
                        lineOfCode_ValidationInfo = this.RunToConstConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.COPY:
                        lineOfCode_ValidationInfo = this.RunCopyInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.COPYERASE:
                        lineOfCode_ValidationInfo = this.RunCopyEraseInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHEN:
                        lineOfCode_ValidationInfo = this.RunLessThenInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENPOS:
                        lineOfCode_ValidationInfo = this.RunLessThenPosInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENCONST:
                        lineOfCode_ValidationInfo = this.RunLessThenConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENEQ:
                        lineOfCode_ValidationInfo = this.RunLessThenEqInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENEQPOS:
                        lineOfCode_ValidationInfo = this.RunLessThenEqPosInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENEQCONST:
                        lineOfCode_ValidationInfo = this.RunLessThenEqConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHEN:
                        lineOfCode_ValidationInfo = this.RunMoreThenInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENPOS:
                        lineOfCode_ValidationInfo = this.RunMoreThenPosInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENCONST:
                        lineOfCode_ValidationInfo = this.RunMoreThenConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENEQ:
                        lineOfCode_ValidationInfo = this.RunMoreThenEqInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENEQPOS:
                        lineOfCode_ValidationInfo = this.RunMoreThenEqPosInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENEQCONST:
                        lineOfCode_ValidationInfo = this.RunMoreThenEqConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.XOR:
                        lineOfCode_ValidationInfo = this.RunXORInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.XORCONST:
                        lineOfCode_ValidationInfo = this.RunXORConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.SAVEADDRESS:
                        lineOfCode_ValidationInfo = this.RunSaveAddressInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTO:
                        lineOfCode_ValidationInfo = this.RunGoToInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.EQ:
                        lineOfCode_ValidationInfo = this.RunEqInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.EQCONST:
                        lineOfCode_ValidationInfo = this.RunEqConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOEQ:
                        lineOfCode_ValidationInfo = this.RunGoToEqInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOEQCONST:
                        lineOfCode_ValidationInfo = this.RunGoToEqConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTONOEQ:
                        lineOfCode_ValidationInfo = this.RunGoToNoEqInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTONOEQCONST:
                        lineOfCode_ValidationInfo = this.RunGoToNoEqConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOMORETHAN:
                        lineOfCode_ValidationInfo = this.RunGoToMoreThenInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOMORETHANCONST:
                        lineOfCode_ValidationInfo = this.RunGoToMoreThenConstInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOLESSTHEN:
                        lineOfCode_ValidationInfo = this.RunGoToLessThenInstructionValidtionCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOLESSTHENCONST:
                        lineOfCode_ValidationInfo = this.RunGoToLessThenConstInstructionValidtionCheck(lineOfCode);
                        break;
                }

                this.UpdateValidationInfo(lineOfCode_ValidationInfo.IsValid, lineOfCode_ValidationInfo.ValidationMessages);
            }
        }

        /// <summary>
        /// Updates Validation Info
        /// </summary>
        /// <param name="isValid">validity to logicall append</param>
        /// <param name="validationMessage">Validation Message to Add</param>
        private void UpdateValidationInfo(bool isValid, List<string> validationMessage)
        {
            this.ValidationInfo.IsValid = this.ValidationInfo.IsValid && isValid;
            this.ValidationInfo.ValidationMessages.AddRange(validationMessage);
        }

        /// <summary>
        /// Runs Instruction Validation Check for AddNS
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunADDNSInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "addns";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for AddConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunADDConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "addconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for AddPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunADDPOSInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "addpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SubNS
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSUBNSInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "subns";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SubConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSubConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "subconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SubPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSUBPOSInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "subpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicAnd
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicAndInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "logicand";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicAndConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicAndConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "logicandconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicOr
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicOrInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "logicor";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicOrConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicOrConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "logicorconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftLeft
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftLeftInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "shiftleft";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftLeftPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftLeftPosInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "shiftleftpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftLeftConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftLeftConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "shiftleftconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftRight
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftRightInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "shiftright";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftRightPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftRightPosInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "shiftrightpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftRightConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftRightConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "shiftrightconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for FromMem
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunFromMemInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "frommem";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for FromMemConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunFromMemConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "frommemconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for FromConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunFromConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "fromconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 3)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToMem
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToMemInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "tomem";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToMemConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToMemConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "tomemconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 5, #2");
            }

            int num = 0;
            if (!int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 5, #2");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "toconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToConstConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToConstConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "toconstconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 2, 5");
            }
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, 2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for copy
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunCopyInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "copy";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 3)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for CopyErase
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunCopyEraseInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "copyerase";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 3)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for Lessthen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthen";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenPosInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthenpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthenconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenEqInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lesstheneq";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenEqPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenEqPosInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lesstheneqpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenEqConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lesstheneqconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for Morethen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethen";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenPosInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethenpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethenconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenEqInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "moretheneq";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenEqPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenEqPosInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "moretheneqpos";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenEqConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "moretheneqconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for XOR
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunXORInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "xor";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for XORConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunXORConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "xorconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SaveAddress
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSaveAddressInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "saveaddress";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 2)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} labelName");
            }

            int num = 0;
            if (int.TryParse(operationParts[1], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Number Found (${operationParts[1]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} labelName");
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoTo
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "goto";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 2)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1");
            }

            int num = 0;
            if (!int.TryParse(operationParts[1], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[1]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for Eq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunEqInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "eq";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for EqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunEqConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "eqconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToEqInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotoeq";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToEqConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotoeqconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToNoEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToNoEqInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotonoeq";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToNoEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToNoEqConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotonoeqconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToMorethen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToMoreThenInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotomorethen";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToMoreThenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToMoreThenConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotomorethenconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToLessthen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToLessThenInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotolessthen";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToLessThenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToLessThenConstInstructionValidtionCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotolessthenconst";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 4)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            int num = 0;
            if (!int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> ${operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found (${operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

    }
}
