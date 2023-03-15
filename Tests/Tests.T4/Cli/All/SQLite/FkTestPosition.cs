// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------

using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Tools.Comparers;
using System;
using System.Collections.Generic;

#pragma warning disable 1573, 1591
#nullable enable

namespace Cli.All.SQLite
{
	[Table("FKTestPosition")]
	public class FkTestPosition : IEquatable<FkTestPosition>
	{
		[Column("Company"   , DataType  = DataType.Int64, DbType   = "integer"        , Length = 8             , Precision = 19, Scale     = 0, IsPrimaryKey = true, PrimaryKeyOrder = 0)] public long   Company    { get; set; } // integer
		[Column("Department", DataType  = DataType.Int64, DbType   = "integer"        , Length = 8             , Precision = 19, Scale     = 0, IsPrimaryKey = true, PrimaryKeyOrder = 1)] public long   Department { get; set; } // integer
		[Column("PositionID", DataType  = DataType.Int64, DbType   = "integer"        , Length = 8             , Precision = 19, Scale     = 0, IsPrimaryKey = true, PrimaryKeyOrder = 2)] public long   PositionId { get; set; } // integer
		[Column("Name"      , CanBeNull = false         , DataType = DataType.NVarChar, DbType = "nvarchar(50)", Length    = 50, Precision = 0, Scale        = 0                        )] public string Name       { get; set; } = null!; // nvarchar(50)

		#region IEquatable<T> support
		private static readonly IEqualityComparer<FkTestPosition> _equalityComparer = ComparerBuilder.GetEqualityComparer<FkTestPosition>(c => c.Company, c => c.Department, c => c.PositionId);

		public bool Equals(FkTestPosition? other)
		{
			return _equalityComparer.Equals(this, other!);
		}

		public override int GetHashCode()
		{
			return _equalityComparer.GetHashCode(this);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as FkTestPosition);
		}
		#endregion
	}
}