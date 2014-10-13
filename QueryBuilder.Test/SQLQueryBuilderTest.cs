using System;
using NUnit.Framework;
using QueryBuilder.Enums;

namespace QueryBuilder
{
	[TestFixture]
	public class SQLQueryBuilderTest
	{
		[Test]
		public void DeveGerarQuerySimples()
		{
			string queryEsperada = "select * from Usuario";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*").From("Usuario");

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		public void DeveGerarQueryComDistinct()
		{
			string queryEsperada = "select distinct * from Usuario";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*").Distinct().From("Usuario");

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void DeveLancarExcecaoAoAdicionarJoinSemTabelaParaCruzar()
		{
			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*")
				.InnerJoin("Debitante", "db", "IdDebitante", "d", "IdDebitante")
				.Build();
		}

		[Test]
		public void DeveGerarQueryComJoins()
		{
			string queryEsperada = "select * from Debito d inner join Debitante db on db.IdDebitante = d.IdDebitante left join Contribuinte c on c.IdContribuinteISSQN = db.IdContribuinteISSQN";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*")
				.From("Debito", "d")
				.InnerJoin("Debitante", "db", "IdDebitante", "d", "IdDebitante")
				.LeftJoin("Contribuinte", "c", "IdContribuinteISSQN", "db", "IdContribuinteISSQN");

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		public void DeveGerarQueryComCondicao()
		{
			string queryEsperada = "select * from Usuario u where u.IdUsuario = 1";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*").From("Usuario", "u").Where("u.IdUsuario = ?", 1);

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		public void DeveGerarQueryComMaisDeUmaCondicao()
		{
			string queryEsperada = "select * from Usuario u where u.IdUsuario = 1 and u.Nome = 'Miranda' or u.Login = '13338011733'";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*")
				.From("Usuario", "u")
				.Where("u.IdUsuario = ?", 1)
				.And()
				.Where("u.Nome = ?", "Miranda")
				.Or()
				.Where("u.Login = ?", "13338011733");

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		public void DeveGerarUmaQueryComOrdenacao()
		{
			string queryEsperada = "select * from Usuario u order by u.Nome asc";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*").From("Usuario", "u").OrderBy("u.Nome", Order.ASC);

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		public void DeveGerarUmaQueryComOrdenacaoMultipla()
		{
			string queryEsperada = "select * from Usuario u order by u.Nome asc, u.Login desc";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Select("*").From("Usuario", "u").OrderBy("u.Nome", Order.ASC).OrderBy("u.Login", Order.DESC);

			Assert.AreEqual(queryEsperada, builder.ToString());
		}

		[Test]
		public void DeveGerarQueryComCaseEmSelects()
		{
			string queryEsperada = "select case when d.IdDebito IS NOT NULL then 'Debito' end as Debito from Debitante d";

			SQLQueryBuilder builder = new SQLQueryBuilder();
			builder.Case()
				.When("d.IdDebito IS NOT NULL")
				.Then("'Debito'")
				.EndCase("Debito")
				.From("Debitante", "d");

			Assert.AreEqual(queryEsperada, builder.ToString());
		}
	}
}
