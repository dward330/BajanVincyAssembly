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
        GOTOMORETHAN = 49,
        GOTOMORETHANCONST = 50,
        GOTOLESSTHEN = 51,
        GOTOLESSTHENCONST = 52,
        JUMPLABEL = 53
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
            { "gotomorethan", BVOperation.GOTOMORETHAN },
            { "gotomorethanconst", BVOperation.GOTOMORETHANCONST },
            { "gotolessthen", BVOperation.GOTOLESSTHEN },
            { "gotolessthenconst", BVOperation.GOTOLESSTHENCONST }
        };
    }
}
