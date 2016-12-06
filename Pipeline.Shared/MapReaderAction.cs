using System;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize {
    public class MapReaderAction : IAction {

        private readonly IContext _context;
        private readonly Map _map;
        private readonly IMapReader _mapReader;

        public MapReaderAction(IContext context, Map map, IMapReader mapReader) {
            _context = context;
            _map = map;
            _mapReader = mapReader;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse();
            try {
                _map.Items.AddRange(_mapReader.Read(_context));
            } catch (Exception ex) {
                response.Code = 500;
                response.Message = "Could not read map " + _map.Name + ". Using query: " + _map.Query + ". Error: " + ex.Message;
            }
            return response;
        }
    }
}