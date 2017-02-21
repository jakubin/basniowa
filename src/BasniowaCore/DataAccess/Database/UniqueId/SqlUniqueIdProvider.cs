using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Common;
using Common.CodeAnalysis;

namespace DataAccess.Database.UniqueId
{
    /// <summary>
    /// Implementation of <see cref="IUniqueIdProvider"/> using SQL stored procedure.
    /// </summary>
    /// <seealso cref="IUniqueIdProvider" />
    [ExcludeFromCodeCoverage]
    public class SqlUniqueIdProvider : IUniqueIdProvider
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUniqueIdProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlUniqueIdProvider(string connectionString)
        {
            Guard.NotNull(connectionString, nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <inheritdoc/>
        public async Task<long> GetNextIds(long count)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "[dbo].[GetNextIds]";
                command.CommandType = CommandType.StoredProcedure;
                var countParameter = CreateParameter(command, "@Count", DbType.Int64, count);
                var rangeFromParameter = CreateParameter(command, "@RangeFrom", DbType.Int64, direction: ParameterDirection.InputOutput);

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                await command.ExecuteNonQueryAsync();

                return (long)rangeFromParameter.Value;
            }
        }

        private static DbParameter CreateParameter(
            DbCommand command,
            string name,
            DbType type,
            object value = null,
            ParameterDirection direction = ParameterDirection.Input,
            bool isNullable = true)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = direction;
            parameter.IsNullable = isNullable;

            command.Parameters.Add(parameter);

            return parameter;
        }
    }
}
