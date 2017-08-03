#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Reflection;

namespace Transformalize.Transform.CSharp.ILReader {
    public interface ITokenResolver {
        MethodBase AsMethod(int token);
        FieldInfo AsField(int token);
        Type AsType(int token);
        String AsString(int token);
        MemberInfo AsMember(int token);
        byte[] AsSignature(int token);
    }

    public class ModuleScopeTokenResolver : ITokenResolver {
        private Module m_module;
        private MethodBase m_enclosingMethod;
        private Type[] m_methodContext;
        private Type[] m_typeContext;

        public ModuleScopeTokenResolver(MethodBase method) {
            m_enclosingMethod = method;
            m_module = method.Module;
            m_methodContext = (method is ConstructorInfo) ? null : method.GetGenericArguments();
            m_typeContext = (method.DeclaringType == null) ? null : method.DeclaringType.GetGenericArguments();
        }

        public MethodBase AsMethod(int token) {
            return m_module.ResolveMethod(token, m_typeContext, m_methodContext);
        }

        public FieldInfo AsField(int token) {
            return m_module.ResolveField(token, m_typeContext, m_methodContext);
        }

        public Type AsType(int token) {
            return m_module.ResolveType(token, m_typeContext, m_methodContext);
        }

        public MemberInfo AsMember(int token) {
            return m_module.ResolveMember(token, m_typeContext, m_methodContext);
        }

        public string AsString(int token) {
            return m_module.ResolveString(token);
        }

        public byte[] AsSignature(int token) {
            return m_module.ResolveSignature(token);
        }
    }
}
