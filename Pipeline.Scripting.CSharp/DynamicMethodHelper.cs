// 
// A helper class to turn MethodInfo to DynamicMethod, demo'ing how to use DynamicILInfo
// This class depends on the ILReader described at http://blogs.msdn.com/haibo_luo/archive/2006/11/06/system-reflection-based-ilreader.aspx
//
// By Haibo Luo @ http://blogs.msdn.com/haibo_luo
//
// THIS CODE IS PROVIDED "AS IS", WITH NO WARRANTIES INTENDED OR IMPLIED. USE AT YOUR OWN RISK
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Pipeline.Scripting.CSharp.ILReader;

namespace Pipeline.Scripting.CSharp {
    public class DynamicMethodHelper {
        class IlInfoGetTokenVisitor : ILInstructionVisitor {
            private readonly DynamicILInfo _ilInfo;
            private readonly byte[] _code;

            public IlInfoGetTokenVisitor(DynamicILInfo ilinfo, byte[] code) {
                _ilInfo = ilinfo;
                _code = code;
            }

            public override void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction) {
                OverwriteInt32(_ilInfo.GetTokenFor(
                    inlineMethodInstruction.Method.MethodHandle,
                    inlineMethodInstruction.Method.DeclaringType.TypeHandle),
                    inlineMethodInstruction.Offset + inlineMethodInstruction.OpCode.Size);
            }

            public override void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction) {
                OverwriteInt32(_ilInfo.GetTokenFor(inlineSigInstruction.Signature),
                    inlineSigInstruction.Offset + inlineSigInstruction.OpCode.Size);
            }

            public override void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction) {
                //CLR BUG: 
                //OverwriteInt32(ilInfo.GetTokenFor(inlineFieldInstruction.Field.FieldHandle, inlineFieldInstruction.Field.DeclaringType.TypeHandle),
                //    inlineFieldInstruction.Offset + inlineFieldInstruction.OpCode.Size);

                OverwriteInt32(_ilInfo.GetTokenFor(inlineFieldInstruction.Field.FieldHandle),
                    inlineFieldInstruction.Offset + inlineFieldInstruction.OpCode.Size);
            }

            public override void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction) {
                OverwriteInt32(_ilInfo.GetTokenFor(inlineStringInstruction.String),
                   inlineStringInstruction.Offset + inlineStringInstruction.OpCode.Size);
            }

            public override void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction) {
                OverwriteInt32(_ilInfo.GetTokenFor(inlineTypeInstruction.Type.TypeHandle),
                   inlineTypeInstruction.Offset + inlineTypeInstruction.OpCode.Size);
            }

            public override void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction) {
                var mi = inlineTokInstruction.Member;
                var token = 0;
                if (mi.MemberType == MemberTypes.TypeInfo || mi.MemberType == MemberTypes.NestedType) {
                    var type = mi as Type;
                    token = _ilInfo.GetTokenFor(type.TypeHandle);
                } else if (mi.MemberType == MemberTypes.Method || mi.MemberType == MemberTypes.Constructor) {
                    var m = mi as MethodBase;
                    token = _ilInfo.GetTokenFor(m.MethodHandle, m.DeclaringType.TypeHandle);
                } else if (mi.MemberType == MemberTypes.Field) {
                    var f = mi as FieldInfo;
                    //CLR BUG: token = ilInfo.GetTokenFor(f.FieldHandle, f.DeclaringType.TypeHandle);
                    token = _ilInfo.GetTokenFor(f.FieldHandle);
                }

                OverwriteInt32(token, inlineTokInstruction.Offset + inlineTokInstruction.OpCode.Size);
            }

            void OverwriteInt32(int value, int pos) {
                _code[pos++] = (byte)value;
                _code[pos++] = (byte)(value >> 8);
                _code[pos++] = (byte)(value >> 16);
                _code[pos++] = (byte)(value >> 24);
            }
        }

        public static DynamicMethod ConvertFrom(MethodInfo method) {
            if (method == null) {
                throw new ArgumentNullException(nameof(method));
            }

            if (!method.IsStatic) {
                throw new InvalidOperationException("the method is expected to be static");
            }

            if (method.IsGenericMethod) {
                throw new InvalidOperationException("currently dynamic method cannot be generic");
            }

            var body = method.GetMethodBody();

            if (body == null) {
                throw new InvalidOperationException("the method does not have method body");
            }

            var dm = new DynamicMethod(method.Name,method.ReturnType,GetParameterTypes(method),typeof(DynamicMethodHelper));

            var ilInfo = dm.GetDynamicILInfo();

            SetLocalSignature(body, ilInfo);
            SetCode(method, body, ilInfo);
            SetExceptions(body, ilInfo);

            return dm;
        }

        private static void SetCode(MethodBase method, MethodBody body, DynamicILInfo ilInfo) {
            var code = body.GetILAsByteArray();
            var reader = new ILReader.ILReader(method);
            var visitor = new IlInfoGetTokenVisitor(ilInfo, code);
            reader.Accept(visitor);
            ilInfo.SetCode(code, body.MaxStackSize);
        }

        private static void SetLocalSignature(MethodBody body, DynamicILInfo ilInfo) {
            SignatureHelper sig = SignatureHelper.GetLocalVarSigHelper();
            foreach (LocalVariableInfo lvi in body.LocalVariables) {
                sig.AddArgument(lvi.LocalType, lvi.IsPinned);
            }
            ilInfo.SetLocalSignature(sig.GetSignature());
        }

        private static void SetExceptions(MethodBody body, DynamicILInfo ilInfo) {
            IList<ExceptionHandlingClause> ehcs = body.ExceptionHandlingClauses;
            int ehCount = ehcs.Count;
            if (ehCount == 0) return;

            // Let us do FAT exception header
            int size = 4 + 24 * ehCount;
            byte[] exceptions = new byte[size];

            exceptions[0] = 0x01 | 0x40; //Offset: 0, Kind: CorILMethod_Sect_EHTable | CorILMethod_Sect_FatFormat
            OverwriteInt32(size, 1, exceptions);  // Offset: 1, DataSize: n * 24 + 4

            int pos = 4;
            foreach (ExceptionHandlingClause ehc in ehcs) {
                // 
                // Flags, TryOffset, TryLength, HandlerOffset, HandlerLength, 
                //
                OverwriteInt32((int)ehc.Flags, pos, exceptions); pos += 4;
                OverwriteInt32(ehc.TryOffset, pos, exceptions); pos += 4;
                OverwriteInt32(ehc.TryLength, pos, exceptions); pos += 4;
                OverwriteInt32(ehc.HandlerOffset, pos, exceptions); pos += 4;
                OverwriteInt32(ehc.HandlerLength, pos, exceptions); pos += 4;

                //
                // ClassToken or FilterOffset
                //
                switch (ehc.Flags) {
                    case ExceptionHandlingClauseOptions.Clause:
                        int token = ilInfo.GetTokenFor(ehc.CatchType.TypeHandle);
                        OverwriteInt32(token, pos, exceptions);
                        break;
                    case ExceptionHandlingClauseOptions.Filter:
                        OverwriteInt32(ehc.FilterOffset, pos, exceptions);
                        break;
                    case ExceptionHandlingClauseOptions.Fault:
                        throw new NotSupportedException("dynamic method does not support fault clause");
                    case ExceptionHandlingClauseOptions.Finally:
                        break;
                }
                pos += 4;
            }

            ilInfo.SetExceptions(exceptions);
        }

        public static void OverwriteInt32(int value, int pos, byte[] array) {
            array[pos++] = (byte)value;
            array[pos++] = (byte)(value >> 8);
            array[pos++] = (byte)(value >> 16);
            array[pos++] = (byte)(value >> 24);
        }

        static Type[] GetParameterTypes(MethodInfo method) {
            ParameterInfo[] pia = method.GetParameters();
            Type[] types = new Type[pia.Length];

            for (int i = 0; i < pia.Length; i++) {
                types[i] = pia[i].ParameterType;
            }
            return types;
        }
    }
}
