#region license
// Transformalize
// Copyright 2013 Dale Newman
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
using System.Net;
using System.Net.Sockets;
using Orchard;

namespace Pipeline.Web.Orchard.Services {

    public interface IIpRangeService : IDependency {
        bool InRange(string address, string start, string end);
    }

    public class IpRangeService : IIpRangeService {
        AddressFamily _addressFamily;
        byte[] _lowerBytes;
        byte[] _upperBytes;

        public bool InRange(string address, string start, string end) {

            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                return false;

            IPAddress clientAddress;
            IPAddress startAddress;
            IPAddress endAddress;

            if (!IPAddress.TryParse(address, out clientAddress)) {
                return false;
            }

            if (!IPAddress.TryParse(start, out startAddress)) {
                return false;
            }

            if (!IPAddress.TryParse(end, out endAddress)) {
                return false;
            }

            _addressFamily = startAddress.AddressFamily;
            _lowerBytes = startAddress.GetAddressBytes();
            _upperBytes = endAddress.GetAddressBytes();

            if (clientAddress.AddressFamily != _addressFamily) {
                return false;
            }

            var addressBytes = clientAddress.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (var i = 0; i < _lowerBytes.Length &&
                            (lowerBoundary || upperBoundary); i++) {
                if ((lowerBoundary && addressBytes[i] < _lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > _upperBytes[i])) {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == _lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == _upperBytes[i]);
            }

            return true;
        }
    }

}