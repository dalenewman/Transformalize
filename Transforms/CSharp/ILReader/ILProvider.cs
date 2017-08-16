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

namespace Transformalize.Transforms.CSharp.ILReader {
    public interface IILProvider {
        Byte[] GetByteArray();
    }

    public class MethodBaseILProvider : IILProvider {
        MethodBase m_method;
        byte[] m_byteArray;

        public MethodBaseILProvider(MethodBase method) {
            m_method = method;
        }

        public byte[] GetByteArray() {
            if (m_byteArray == null) {
                MethodBody methodBody = m_method.GetMethodBody();
                m_byteArray = (methodBody == null) ? new Byte[0] : methodBody.GetILAsByteArray();
            }
            return m_byteArray;
        }
    }
}
