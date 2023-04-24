DROP SECURITY POLICY IF EXISTS [Security].[SensitiveDataFilter]
GO

DROP FUNCTION IF EXISTS [Security].[fn_tenantfilterpredicate]
GO

DROP SCHEMA IF EXISTS [Security]
GO

CREATE SCHEMA [Security]
GO

CREATE FUNCTION [Security].[fn_tenantfilterpredicate](@TenantId int)
    RETURNS TABLE
    WITH SCHEMABINDING
AS
    RETURN SELECT 1 AS granted
    WHERE
        @TenantId = SESSION_CONTEXT(N'TenantId');
GO

CREATE SECURITY POLICY [Security].[SensitiveDataFilter]
    ADD FILTER PREDICATE [Security].[fn_tenantfilterpredicate](TenantId)
        ON [dbo].[SensitiveData],
    ADD BLOCK PREDICATE [Security].[fn_tenantfilterpredicate](TenantId)
        ON [dbo].[SensitiveData]
    WITH (STATE = ON);
GO
