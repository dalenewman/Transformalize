using System.CodeDom;
using System.Data.Common;
using System.Linq;
using Dapper;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.Ado {
    public class AdoFlatTableCreator : IAction {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoFlatTableCreator(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }

        public ActionResponse Execute() {
            var drop = _output.SqlDropFlatTable(_cf);
            var create = _output.SqlCreateFlatTable(_cf);

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                try {
                    cn.Execute(drop);
                } catch (DbException ex) {
                    _output.Debug(() => ex.Message);
                }
                cn.Execute(create);
            }
            return new ActionResponse();
        }
    }
}