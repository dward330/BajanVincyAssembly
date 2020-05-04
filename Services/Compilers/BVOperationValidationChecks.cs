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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
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

            return ValidationInfo;
        }

    }
}
