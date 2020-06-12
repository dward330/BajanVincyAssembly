using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.ComputerArchitecture
{
    /// <summary>
    /// All BV Assembly Opertions
    /// </summary>
    public enum BVOperation
    {
        ADDNS = 1,
        ADDCONST = 2,
        ADDPOS = 3,
        SUBNS = 4,
        SUBCONST = 5,
        SUBPOS = 6,
        LOGICAND = 7,
        LOGICANDCOSNT = 8,
        LOGICOR = 9,
        LOGICORCONST = 10,
        SHIFTLEFT = 11,
        SHIFTLEFTPOS = 12,
        SHIFTLEFTCONST = 13,
        SHIFTRIGHT = 14,
        SHIFTRIGHTPOS = 15,
        SHIFTRIGHTCONST = 16,
        FROMEMEM = 17,
        FROMMEMCONST = 18,
        FROMCONST = 19,
        TOMEM = 20,
        TOMEMCONST = 21,
        TOCONST = 22,
        TOCONSTCONST = 23,
        COPY = 24,
        COPYERASE = 25,
        LESSTHEN = 26,
        LESSTHENPOS = 27,
        LESSTHENCONST = 28,
        LESSTHENEQ = 29,
        LESSTHENEQPOS = 30,
        LESSTHENEQCONST = 31,
        MORETHEN = 32,
        MORETHENPOS = 33,
        MORETHENCONST = 34,
        MORETHENEQ = 35,
        MORETHENEQPOS = 36,
        MORETHENEQCONST = 37,
        XOR = 38,
        XORCONST = 39,
        SAVEADDRESS = 40,
        GOTO = 41,
        GOTOCONST = 42,
        EQ = 43,
        EQCONST = 44,
        GOTOEQ = 45,
        GOTOEQCONST = 46,
        GOTONOEQ = 47,
        GOTONOEQCONST = 48,
        GOTOMORETHEN = 49,
        GOTOMORETHENCONST = 50,
        GOTOLESSTHEN = 51,
        GOTOLESSTHENCONST = 52,
        JUMPLABEL = 53,
        MIPSADD = 54, 
        MIPSSUB = 55,
        MIPSLW = 56, 
        MIPSSW = 57
    }

    /// <summary>
    /// Contains Information about BV Operations
    /// </summary>
    public class BVOperationInfo
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="BVOperationInfo" class/>
        /// </summary>
        public BVOperationInfo()
        {
            
        }

        /// <summary>
        /// Gets BV Operation Loopkup Dictionary
        /// </summary>
        public static readonly Dictionary<string, BVOperation> BVOperationLookup = new Dictionary<string, BVOperation>()
        {
            { "addns", BVOperation.ADDNS },
            { "addconst", BVOperation.ADDCONST },
            { "addpos", BVOperation.ADDPOS },
            { "subns", BVOperation.SUBNS },
            { "subconst", BVOperation.SUBCONST },
            { "subpos", BVOperation.SUBPOS },
            { "logicand", BVOperation.LOGICAND },
            { "logicandcosnt", BVOperation.LOGICANDCOSNT },
            { "logicor", BVOperation.LOGICOR },
            { "logicorconst", BVOperation.LOGICORCONST },
            { "shiftleft", BVOperation.SHIFTLEFT },
            { "shiftleftpos", BVOperation.SHIFTLEFTPOS },
            { "shiftleftconst", BVOperation.SHIFTLEFTCONST },
            { "shiftright", BVOperation.SHIFTRIGHT },
            { "shiftrightpos", BVOperation.SHIFTRIGHTPOS },
            { "shiftrightconst", BVOperation.SHIFTRIGHTCONST },
            { "fromemem", BVOperation.FROMEMEM },
            { "frommemconst", BVOperation.FROMMEMCONST },
            { "fromconst", BVOperation.FROMCONST },
            { "tomem", BVOperation.TOMEM },
            { "tomemconst", BVOperation.TOMEMCONST },
            { "toconst", BVOperation.TOCONST },
            { "toconstconst", BVOperation.TOCONSTCONST },
            { "copy", BVOperation.COPY },
            { "copyerase", BVOperation.COPYERASE },
            { "lessthen", BVOperation.LESSTHEN },
            { "lessthenpos", BVOperation.LESSTHENPOS },
            { "lessthenconst", BVOperation.LESSTHENCONST },
            { "lesstheneq", BVOperation.LESSTHENEQ },
            { "lesstheneqpos", BVOperation.LESSTHENEQPOS },
            { "lesstheneqconst", BVOperation.LESSTHENEQCONST },
            { "morethen", BVOperation.MORETHEN },
            { "morethenpos", BVOperation.MORETHENPOS },
            { "morethenconst", BVOperation.MORETHENCONST },
            { "moretheneq", BVOperation.MORETHENEQ },
            { "moretheneqpos", BVOperation.MORETHENEQPOS },
            { "moretheneqconst", BVOperation.MORETHENEQCONST },
            { "xor", BVOperation.XOR },
            { "xorconst", BVOperation.XORCONST },
            { "saveaddress", BVOperation.SAVEADDRESS },
            { "goto", BVOperation.GOTO },
            { "gotoconst", BVOperation.GOTOCONST },
            { "eq", BVOperation.EQ },
            { "eqconst", BVOperation.EQCONST },
            { "gotoeq", BVOperation.GOTOEQ },
            { "gotoeqconst", BVOperation.GOTOEQCONST },
            { "gotonoeq", BVOperation.GOTONOEQ },
            { "gotonoeqconst", BVOperation.GOTONOEQCONST },
            { "gotomorethan", BVOperation.GOTOMORETHEN },
            { "gotomorethanconst", BVOperation.GOTOMORETHENCONST },
            { "gotolessthen", BVOperation.GOTOLESSTHEN },
            { "gotolessthenconst", BVOperation.GOTOLESSTHENCONST },
            { "add", BVOperation.MIPSADD },
            { "sub", BVOperation.MIPSSUB },
            { "lw", BVOperation.MIPSLW },
            { "sw", BVOperation.MIPSSW }
        };

        /// <summary>
        /// Gets BV Operation Code Loopkup Dictionary
        /// </summary>
        public static readonly Dictionary<BVOperation, string> BVOperationCodeLookup = new Dictionary<BVOperation, string>()
        {
            { BVOperation.ADDNS, "000001 " },
            { BVOperation.ADDCONST, "000010 " },
            { BVOperation.ADDPOS, "000011 " },
            { BVOperation.SUBNS, "000100 " },
            { BVOperation.SUBCONST, "000101 " },
            { BVOperation.SUBPOS, "000110 " },
            { BVOperation.LOGICAND, "000111 " },
            { BVOperation.LOGICANDCOSNT, "001000 " },
            { BVOperation.LOGICOR, "001001 " },
            { BVOperation.LOGICORCONST, "001010 " },
            { BVOperation.SHIFTLEFT, "001011 " },
            { BVOperation.SHIFTLEFTPOS, "001100 " },
            { BVOperation.SHIFTLEFTCONST, "001101 " },
            { BVOperation.SHIFTRIGHT, "001110 " },
            { BVOperation.SHIFTRIGHTPOS, "001111 " },
            { BVOperation.SHIFTRIGHTCONST, "010000 " },
            { BVOperation.FROMEMEM, "010001 " },
            { BVOperation.FROMMEMCONST, "010010 " },
            { BVOperation.FROMCONST, "010011 " },
            { BVOperation.TOMEM, "010100 " },
            { BVOperation.TOMEMCONST, "010101 " },
            { BVOperation.TOCONST, "010110 " },
            { BVOperation.TOCONSTCONST, "010111 " },
            { BVOperation.COPY, "011000 " },
            { BVOperation.COPYERASE, "011001 " },
            { BVOperation.LESSTHEN, "011010 " },
            { BVOperation.LESSTHENPOS, "011011 " },
            { BVOperation.LESSTHENCONST, "011100 " },
            { BVOperation.LESSTHENEQ, "011101 " },
            { BVOperation.LESSTHENEQPOS, "011110 " },
            { BVOperation.LESSTHENEQCONST, "011111 " },
            { BVOperation.MORETHEN, "100000 " },
            { BVOperation.MORETHENPOS, "100001 " },
            { BVOperation.MORETHENCONST, "100010 " },
            { BVOperation.MORETHENEQ, "100011 " },
            { BVOperation.MORETHENEQPOS, "100100 " },
            { BVOperation.MORETHENEQCONST, "100101 " },
            { BVOperation.XOR, "100110 " },
            { BVOperation.XORCONST, "100111 " },
            { BVOperation.SAVEADDRESS, "101000 " },
            { BVOperation.GOTO, "101001 " },
            { BVOperation.GOTOCONST, "101010 " },
            { BVOperation.EQ, "101011 " },
            { BVOperation.EQCONST, "101100 " },
            { BVOperation.GOTOEQ, "101101 " },
            { BVOperation.GOTOEQCONST, "101110 " },
            { BVOperation.GOTONOEQ, "101111 " },
            { BVOperation.GOTONOEQCONST, "110000 " },
            { BVOperation.GOTOMORETHEN, "110001 " },
            { BVOperation.GOTOMORETHENCONST, "110010 " },
            { BVOperation.GOTOLESSTHEN, "110011 " },
            { BVOperation.GOTOLESSTHENCONST, "110100 " },
            { BVOperation.JUMPLABEL, "110101 " },
            { BVOperation.MIPSADD, "110110" },
            { BVOperation.MIPSSUB, "110111" },
            { BVOperation.MIPSLW, "111000" },
            { BVOperation.MIPSSW, "111001" }
        };

        /// <summary>
        /// Collection of Mips Operation Codes
        /// </summary>
        public List<BVOperation> MipsOperations { get; } = new List<BVOperation>() { BVOperation.MIPSADD, BVOperation.MIPSSUB, BVOperation.MIPSLW, BVOperation.MIPSSW };
    }
}
