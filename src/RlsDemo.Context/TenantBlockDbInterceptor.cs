﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using RlsDemo.Context.Extensions;
using RlsDemo.Context.Model;
using Softfluent.Asapp.Core.Context;

namespace RslDemo.Context
{
	public class TenantBlockDbInterceptor : SaveChangesInterceptor
	{
		private readonly CallContext _context;

		public TenantBlockDbInterceptor(CallContext context)
		{
			_context = context;
		}

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			if (eventData.Context is null)
				return base.SavingChangesAsync(eventData, result, cancellationToken);

			foreach (var entry in eventData.Context.ChangeTracker.Entries())
			{
				if (entry.Entity is ITenantEntity)
				{
					entry.GetTenantId()!.CurrentValue = _context.TenantId;
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
					entry.GetTenantId()!.CurrentValue = _context.TenantId;
				}
			}
			return base.SavingChanges(eventData, result);
		}
	}
}
