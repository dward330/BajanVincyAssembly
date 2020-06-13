using BajanVincyAssembly.Models;
using BajanVincyAssembly.Models.ComputerArchitecture;
using BajanVincyAssembly.Models.Validation;
using BajanVincyAssembly.Services.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// Jump Label Regex
        /// </summary>
        public static readonly Regex JumpLabelregex = new Regex(@"[a-zA-Z0-9]+:");

        /// <summary>
        /// Letters Only Regex
        /// </summary>
        public static readonly Regex LettersOnlyregex = new Regex(@"[a-zA-Z0-9]+");

        /// <summary>
        /// Mips Instruction Detection Message Prefix
        /// </summary>
        public static readonly string MIPS_INSTRUCTION_DETECTED_MESSAGE_PREFIX = "Mips Instruction Detected";

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
                bool jumpLabelFound = JumpLabelregex.Match(operationFound).Success;

                if (!BVOperationInfo.BVOperationLookup.ContainsKey(operationFound) && !jumpLabelFound)
                {
                    this.UpdateValidationInfo(false, new List<string>() { $"Invalid Operation Found: -> {lineOfCode}" });
                    continue;
                }

                BVOperation operation = jumpLabelFound ? BVOperation.JUMPLABEL : BVOperationInfo.BVOperationLookup[operationFound];

                ValidationInfo lineOfCode_ValidationInfo = new ValidationInfo();

                switch (operation)
                {
                    case BVOperation.ADDNS:
                        lineOfCode_ValidationInfo = this.RunADDNSInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.ADDCONST:
                        lineOfCode_ValidationInfo = this.RunADDConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.ADDPOS:
                        lineOfCode_ValidationInfo = this.RunADDPOSInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SUBNS:
                        lineOfCode_ValidationInfo = this.RunSUBNSInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SUBCONST:
                        lineOfCode_ValidationInfo = this.RunSubConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SUBPOS:
                        lineOfCode_ValidationInfo = this.RunSUBPOSInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICAND:
                        lineOfCode_ValidationInfo = this.RunLogicAndInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICANDCOSNT:
                        lineOfCode_ValidationInfo = this.RunLogicAndConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICOR:
                        lineOfCode_ValidationInfo = this.RunLogicOrInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LOGICORCONST:
                        lineOfCode_ValidationInfo = this.RunLogicOrConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTLEFT:
                        lineOfCode_ValidationInfo = this.RunShiftLeftInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTLEFTPOS:
                        lineOfCode_ValidationInfo = this.RunShiftLeftPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTLEFTCONST:
                        lineOfCode_ValidationInfo = this.RunShiftLeftConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTRIGHT:
                        lineOfCode_ValidationInfo = this.RunShiftRightInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTRIGHTPOS:
                        lineOfCode_ValidationInfo = this.RunShiftRightPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SHIFTRIGHTCONST:
                        lineOfCode_ValidationInfo = this.RunShiftRightConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.FROMEMEM:
                        lineOfCode_ValidationInfo = this.RunFromMemInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.FROMMEMCONST:
                        lineOfCode_ValidationInfo = this.RunFromMemConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.FROMCONST:
                        lineOfCode_ValidationInfo = this.RunFromConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.TOMEM:
                        lineOfCode_ValidationInfo = this.RunToMemInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.TOMEMCONST:
                        lineOfCode_ValidationInfo = this.RunToMemConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.TOCONST:
                        lineOfCode_ValidationInfo = this.RunToConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.TOCONSTCONST:
                        lineOfCode_ValidationInfo = this.RunToConstConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.COPY:
                        lineOfCode_ValidationInfo = this.RunCopyInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.COPYERASE:
                        lineOfCode_ValidationInfo = this.RunCopyEraseInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHEN:
                        lineOfCode_ValidationInfo = this.RunLessThenInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENPOS:
                        lineOfCode_ValidationInfo = this.RunLessThenPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENCONST:
                        lineOfCode_ValidationInfo = this.RunLessThenConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENEQ:
                        lineOfCode_ValidationInfo = this.RunLessThenEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENEQPOS:
                        lineOfCode_ValidationInfo = this.RunLessThenEqPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHENEQCONST:
                        lineOfCode_ValidationInfo = this.RunLessThenEqConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHEN:
                        lineOfCode_ValidationInfo = this.RunMoreThenInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENPOS:
                        lineOfCode_ValidationInfo = this.RunMoreThenPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENCONST:
                        lineOfCode_ValidationInfo = this.RunMoreThenConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENEQ:
                        lineOfCode_ValidationInfo = this.RunMoreThenEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENEQPOS:
                        lineOfCode_ValidationInfo = this.RunMoreThenEqPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHENEQCONST:
                        lineOfCode_ValidationInfo = this.RunMoreThenEqConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.XOR:
                        lineOfCode_ValidationInfo = this.RunXORInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.XORCONST:
                        lineOfCode_ValidationInfo = this.RunXORConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.SAVEADDRESS:
                        lineOfCode_ValidationInfo = this.RunSaveAddressInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTO:
                        lineOfCode_ValidationInfo = this.RunGoToInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.EQ:
                        lineOfCode_ValidationInfo = this.RunEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.EQCONST:
                        lineOfCode_ValidationInfo = this.RunEqConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOEQ:
                        lineOfCode_ValidationInfo = this.RunGoToEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOEQCONST:
                        lineOfCode_ValidationInfo = this.RunGoToEqConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTONOEQ:
                        lineOfCode_ValidationInfo = this.RunGoToNoEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTONOEQCONST:
                        lineOfCode_ValidationInfo = this.RunGoToNoEqConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOMORETHEN:
                        lineOfCode_ValidationInfo = this.RunGoToMoreThenInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOMORETHENCONST:
                        lineOfCode_ValidationInfo = this.RunGoToMoreThenConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOLESSTHEN:
                        lineOfCode_ValidationInfo = this.RunGoToLessThenInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOLESSTHENCONST:
                        lineOfCode_ValidationInfo = this.RunGoToLessThenConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.JUMPLABEL:
                        lineOfCode_ValidationInfo = this.RunGoToJumpLabelInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MIPSADD:
                        lineOfCode_ValidationInfo = this.RunMipsAddInstructionValidationCheck(lineOfCode);
                        if (BVOperationInfo.MipsOperations.Exists(x => x == operation))
                        {
                            lineOfCode_ValidationInfo.IsValid = false;
                            lineOfCode_ValidationInfo.ValidationMessages.Add($"{MIPS_INSTRUCTION_DETECTED_MESSAGE_PREFIX}: MIPSADD");
                        }
                        break;
                    case BVOperation.MIPSSUB:
                        lineOfCode_ValidationInfo = this.RunMipsSubInstructionValidationCheck(lineOfCode);
                        if (BVOperationInfo.MipsOperations.Exists(x => x == operation))
                        {
                            lineOfCode_ValidationInfo.IsValid = false;
                            lineOfCode_ValidationInfo.ValidationMessages.Add($"{MIPS_INSTRUCTION_DETECTED_MESSAGE_PREFIX}: MIPSSUB");
                        }
                        break;
                    case BVOperation.MIPSLW:
                        lineOfCode_ValidationInfo = this.RunMipsLWInstructionValidationCheck(lineOfCode);
                        if (BVOperationInfo.MipsOperations.Exists(x => x == operation))
                        {
                            lineOfCode_ValidationInfo.IsValid = false;
                            lineOfCode_ValidationInfo.ValidationMessages.Add($"{MIPS_INSTRUCTION_DETECTED_MESSAGE_PREFIX}: MIPSLW");
                        }
                        break;
                    case BVOperation.MIPSSW:
                        lineOfCode_ValidationInfo = this.RunMipsSWInstructionValidationCheck(lineOfCode);
                        if (BVOperationInfo.MipsOperations.Exists(x => x == operation))
                        {
                            lineOfCode_ValidationInfo.IsValid = false;
                            lineOfCode_ValidationInfo.ValidationMessages.Add($"{MIPS_INSTRUCTION_DETECTED_MESSAGE_PREFIX}: MIPSSW");
                        }
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
        /// Run Instruction Validation Check for Mips Add
        /// </summary>
        /// <param name="lineOfCode"></param>
        /// <returns></returns>
        private ValidationInfo RunMipsAddInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "add";

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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for AddNS
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunADDNSInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");

                return validationInfo;
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for AddConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunADDConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for AddPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunADDPOSInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Run Instruction Validation Check for Mips Sub
        /// </summary>
        /// <param name="lineOfCode"></param>
        /// <returns></returns>
        private ValidationInfo RunMipsSubInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "sub";

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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SubNS
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSUBNSInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SubConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSubConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SubPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSUBPOSInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicAnd
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicAndInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicAndConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicAndConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicOr
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicOrInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LogicOrConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLogicOrConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftLeft
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftLeftInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftLeftPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftLeftPosInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftLeftConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftLeftConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftRight
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftRightInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftRightPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftRightPosInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ShiftRightConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunShiftRightConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for FromMem
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunFromMemInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for FromMemConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunFromMemConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for FromConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunFromConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 5");
            }

            int num = 0;
            if (operationParts.Length >= 3 && !int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToMem
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToMemInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToMemConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToMemConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 5, #2");
            }

            int num = 0;
            if (operationParts.Length >= 3 && !int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 5, #2");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for ToConstConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunToConstConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 3 && !int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 2, 5");
            }
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for copy
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunCopyInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for CopyErase
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunCopyEraseInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for Lessthen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenPosInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenEqInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenEqPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenEqPosInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for LessthenEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThenEqConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for Morethen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenPosInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenEqInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenEqPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenEqPosInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for MoreThenEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThenEqConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for XOR
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunXORInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for XORConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunXORConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for SaveAddress
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunSaveAddressInstructionValidationCheck(string lineOfCode)
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

            if (operationParts.Length != 3)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, labelName");
            }

            int num = 0;
            if (operationParts.Length >= 3 && int.TryParse(operationParts[2], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Number Found (${operationParts[2]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, labelName");
            }

            var jumpLabelRegexValidation = LettersOnlyregex.Match(operationParts[0]);
            if (!jumpLabelRegexValidation.Success)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Jump Label Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, labelName");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoTo
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1");
            }

            int num = 0;
            if (operationParts.Length >= 2 && !int.TryParse(operationParts[1], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[1]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for Eq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunEqInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for EqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunEqConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToEqInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToEqConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToNoEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToNoEqInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToNoEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToNoEqConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToMorethen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToMoreThenInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToMoreThenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToMoreThenConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToLessthen
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToLessThenInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, #3");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2], operationParts[3] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for GoToLessThenConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToLessThenConstInstructionValidationCheck(string lineOfCode)
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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            int num = 0;
            if (operationParts.Length >= 4 && !int.TryParse(operationParts[3], out num))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Non-Number Found (${operationParts[3]}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, #2, 5");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operationParts[2] };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Runs Instruction Validation Check for JumpLabel
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToJumpLabelInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "labelName:";

            string[] operationPartsSplitter = { " " };
            var operationParts = lineOfCode.Split(operationPartsSplitter, StringSplitOptions.RemoveEmptyEntries);

            if (!operationParts.Any())
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Found (No Operation Parts Found): -> ${lineOfCode}");
            }

            if (operationParts.Length != 1)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Instruction Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName}:");
            }

            var jumpLabelRegexValidation = LettersOnlyregex.Match(operationParts[0]);
            if (!jumpLabelRegexValidation.Success)
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Jump Label Format Found: -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName}");
            }

            return validationInfo;
        }

        /// <summary>
        /// Run Instruction Validation Check for Mips LW
        /// </summary>
        /// <param name="lineOfCode"></param>
        /// <returns></returns>
        private ValidationInfo RunMipsLWInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lw";

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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 0(#3)");
            }

            string secondOperationPart = operationParts[2].Replace(",", "").Trim();
            int indexOfFirstParenthesis = secondOperationPart.IndexOf("(");
            int indexOfSecondParenthesis = secondOperationPart.IndexOf(")");
            string operandARegister = secondOperationPart.Substring(indexOfFirstParenthesis + 1, (indexOfSecondParenthesis - indexOfFirstParenthesis - 1)).Trim();
            int offset;
            if (!int.TryParse(secondOperationPart.Substring(0, indexOfFirstParenthesis).Trim(), out offset))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Offset Value Found ({secondOperationPart.Substring(0, indexOfFirstParenthesis).Trim()}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 0(#3)");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operandARegister };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Run Instruction Validation Check for Mips SW
        /// </summary>
        /// <param name="lineOfCode"></param>
        /// <returns></returns>
        private ValidationInfo RunMipsSWInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "sw";

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
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 0(#3)");
            }

            string secondOperationPart = operationParts[2].Replace(",", "").Trim();
            int indexOfFirstParenthesis = secondOperationPart.IndexOf("(");
            int indexOfSecondParenthesis = secondOperationPart.IndexOf(")");
            string operandARegister = secondOperationPart.Substring(indexOfFirstParenthesis + 1, (indexOfSecondParenthesis - indexOfFirstParenthesis - 1)).Trim();
            int offset;
            if (!int.TryParse(secondOperationPart.Substring(0, indexOfFirstParenthesis).Trim(), out offset))
            {
                validationInfo.IsValid = validationInfo.IsValid && false;
                validationInfo.ValidationMessages.Add($"Invalid Offset Value Found ({secondOperationPart.Substring(0, indexOfFirstParenthesis).Trim()}): -> ${lineOfCode}");
                validationInfo.ValidationMessages.Add($"Correct Format: -> {operationName} #1, 0(#3)");
            }

            var operationOperandPartsRaw = new List<string> { operationParts[1], operandARegister };
            var operationOperandParts = operationOperandPartsRaw.Select(part => part.Replace(",", "").Trim());

            foreach (string operationOperandPart in operationOperandParts)
            {
                if (!this._Registry.Exists(operationOperandPart))
                {
                    validationInfo.IsValid = validationInfo.IsValid && false;
                    validationInfo.ValidationMessages.Add($"Unknown Register Found ({operationOperandPart}): -> ${lineOfCode}");
                }
            }

            return validationInfo;
        }

        /// <summary>
        /// Indicates if the only validation messages are mips detection messages
        /// </summary>
        /// <param name="validationInfo"> Validation Info </param>
        /// <returns>bool</returns>
        public static bool OnlyValidationMessagesAreMipsCodeDetections(ValidationInfo validationInfo)
        {
            bool mipsDetectionMessageExist = false;
            bool nonMipsValidationMessagesExist = false;

            foreach (string validationMessage in validationInfo?.ValidationMessages)
            {
                if (validationMessage.Contains(BVOperationValidationChecks.MIPS_INSTRUCTION_DETECTED_MESSAGE_PREFIX))
                {
                    mipsDetectionMessageExist = true;
                }
                else
                {
                    nonMipsValidationMessagesExist = true;
                }
            }

            return mipsDetectionMessageExist && !nonMipsValidationMessagesExist;
        }
    }
}
