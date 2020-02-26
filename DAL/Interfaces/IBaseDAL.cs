using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.DAL.Interfaces
{
    public interface IBaseDAL
    {
        IDbConnection GetConnection();
        IDbCommand GetCommand(string Query = "", IDbConnection Connection = null);
        IDbDataParameter GetParameter(string Name, object Value);
        void EnsureSchema();
    }
}
