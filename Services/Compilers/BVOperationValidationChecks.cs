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
                    case BVOperation.LESSTHAN:
                        lineOfCode_ValidationInfo = this.RunLessThanInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHANPOS:
                        lineOfCode_ValidationInfo = this.RunLessThanPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHANCONST:
                        lineOfCode_ValidationInfo = this.RunLessThanConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHANEQ:
                        lineOfCode_ValidationInfo = this.RunLessThanEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHANEQPOS:
                        lineOfCode_ValidationInfo = this.RunLessThanEqPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.LESSTHANEQCONST:
                        lineOfCode_ValidationInfo = this.RunLessThanEqConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHAN:
                        lineOfCode_ValidationInfo = this.RunMoreThanInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHANPOS:
                        lineOfCode_ValidationInfo = this.RunMoreThanPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHANCONST:
                        lineOfCode_ValidationInfo = this.RunMoreThanConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHANEQ:
                        lineOfCode_ValidationInfo = this.RunMoreThanEqInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHANEQPOS:
                        lineOfCode_ValidationInfo = this.RunMoreThanEqPosInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.MORETHANEQCONST:
                        lineOfCode_ValidationInfo = this.RunMoreThanEqConstInstructionValidationCheck(lineOfCode);
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
                    case BVOperation.GOTOMORETHAN:
                        lineOfCode_ValidationInfo = this.RunGoToMoreThanInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOMORETHANCONST:
                        lineOfCode_ValidationInfo = this.RunGoToMoreThanConstInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOLESSTHAN:
                        lineOfCode_ValidationInfo = this.RunGoToLessThanInstructionValidationCheck(lineOfCode);
                        break;
                    case BVOperation.GOTOLESSTHANCONST:
                        lineOfCode_ValidationInfo = this.RunGoToLessThanConstInstructionValidationCheck(lineOfCode);
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
        /// Runs Instruction Validation Check for Lessthan
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThanInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthan";

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
        /// Runs Instruction Validation Check for LessthanPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThanPosInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthanpos";

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
        /// Runs Instruction Validation Check for LessthanConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThanConstInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthanconst";

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
        /// Runs Instruction Validation Check for LessthanEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThanEqInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthaneq";

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
        /// Runs Instruction Validation Check for LessthanEqPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThanEqPosInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthaneqpos";

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
        /// Runs Instruction Validation Check for LessthanEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunLessThanEqConstInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "lessthaneqconst";

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
        /// Runs Instruction Validation Check for Morethan
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThanInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethan";

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
        /// Runs Instruction Validation Check for MoreThanPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThanPosInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethanpos";

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
        /// Runs Instruction Validation Check for MoreThanConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThanConstInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethanconst";

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
        /// Runs Instruction Validation Check for MoreThanEq
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThanEqInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethaneq";

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
        /// Runs Instruction Validation Check for MoreThanEqPos
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThanEqPosInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethaneqpos";

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
        /// Runs Instruction Validation Check for MoreThanEqConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunMoreThanEqConstInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "morethaneqconst";

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
        /// Runs Instruction Validation Check for GoToMorethan
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToMoreThanInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotomorethan";

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
        /// Runs Instruction Validation Check for GoToMoreThanConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToMoreThanConstInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotomorethanconst";

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
        /// Runs Instruction Validation Check for GoToLessthan
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToLessThanInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotolessthan";

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
        /// Runs Instruction Validation Check for GoToLessThanConst
        /// </summary>
        /// <param name="lineOfCode">line of code to validate</param>
        /// <returns>Validation Info</returns>
        private ValidationInfo RunGoToLessThanConstInstructionValidationCheck(string lineOfCode)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            const string operationName = "gotolessthanconst";

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
