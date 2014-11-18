using System.Net;
using System.Net.Sockets;

namespace Transformalize.Orchard.Models {
    /// <summary>
    /// http://stackoverflow.com/questions/2138706/how-to-check-a-input-ip-fall-in-a-specific-ip-range
    /// </summary>
    public class IpAddressRange {

        readonly AddressFamily _addressFamily;
        readonly byte[] _lowerBytes;
        readonly byte[] _upperBytes;

        public IpAddressRange(IPAddress lower, IPAddress upper) {
            _addressFamily = lower.AddressFamily;
            _lowerBytes = lower.GetAddressBytes();
            _upperBytes = upper.GetAddressBytes();
        }

        public bool IsInRange(IPAddress address) {
            if (address.AddressFamily != _addressFamily) {
                return false;
            }

            var addressBytes = address.GetAddressBytes();

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
