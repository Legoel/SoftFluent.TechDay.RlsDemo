using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Softfluent.Asapp.Core.Context;

namespace Afdec.OptiqFluent.Context
{
	public class TenantFilterDbInterceptor : DbCommandInterceptor
	{
		private readonly CallContext _callContext;

		public TenantFilterDbInterceptor(CallContext callContext)
		{
			_callContext = callContext;
		}

		public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
			DbCommand command,
			CommandEventData eventData,
			InterceptionResult<DbDataReader> result,
			CancellationToken cancellationToken = default)
		{
			command.CommandText =
				$"EXEC sp_set_session_context @key=N'TenantId', @value=N'{_callContext.TenantId}';" +
				$"{command.CommandText}";
			return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
		}

		public override InterceptionResult<DbDataReader> ReaderExecuting(
			DbCommand command,
			CommandEventData eventData,
			InterceptionResult<DbDataReader> result)
		{
			command.CommandText =
				$"EXEC sp_set_session_context @key=N'TenantId', @value={_callContext.TenantId};" +
				$"{command.CommandText}";
			return base.ReaderExecuting(command, eventData, result);
		}
	}
}
