namespace Pulsus
{
	public class SqlInformation
	{
		public static SqlInformation Create(string sql, object parameters = null)
		{
			return new SqlInformation()
			{
				SQL = sql,
				Parameters = parameters
			};
		}

		public string SQL { get; set; }
		public object Parameters { get; set; }
	}
}
