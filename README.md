# SoftFluent.TechDay.RlsDemo

[Projet de démonstration](https://github.com/Legoel/SoftFluent.TechDay.RlsDemo) de SQL Server Row-Level Security.

## Résumé

Le projet est une solution Visual Studio autonome contenant 3 projets

- _Asapp.Core_ est un projet interne dont l'objectif est d'accélerer les développements sur les projets SoftFluent (par exemple pour des prototypes ou des réponses à appels d'offres). Il est ici utilisé surtout pour sa classe `BaseRepository` permettant un accès complet à une table de base de données via EFCore et à la dernière étape pour sa classe `CallContext`.
- _RlsDemo.Context_ est un projet Entity Framework Core exposant 2 entités : `Tenant` & `SensitiveDatum`
- _RlsDemo.Web_ est un projet ASP.Net Core WebAPI standard auquel on a ajouté un contrôleur `SensitiveDataController` permettant d'effectuer les opérations élémentaires (CRUD) sur l'entité `SensitiveDatum` et un contrôleur `TokenController` permettant de récupérer un jeton JWT pour s'authentifier auprès du service REST.

Pour pouvoir mettre en œuvre la fonctionnalité **Row-Level Security**, il nous faut un serveur SQL, on ne peut donc pas s'appuyer sur une base _InMemory_ dans ce projet.
La chaine de connexion est à préciser dans le fichier appsettings.json dans une chaine de connexion portant le nom de la machine sur lequel le projet est executé.
La base de données est recrée à chaque lancement de l'application.

Pour récupérer un jeton JWT "Administrateur" il suffit de demander un jeton au nom de "Thomas". Tout autre nom génèrera un jeton "Utilisateur" (les requêtes de création, suppression et modification sont bloquées).
Dans la version finale de l'application, l'utilisateur "Thomas" est associé au locataire 1, l'utilisateur "Pierre" au locataire 2, tous les autres utilisateurs au locataire 3.

## Etape initiale

Pour consulter le projet dans sa version initiale vous pouvez répartir du commit 252b4db8d9287d29d364399649bfa28340c4284c

```bash
git checkout 252b4db8d9287d29d364399649bfa28340c4284c
```

Dans cette version, les données sont totalement décloisonnées : un utilisateur authentifié peut voir, créer, modifier, supprimer les données de tous les locataires (tenant) dès lors qu'il a le droit de le faire sur les siennes !

## Modification itérative du code

Dans les étapes suivantes, nous avons successivement

1. Ajouté un filtre "manuellement" à chaque requête à partir d'un identifiant locataire fourni en paramètre.
2. Masqué l'identifiant locataire dans un _claim_ du jeton JWT et utilisé cette information dans les requêtes sans que l'utilisateur ne puisse la modifier.
3. Utilisé les [filtres globaux de requête d'EFCore](https://learn.microsoft.com/en-us/ef/core/querying/filters) pour forcer le filtres sur les données des locataires et les [filtres d'actions d'ASP.NET](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/controllers-and-routing/understanding-action-filters-cs) pour extraire automatiquement l'identifiant du locataire du jeton JWT

   <mark>Astuce</mark> Pour créer un filtre global "dynamique" il faut exposer une propriété au niveau de la classe `RlsDemoContext`. Grâce à l'injection de dépendance, on peut assigner une valeur à cette propriété au niveau de la couche _Web_ (mais ce n'est pas très propre) et cette valeur sera exploitée pour le filtre global.

4. Finalement nous avons supprimé le filtre global de requête et exploité le **Row-Level Security** de SQL Server (cf. ci-dessous).

## Row-Level Security

Pour cela nous avons ajouté une _Security Policy_ basée sur une _Table-Valued function_

```sql
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
```

La requête utilisée dans la fonction peut-être beaucoup plus complexe et utiliser des jointures par exemples pour aller récupérer le rôle de l'utilisateur ou l'identifiant de son locataire.

Il est également possible d'utiliser des fonctions différentes pour les prédicats `FILTER` (correspondant au `SELECT`) et le prédicat `BLOCK` (correspondant aux `INSERT INTO`, `UPDATE`, `DELETE`).

Il est même possible de différencier les 3 opérations avec des comportements différents (à vous de voir si c'est pertinent 😂 )

Pour que cela fonctionne pour les requêtes de type `SELECT` il nous reste à ajouter dans la session de contexte SQL l'identifiant locataire à l'aide d'un nouvelle injecteur (Entity Framework Core cette fois) : `TenantFilterDbInterceptor`.

Pour les requêtes de modification `INSERT INTO`, `UPDATE`, `DELETE` nous avons besoin d'un nouvel injecteur qui va assigner la propriété TenantId des entités qui sont modifiées dans le contexte : `TenantBlockDbInterceptor`.

C'est la classe `CallContext` et le mécanisme d'injection de dépendance qui permet de véhiculer ces informations de la couche Web jusqu'à la couche Data.

[Documentation officielle de RLS](https://learn.microsoft.com/en-us/sql/relational-databases/security/row-level-security?view=sql-server-ver16)
