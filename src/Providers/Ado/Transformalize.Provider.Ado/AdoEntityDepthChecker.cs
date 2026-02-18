using Dapper;
using System.Data;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
    public class AdoEntityDepthChecker : IAction {

        private readonly OutputContext _context;
        private readonly IConnectionFactory _cf;

        public AdoEntityDepthChecker(OutputContext context, IConnectionFactory cf) {
            _context = context;
            _cf = cf;
        }

        private int GetDepth(IDbConnection cn) {
            _context.Debug(() => "Checking Depth");

            try {
                return cn.ExecuteScalar<int>(_context.SqlDepthFinder(_cf));
            } catch (System.Data.Common.DbException ex) {

                _context.Warn($"Could not check depth of {_context.Entity.OutputViewName(_context.Process.Name)}");
                _context.Debug(() => ex.Message);
                return 1;
            }
        }

        public ActionResponse Execute() {
            var response = new ActionResponse();

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var depth = GetDepth(cn);
                if (depth <= 1) {
                    _context.Debug(() => $"The relationship to {_context.Entity.Alias} checks out. The field(s) used are unique.");
                    return response;
                }

                response.Code = 500;
                response.Message = $"The relationship to {_context.Entity.Alias} produces duplicates, as many as {depth} per join.";
            }

            return response;

        }
    }
}