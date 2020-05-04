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
        ADDNS,
        ADDCONST,
        ADDPOS,
        SUBNS,
        SUBCONST,
        SUBPOS,
        LOGICAND,
        LOGICANDCOSNT,
        LOGICOR,
        LOGICORCONST,
        SHIFTLEFT,
        SHIFTLEFTPOS,
        SHIFTLEFTCONST,
        SHIFTRIGHT,
        SHIFTRIGHTPOS,
        SHIFTRIGHTCONST,
        FROMEMEM,
        FROMMEMCONST,
        FROMCONST,
        TOMEM,
        TOMEMCONST,
        TOCONST,
        TOCONSTCONST,
        COPY,
        COPYERASE,
        LESSTHEN,
        LESSTHENPOS,
        LESSTHENCONST,
        LESSTHENEQ,
        LESSTHENEQPOS,
        LESSTHENEQCONST,
        MORETHEN,
        MORETHENPOS,
        MORETHENCONST,
        MORETHENEQ,
        MORETHENEQPOS,
        MORETHENEQCONST,
        XOR,
        XORCONST,
        SAVEADDRESS,
        GOTO,
        GOTOCONST,
        EQ,
        EQCONST,
        GOTOEQ,
        GOTOEQCONST,
        GOTONOEQ,
        GOTONOEQCONST,
        GOTOMORETHAN,
        GOTOMORETHANCONST,
        GOTOLESSTHEN,
        GOTOLESSTHENCONST
    }
}
