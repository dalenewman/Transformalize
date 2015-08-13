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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Main;
using ILogger = Transformalize.Logging.ILogger;

namespace Transformalize.Runner {

    public class ProcessXmlConfigurationReader : IReader<List<TflProcess>> {

        private readonly string _resource;
        private readonly ContentsReader _contentsReader;
        private readonly ILogger _logger;

        public ProcessXmlConfigurationReader(string resource, ContentsReader contentsReader, ILogger logger) {
            _resource = resource;
            _contentsReader = contentsReader;
            _logger = logger;
        }

        public List<TflProcess> Read(Dictionary<string, string> parameters) {

            var shouldThrow = false;
            var content = _contentsReader.Read(_resource).Content;
            var cfg = new TflRoot(content, parameters);

            if (cfg.Response.Any()) {
                foreach (var response in cfg.Response.Where(response => response.Status != (short)200)) {
                    _logger.Warn("API at {0} responded with {1} {2}.", _resource, response.Status, response.Message);
                }
            }

            foreach (var warning in cfg.Warnings()) {
                _logger.Warn(warning);
            }

            foreach (var error in cfg.Errors()) {
                shouldThrow = true;
                _logger.Error(error);
            }

            if (shouldThrow) {
                throw new TransformalizeException(_logger, string.Join(Environment.NewLine, cfg.Errors()));
            }

            return cfg.Processes;

        }

    }
}