# Raydreams.Common

## Disclaimer

This repo is not meant for public consumption but you are welcome to look at it and use/copy anything you see.

At some point I may document it more thoughly and certainly add more unit tests - but for now it's really just for my own personal use.

## Description

This is just a common C# .NET Core Library I started years ago, changing, adding to, refactoring as needed.

Classes have come and gone .NET framework includes more stuff or changes. I mean, Server Side Controls, SOAP and XML were all the rage when I started this.

I mainly use to connect to various data sources like SQL Server, SQLite, MongoDB and Azure since I hate using bloated ORMs like Entity Framework. The pattern I use here is super simple, repeatable and easy to mock without any worries EF will break on its next version update. Seriously, if you are in love with EF you probably haven't been using it for very long.

## SQLSchemaConfig

SQL Schema Config can be used to inject physical SQL table and field names from something like app.config like:

```
  <SQLObjects>
    <tables>
      <table id="ContractorsTable" schema="dbo" object="Contractors" />
      <table id="ConversionsTable" schema="meta" object="Conversions" />
      <table id="CountryTable" schema="dbo" object="CountryCodes">
        <columns>
          <column id="CountryCodeCol" name="Code3" />
          <column id="CountryNameCol" name="Name" />
        </columns>
      </table>
      <table id="LocationTable" schema="dbo" object="SiteLocations" />
      <table id="LogTable" schema="meta" object="Logs" />
      <table id="NoActionsTable" schema="meta" object="NoActions" />
    </tables>
  </SQLObjects>
```

It's mainly used with exe chron jobs.

## Raydreams.Common.Data

Since most SQL to Object ORMs tend to be over bloated beasts (I'm looking at you Entity Framework), this was orginally written to be a simple adorn your Data Objects with attributes approach much like is find in the MongoDB driver or JSON drivers. However, the base condidtion is that it will simply try to map data fields to properties. Only adding the RayProprety attribute will override and use the attributes then.

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

Right now there are some refactoring issues I have not fully tested because I've change the Attribute class a few times.