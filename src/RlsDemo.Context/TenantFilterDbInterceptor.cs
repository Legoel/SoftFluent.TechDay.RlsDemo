using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Softfluent.Asapp.Core.Context;

namespace RlsDemo.Context
{
	public class TenantFilterDbInterceptor : DbCommandInterceptor
	{
		private readonly CallContext _context;

		public TenantFilterDbInterceptor(CallContext context)
		{
			_context = context;
		}

		public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
			DbCommand command,
			CommandEventData eventData,
			InterceptionResult<DbDataReader> result,
			CancellationToken cancellationToken = default)
		{
			command.CommandText =
				$"EXEC sp_set_session_context @key=N'TenantId', @value=N'{_context.TenantId}';" +
				$"{command.CommandText}";
			return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
		}

		public override InterceptionResult<DbDataReader> ReaderExecuting(
			DbCommand command,
			CommandEventData eventData,
			InterceptionResult<DbDataReader> result)
		{
			command.CommandText =
				$"EXEC sp_set_session_context @key=N'TenantId', @value={_context.TenantId};" +
				$"{command.CommandText}";
			return base.ReaderExecuting(command, eventData, result);
		}
	}
}
