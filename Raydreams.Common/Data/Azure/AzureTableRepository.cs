using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Raydreams.Common.Data.Azure
{
    public class AzureTableRepository<T> where T : ITableEntity, new()
	{
		#region [Fields]

		private string _connStr = String.Empty;

		private string _table = String.Empty;

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public AzureTableRepository( string connStr, string table )
		{
			this._connStr = connStr;
			this.TableName = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The physical Collection name</summary>
		public string TableName
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		/// <summary>The physical Collection name</summary>
		public CloudTable AzureTable
		{
			get
			{
				CloudStorageAccount account = CloudStorageAccount.Parse( this._connStr );
				CloudTableClient client = account.CreateCloudTableClient();
				CloudTable tbl = client.GetTableReference( this.TableName );
				return tbl;
			}
		}

		#endregion [Properties]

		/// <summary>Gets ALL the markers in the table</summary>
		/// <returns></returns>
		public List<T> GetAll()
		{
			TableQuerySegment<T> results = null;
			TableContinuationToken tok = new TableContinuationToken();
			results = this.AzureTable.ExecuteQuerySegmentedAsync<T>( new TableQuery<T>(), tok ).GetAwaiter().GetResult();

			return results.Results;
		}

		/// <summary>Updates a single product item</summary>
		public int Update( T item )
		{
			if ( item == null )
				return 0;

			TableOperation op = TableOperation.Replace( item );
			TableResult results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();

			return results.HttpStatusCode;
		}

		/// <summary></summary>
		/// <param name="items"></param>
		public int Insert( List<T> items )
		{
			TableResult results = null;

			foreach ( T item in items )
			{
				try
				{
					TableOperation op = TableOperation.InsertOrMerge( item );
					results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();
				}
				catch ( System.Exception exp )
				{
					; // log it somehow and keep going
				}
			}

			return results.HttpStatusCode;
		}
	}
}
