# Raydreams.Common

This is just a common C# .NET Core Library I have been using for years, changing and refactoring as needed.

Classes come and go as the .NET framework includes more stuff.

I mainly use to connect to various data sources like SQL Server, SQLite, MongoDB and Azure since I hate using bloated ORMs like Entity Framework. The pattern I use here is super simple.

However, it also contains a lot of common code I use but it is in need of a lot of refactoring and clean-up.

It's not meant for public consumption but you are welcome to look at it and use anything you see.

At some point I may document it more thoughly - but probably not because it's really just for my own personal use for now.

## Raydreams.Common.Data

Since most SQL to Object ORMs tend to be over bloated beasts (I'm looking at you Entity Framework), this was orginally written to be a simple adorn your Data Objects with atteibutes approach much like is find in the MongoDB driver or JSON drivers. However, the base condidtion is that it will simply try to map data fields to properties. Only adding the RayProprety attribute will override and use the attributes then.

It was also designed for multiple contexes because I was using the same DO for both input and output in data integration. An input source might be a CSV file where the output was SQL Server or maybe a tabbed file but with differnt field names.

And finally, in some use cases, the input source was no consistent with field names which may vary slightly like fname or First_Name.
	
~~~~
/// <summary>A simple test object to read from SQL</summary>
public class TestObject
{
	public int ID { get; set; }

	public Guid GUID { get; set; }

	public DateTime Timestamp { get; set; }

	public bool? IsTrue { get; set; }

	public double? Big { get; set; }
}

/// <summary>Inherit from SQLDataManager</summary>
public class TestRepository : SQLDataManager
{
	#region [Fields]

	private string _table = "[dbo].[Tests]";

	#endregion [Fields]

	#region [Constructors]

	/// <summary></summary>
	/// <param name="connStr">The Connection String to the DB</param>
	/// <param name="tableName">The SQL Table Name</param>
	public TestRepository(string connStr, string tableName) : base(connStr)
	{
		this.TableName = tableName;
	}

	#endregion [Constructors]

	#region [Properties]

	/// <summary>Name of the Table</summary>
	public string TableName
	{
		get { return this._table; }
		set
		{
			if (!String.IsNullOrWhiteSpace(value))
				this._table = value.Trim();
		}
	}

	#endregion [Properties]

	#region [Methods]

	/// <summary>Just gets all the records</summary>
	/// <returns></returns>
	public List<Test> GetAll()
	{
		return this.SelectAll<TestObject>(this.TableName);
	}

	#endregion [Methods]
}

// Where the magic happens
TestRepository repo = new TestRepository("myConnectionString", "[dbo].[Tests]");
List<TestObject> results = repo.GetAll();
~~~~
