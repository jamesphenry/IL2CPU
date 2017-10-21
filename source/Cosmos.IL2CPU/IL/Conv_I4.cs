using System;

using XSharp;
using static XSharp.XSRegisters;

namespace Cosmos.IL2CPU.X86.IL
{
  /// <summary>
  /// Convert top Stack element to Int32.
  /// </summary>
  [Cosmos.IL2CPU.OpCode(ILOpCode.Code.Conv_I)] // x86 is a 32-bit system, so this is the op-code that we should be using, for an x64 target, use Conv_I8 instead.
  [Cosmos.IL2CPU.OpCode(ILOpCode.Code.Conv_I4)]
  public class Conv_I4 : ILOp
  {
    public Conv_I4(XSharp.Assembler.Assembler aAsmblr)
        : base(aAsmblr)
    {
    }

    public override void Execute(_MethodInfo aMethod, ILOpCode aOpCode)
    {
      var xSource = aOpCode.StackPopTypes[0];
      var xSourceSize = SizeOfType(xSource);
      var xSourceIsFloat = TypeIsFloat(xSource);
      switch (xSourceSize)
      {
        case 1:
        case 2:
        case 4:
          {
            if (xSourceIsFloat)
            {
              XS.SSE.MoveSS(XMM0, ESP, sourceIsIndirect: true);
              XS.SSE.ConvertSS2SIAndTruncate(EAX, XMM0);
              XS.Set(ESP, EAX, destinationIsIndirect: true);
            }
            break;
          }
        case 8:
          {
            if (xSourceIsFloat)
            {
              XS.SSE2.MoveSD(XMM0, ESP, sourceIsIndirect: true);
              XS.SSE2.ConvertSD2SIAndTruncate(EAX, XMM0);
              XS.Set(ESP, EAX, destinationIsIndirect: true);
            }

            if (IsReferenceType(xSource))
            {
              XS.Add(ESP, 4);
            }
            else
            {
              XS.Pop(EAX);
              XS.Add(ESP, 4);
              XS.Push(EAX);
            }

            break;
          }
        default:
          //EmitNotImplementedException( Assembler, GetServiceProvider(), "Conv_I4: SourceSize " + xSource + " not yet supported!", mCurLabel, mMethodInformation, mCurOffset, mNextLabel );
          throw new NotImplementedException();
      }
    }
  }
}
