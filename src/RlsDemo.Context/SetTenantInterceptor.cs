using Microsoft.EntityFrameworkCore.Diagnostics;
using RlsDemo.Context.Extensions;
using RlsDemo.Context.Model;
using Softfluent.Asapp.Core.Context;

namespace Afdec.OptiqFluent.Context
{
	public class SetTenantInterceptor : SaveChangesInterceptor
	{
		private readonly CallContext _callContext;

		public SetTenantInterceptor(CallContext callContext)
		{
			_callContext = callContext;
		}

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			if (eventData.Context is null)
				return base.SavingChangesAsync(eventData, result, cancellationToken);

			foreach (var entry in eventData.Context.ChangeTracker.Entries())
			{
				if (entry.Entity is ITenantEntity)
				{
					entry.GetTenantId()!.CurrentValue = _callContext.TenantId;
				}
			}
			return base.SavingChangesAsync(eventData, result, cancellationToken);
		}

		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			if (eventData.Context is null)
				return base.SavingChanges(eventData, result);

			foreach (var entry in eventData.Context.ChangeTracker.Entries())
			{
				if (entry.Entity is ITenantEntity)
				{
					entry.GetTenantId()!.CurrentValue = _callContext.TenantId;
				}
			}
			return base.SavingChanges(eventData, result);
		}
	}
}
