﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Tools.Comparers;

using NUnit.Framework;

namespace Tests.Linq
{
	public class CalculatedColumnTests : TestBase
	{
		[Table(Name="Person")]
		public class PersonCalculated
		{
			[Column, PrimaryKey,  Identity] public int     PersonID   { get; set; } // INTEGER
			[Column, NotNull              ] public string  FirstName  { get; set; } = null!;
			[Column, NotNull              ] public string  LastName   { get; set; } = null!;
			[Column,    Nullable          ] public string? MiddleName { get; set; } // VARCHAR(50)
			[Column, NotNull              ] public char    Gender     { get; set; } // CHARACTER(1)

			[ExpressionMethod(nameof(GetFullNameExpr), IsColumn = true)]
			public string FullName { get; set; } = null!;

			static Expression<Func<PersonCalculated, string>> GetFullNameExpr()
			{
				return p => p.LastName + ", " + p.FirstName;
			}

			[ExpressionMethod(nameof(GetAsSqlFullNameExpr), IsColumn = true)]
			public string AsSqlFullName { get; set; } = null!;

			static Expression<Func<PersonCalculated, string>> GetAsSqlFullNameExpr()
			{
				return p => Sql.AsSql(p.LastName + ", " + p.FirstName);
			}

			[ExpressionMethod(nameof(GetDoctorCountExpr), IsColumn = true)]
			public int DoctorCount { get; set; }

			static Expression<Func<Model.ITestDataContext,PersonCalculated,int>> GetDoctorCountExpr()
			{
				return (db,p) => db.Doctor.Count(d => d.PersonID == p.PersonID);
			}

			public static IEqualityComparer<PersonCalculated> Comparer = ComparerBuilder.GetEqualityComparer<PersonCalculated>();
		}

		[Table("Doctor")]
		public class DoctorCalculated
		{
			[Column, PrimaryKey, Identity] public int    PersonID { get; set; } // Long
			[Column(Length = 50), NotNull] public string Taxonomy { get; set; } = null!; // text(50)

			// Many association for test
			[Association(ThisKey = "PersonID", OtherKey = "PersonID", CanBeNull = false)]
			public IEnumerable<PersonCalculated> PersonDoctor { get; set; } = null!;
		}

		[Test]
		public void CalculatedColumnTest1([DataSources(TestProvName.AllClickHouse)] string context)
		{
			using (var db = GetDataContext(context))
			{
				var q = db.GetTable<PersonCalculated>().Where(i => i.FirstName != "John");
				var l = q.ToList();

				Assert.That(l,                  Is.Not.Empty);
				Assert.Multiple(() =>
				{
					Assert.That(l[0].FullName, Is.Not.Null);
					Assert.That(l[0].AsSqlFullName, Is.Not.Null);
				});
				Assert.Multiple(() =>
				{
					Assert.That(l[0].FullName, Is.EqualTo(l[0].LastName + ", " + l[0].FirstName));
					Assert.That(l[0].AsSqlFullName, Is.EqualTo(l[0].LastName + ", " + l[0].FirstName));
				});
			}
		}

		[Test]
		public void CalculatedColumnTest2([DataSources(TestProvName.AllClickHouse)] string context)
		{
			using (var db = GetDataContext(context))
			{
				var list  = db.GetTable<PersonCalculated>().ToList();
				var query = db.GetTable<PersonCalculated>().Where(i => i.FullName != "Pupkin, John").ToList();

				Assert.That(list, Has.Count.Not.EqualTo(query.Count));

				AreEqual(
					list.Where(i => i.FullName != "Pupkin, John"),
					query,
					PersonCalculated.Comparer);
			}
		}

		[Test]
		public void CalculatedColumnTest3([DataSources(TestProvName.AllClickHouse)] string context)
		{
			using (var db = GetDataContext(context))
			{
				var q = db.GetTable<PersonCalculated>()
					.Where (i => i.FirstName != "John")
					.Select(t => new
					{
						cnt = db.Doctor.Count(d => d.PersonID == t.PersonID),
						t,
					});
				var l = q.ToList();

				Assert.That(l,                    Is.Not.Empty);
				Assert.Multiple(() =>
				{
					Assert.That(l[0].t.FullName, Is.Not.Null);
					Assert.That(l[0].t.AsSqlFullName, Is.Not.Null);
				});
				Assert.Multiple(() =>
				{
					Assert.That(l[0].t.FullName, Is.EqualTo(l[0].t.LastName + ", " + l[0].t.FirstName));
					Assert.That(l[0].t.AsSqlFullName, Is.EqualTo(l[0].t.LastName + ", " + l[0].t.FirstName));
					Assert.That(l[0].t.DoctorCount, Is.EqualTo(l[0].cnt));
				});
			}
		}

		[Test]
		public void CalculatedColumnTest4([DataSources(TestProvName.AllClickHouse)] string context)
		{
			using (var db = GetDataContext(context))
			{
				var q =
					db.GetTable<DoctorCalculated>()
						.SelectMany(d => d.PersonDoctor);
				var l = q.ToList();

				Assert.That(l,                  Is.Not.Empty);
				Assert.That(l[0].AsSqlFullName, Is.Not.Null);
				Assert.That(l[0].AsSqlFullName, Is.EqualTo(l[0].LastName + ", " + l[0].FirstName));
			}
		}

		[Table("Person")]
		public class CustomPerson1
		{
			[ExpressionMethod(nameof(Expr), IsColumn = true)]
			public string? MiddleNamePreview { get; set; }

			private static Expression<Func<CustomPerson1, string?>> Expr()
			{
				return e => Sql.TableField<CustomPerson1, string>(e, "MiddleName").Substring(0, 200);
			}
		}

		[Table("Person")]
		public class CustomPerson2
		{
			[ExpressionMethod(nameof(Expr), IsColumn = true)]
			public string? MiddleNamePreview { get; set; }

			private static Expression<Func<CustomPerson2, string?>> Expr()
			{
				return e => Sql.Property<string>(e, "MiddleName").Substring(0, 200);
			}
		}

		[Test]
		public void CalculatedColumnExpression1([IncludeDataSources(true, TestProvName.AllFirebird, TestProvName.AllClickHouse)] string context)
		{
			using (var db = GetDataContext(context))
			{
				_ = db.GetTable<CustomPerson1>().ToArray();
			}
		}

		[Test]
		public void CalculatedColumnExpression2([IncludeDataSources(true, TestProvName.AllFirebird, TestProvName.AllClickHouse)] string context)
		{
			using (var db = GetDataContext(context))
			{
				_ = db.GetTable<CustomPerson2>().ToArray();
			}
		}
	}
}
