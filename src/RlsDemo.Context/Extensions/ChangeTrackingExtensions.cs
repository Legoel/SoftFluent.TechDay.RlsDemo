using Microsoft.EntityFrameworkCore.ChangeTracking;
using RlsDemo.Context.Model;

namespace RlsDemo.Context.Extensions
{
	public static class ChangeTrackingExtensions
	{
		public static MemberEntry? GetTenantId(this EntityEntry entry)
			=> entry.GetMemberEntry(nameof(ITenantEntity.TenantId));

		public static MemberEntry? GetMemberEntry(this EntityEntry entry, string memberName)
		{
			_ = entry ?? throw new ArgumentNullException(nameof(entry));
			if (string.IsNullOrWhiteSpace(memberName))
				throw new ArgumentException($"'{nameof(memberName)}' cannot be null or whitespace.", nameof(memberName));

			return entry.Members
				.Where(m => m.Metadata.Name == memberName)
				.FirstOrDefault();
		}
	}
}
