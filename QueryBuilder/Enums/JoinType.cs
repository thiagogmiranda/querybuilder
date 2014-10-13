
using QueryBuilder.Attributes;

namespace QueryBuilder.Enums
{
	public enum JoinType
	{
		[Texto("inner join")]
		Inner,
		[Texto("left join")]
		Left,
		[Texto("right join")]
		Right
	}
}
