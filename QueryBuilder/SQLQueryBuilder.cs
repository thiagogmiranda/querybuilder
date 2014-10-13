using System;
using System.Collections.Generic;
using QueryBuilder.Enums;
using QueryBuilder.Extensions;

namespace QueryBuilder
{
	public class SQLQueryBuilder
	{
		private List<string> _froms;
		private List<string> _selects;
		private List<string> _wheres;
		private List<string> _orders;

		private bool _distinct;

		private string _prefix;
		private string _case;

		public SQLQueryBuilder()
		{
			_froms = new List<string>();
			_selects = new List<string>();
			_wheres = new List<string>();
			_orders = new List<string>();
		}

		public SQLQueryBuilder Distinct()
		{
			_distinct = true;
			return this;
		}

		public SQLQueryBuilder Select(string column, string alias = null)
		{
			if (string.IsNullOrEmpty(alias))
			{
				_selects.Add(column);
			}
			else
			{
				_selects.Add(string.Format("{0} as {1}", column, alias));
			}

			return this;
		}

		public SQLQueryBuilder Case()
		{
			_case = "case";
			return this;
		}

		public SQLQueryBuilder When(string condition, object value = null)
		{
			if (value != null)
			{
				if (ValueIsNumber(value))
				{
					condition = condition.Replace("?", value.ToString());
				}
				else
				{
					condition = condition.Replace("?", string.Format("'{0}'", value.ToString()));
				}
			}

			_case = string.Format("{0} when {1}", _case, condition);
			return this;
		}

		public SQLQueryBuilder Then(string then)
		{
			_case = string.Format("{0} then {1}", _case, then);
			return this;
		}

		public SQLQueryBuilder EndCase(string alias)
		{
			_case = string.Format("{0} end as {1}", _case, alias);
			_selects.Add(_case);
			_case = null;
			return this;
		}

		public SQLQueryBuilder From(string table, string alias = null)
		{
			if (_froms.Count > 0)
				return this;

			if (string.IsNullOrEmpty(alias))
				_froms.Add(string.Format("{0}", table));
			else
				_froms.Add(string.Format("{0} {1}", table, alias));

			return this;
		}

		public SQLQueryBuilder InnerJoin(string table, string tableAlias, string tableColumn, string joinAlias, string joinColumn)
		{
			this.AddJoin(JoinType.Inner.Texto(), table, tableAlias, tableColumn, joinAlias, joinColumn);
			return this;
		}

		public SQLQueryBuilder LeftJoin(string table, string tableAlias, string tableColumn, string joinAlias, string joinColumn)
		{
			this.AddJoin(JoinType.Left.Texto(), table, tableAlias, tableColumn, joinAlias, joinColumn);
			return this;
		}

		public SQLQueryBuilder RightJoin(string table, string tableAlias, string tableColumn, string joinAlias, string joinColumn)
		{
			this.AddJoin(JoinType.Right.Texto(), table, tableAlias, tableColumn, joinAlias, joinColumn);
			return this;
		}

		private void AddJoin(params string[] joinParams)
		{
			if (_froms.Count == 0)
				throw new Exception("Não é possível fazer join com nenhuma tabela.");

			_froms.Add(FormatJoin(joinParams));
		}

		private string FormatJoin(params string[] joinParams)
		{
			string joinFormat = "{0} {1} {2} on {3}.{4} = {5}.{6}";

			return string.Format(joinFormat, joinParams[0], joinParams[1],
				joinParams[2], joinParams[2], joinParams[3], joinParams[4], joinParams[5]);
		}

		public SQLQueryBuilder Where(string condition, object value = null)
		{
			if (value != null)
			{
				if (ValueIsNumber(value))
				{
					condition = condition.Replace("?", value.ToString());
				}
				else
				{
					condition = condition.Replace("?", string.Format("'{0}'", value.ToString()));
				}
			}

			if (string.IsNullOrEmpty(_prefix))
			{
				_wheres.Add(condition);
			}
			else
			{
				_wheres.Add(string.Format("{0} {1}", _prefix, condition));
				_prefix = null;
			}

			return this;
		}

		public SQLQueryBuilder And()
		{
			_prefix = "and";
			return this;
		}

		public SQLQueryBuilder Or()
		{
			_prefix = "or";
			return this;
		}

		public SQLQueryBuilder OrderBy(string orderColumn, Order order = Order.ASC)
		{
			_orders.Add(string.Format("{0} {1}", orderColumn, order.Texto()));
			return this;
		}

		private bool ValueIsNumber(object value)
		{
			return value is decimal || value is float || value is double || value is int || value is long || value is uint || value is ulong;
		}

		public string Build()
		{
			return this.ToString();
		}

		public override string ToString()
		{
			string queryFormat = "select {0} from {1}";

			if (_distinct)
			{
				queryFormat = "select distinct {0} from {1}";
			}

			var sqlQuery = string.Format(queryFormat, string.Join(", ", _selects), string.Join(" ", _froms));

			if (_wheres.Count > 0)
			{
				sqlQuery = string.Format("{0} where {1}", sqlQuery, string.Join(" ", _wheres));
			}
			if (_orders.Count > 0)
			{
				sqlQuery = string.Format("{0} order by {1}", sqlQuery, string.Join(", ", _orders));
			}

			return sqlQuery;
		}
	}
}
