#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Text;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.RunTime
{
    internal sealed class AttributesBuilder
    {
        private readonly NetLanguage mLeng;
        private readonly StringBuilder mSb = new StringBuilder(250);

        private bool mFirst = true;

        public AttributesBuilder(NetLanguage lang)
        {
            mLeng = lang;
        }

        public void AddAttribute(string attribute)
        {
            if (attribute == null || attribute == string.Empty)
                return;

            if (mFirst)
            {
                switch (mLeng)
                {
                    case NetLanguage.CSharp:
                        mSb.Append("[");
                        break;
                    case NetLanguage.VbNet:
                        mSb.Append("<");
                        break;
                }
                mFirst = false;
            }
            else
            {
                switch (mLeng)
                {
                    case NetLanguage.VbNet:
                        mSb.Append(", _");
                        mSb.Append(StringHelper.NewLine);
                        mSb.Append(" ");
                        break;
                    case NetLanguage.CSharp:
                        mSb.Append("[");
                        break;
                }
            }

            mSb.Append(attribute);

            switch (mLeng)
            {
                case NetLanguage.CSharp:
                    mSb.Append("]");
                    mSb.Append(StringHelper.NewLine);
                    break;
                case NetLanguage.VbNet:
                    break;
            }
        }

        public string GetAttributesCode()
        {
            if (mFirst)
                return string.Empty;

            switch (mLeng)
            {
                case NetLanguage.VbNet:
                    mSb.Append("> _");
                    mSb.Append(StringHelper.NewLine);
                    break;
            }

            return mSb.ToString();
        }
    }
}