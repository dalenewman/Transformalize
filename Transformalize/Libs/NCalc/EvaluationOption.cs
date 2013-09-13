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

using System;

namespace Transformalize.Libs.NCalc
{
    // Summary:
    //     Provides enumerated values to use to set evaluation options.
    [Flags]
    public enum EvaluateOptions
    {
        // Summary:
        //     Specifies that no options are set.
        None = 1,
        //
        // Summary:
        //     Specifies case-insensitive matching.
        IgnoreCase = 2,
        //
        // Summary:
        //     No-cache mode. Ingores any pre-compiled expression in the cache.
        NoCache = 4,
        //
        // Summary:
        //     Treats parameters as arrays and result a set of results.
        IterateParameters = 8,
        //
        // Summary:
        //     When using Round(), if a number is halfway between two others, it is rounded toward the nearest number that is away from zero. 
        RoundAwayFromZero = 16
    }
}