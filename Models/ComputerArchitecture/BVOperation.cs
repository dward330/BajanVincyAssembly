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
        LESSTHAN = 26,
        LESSTHANPOS = 27,
        LESSTHANCONST = 28,
        LESSTHANEQ = 29,
        LESSTHANEQPOS = 30,
        LESSTHANEQCONST = 31,
        MORETHAN = 32,
        MORETHANPOS = 33,
        MORETHANCONST = 34,
        MORETHANEQ = 35,
        MORETHANEQPOS = 36,
        MORETHANEQCONST = 37,
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
        GOTOMORETHAN = 49,
        GOTOMORETHANCONST = 50,
        GOTOLESSTHAN = 51,
        GOTOLESSTHANCONST = 52,
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
            { "lessthan", BVOperation.LESSTHAN },
            { "lessthanpos", BVOperation.LESSTHANPOS },
            { "lessthanconst", BVOperation.LESSTHANCONST },
            { "lessthaneq", BVOperation.LESSTHANEQ },
            { "lessthaneqpos", BVOperation.LESSTHANEQPOS },
            { "lessthaneqconst", BVOperation.LESSTHANEQCONST },
            { "morethan", BVOperation.MORETHAN },
            { "morethanpos", BVOperation.MORETHANPOS },
            { "morethanconst", BVOperation.MORETHANCONST },
            { "morethaneq", BVOperation.MORETHANEQ },
            { "morethaneqpos", BVOperation.MORETHANEQPOS },
            { "morethaneqconst", BVOperation.MORETHANEQCONST },
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
            { "gotomorethan", BVOperation.GOTOMORETHAN },
            { "gotomorethanconst", BVOperation.GOTOMORETHANCONST },
            { "gotolessthan", BVOperation.GOTOLESSTHAN },
            { "gotolessthanconst", BVOperation.GOTOLESSTHANCONST },
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
            { BVOperation.LESSTHAN, "011010 " },
            { BVOperation.LESSTHANPOS, "011011 " },
            { BVOperation.LESSTHANCONST, "011100 " },
            { BVOperation.LESSTHANEQ, "011101 " },
            { BVOperation.LESSTHANEQPOS, "011110 " },
            { BVOperation.LESSTHANEQCONST, "011111 " },
            { BVOperation.MORETHAN, "100000 " },
            { BVOperation.MORETHANPOS, "100001 " },
            { BVOperation.MORETHANCONST, "100010 " },
            { BVOperation.MORETHANEQ, "100011 " },
            { BVOperation.MORETHANEQPOS, "100100 " },
            { BVOperation.MORETHANEQCONST, "100101 " },
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
            { BVOperation.GOTOMORETHAN, "110001 " },
            { BVOperation.GOTOMORETHANCONST, "110010 " },
            { BVOperation.GOTOLESSTHAN, "110011 " },
            { BVOperation.GOTOLESSTHANCONST, "110100 " },
            { BVOperation.JUMPLABEL, "110101 " },
            { BVOperation.MIPSADD, "110110" },
            { BVOperation.MIPSSUB, "110111" },
            { BVOperation.MIPSLW, "111000" },
            { BVOperation.MIPSSW, "111001" }
        };

        /// <summary>
        /// Collection of Mips Operation Codes
        /// </summary>
        public static List<BVOperation> MipsOperations = new List<BVOperation>() { BVOperation.MIPSADD, BVOperation.MIPSSUB, BVOperation.MIPSLW, BVOperation.MIPSSW };
    }
}
