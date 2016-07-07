#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;

namespace Cfg.Net.Parsers.nanoXML
{
    /// <summary>
    ///     Class representing whole DOM XML document
    /// </summary>
    public class NanoXmlDocument : NanoXmlBase
    {
        private readonly List<NanoXmlAttribute> _declarations = new List<NanoXmlAttribute>();
        private readonly NanoXmlNode _rootNode;

        /// <summary>
        ///     Public constructor. Loads xml document from raw string
        /// </summary>
        /// <param name="xmlString">String with xml</param>
        public NanoXmlDocument(string xmlString)
        {
            int i = 0;

            while (true)
            {
                SkipSpaces(xmlString, ref i);

                if (xmlString[i] != '<')
                    throw new NanoXmlParsingException("Unexpected token");

                i++; // skip <

                if (xmlString[i] == '?') // declaration
                {
                    i++; // skip ?
                    ParseAttributes(xmlString, ref i, _declarations, '?', '>');
                    i++; // skip ending ?
                    i++; // skip ending >

                    continue;
                }

                if (xmlString[i] == '!') // doctype
                {
                    while (xmlString[i] != '>') // skip doctype
                        i++;

                    i++; // skip >

                    continue;
                }

                _rootNode = new NanoXmlNode(xmlString, ref i);
                break;
            }
        }

        /// <summary>
        ///     Root document element
        /// </summary>
        public NanoXmlNode RootNode
        {
            get { return _rootNode; }
        }
    }
}