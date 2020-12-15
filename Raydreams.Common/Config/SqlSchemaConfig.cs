using System;
using System.Configuration;

namespace Raydreams.Common.Config
{
    /// <summary>Base collection of tables</summary>
    public class SqlSchemaConfig : ConfigurationSection
    {
        [ConfigurationProperty( "tables", IsDefaultCollection = true )]
        public TableConfigCollection Tables
        {
            get
            {
                return base["tables"] as TableConfigCollection;
            }
        }
    }

    /// <summary>Collection of Schema Table definitions.</summary>
    [ConfigurationCollection( typeof( TableConfigDefinition ), AddItemName = "table", CollectionType = ConfigurationElementCollectionType.BasicMap )]
    public class TableConfigCollection : ConfigurationElementCollection
    {
        /// <summary></summary>
        public TableConfigDefinition this[int index]
        {
            get { return (TableConfigDefinition)BaseGet( index ); }
            set
            {
                if (BaseGet( index ) != null)
                {
                    BaseRemoveAt( index );
                }
                BaseAdd( index, value );
            }
        }

        /// <summary></summary>
        public new TableConfigDefinition this[string key]
        {
            get
            {
                if (IndexOf( key ) < 0)
                    return null;

                return BaseGet( key ) as TableConfigDefinition;
            }
        }

        /// <summary></summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int IndexOf(string id)
        {
            id = id.ToLower();

            for (int idx = 0; idx < base.Count; idx++)
            {
                if (this[idx].ID.ToLower() == id)
                    return idx;
            }
            return -1;
        }

        /// <summary></summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary></summary>
        public void Add(TableConfigDefinition serviceConfig)
        {
            BaseAdd( serviceConfig );
        }

        /// <summary></summary>
        public void Remove(string name)
        {
            BaseRemove( name );
        }

        /// <summary></summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TableConfigDefinition();
        }

        /// <summary></summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ( (TableConfigDefinition)element ).ID;
        }
    }

    /// <summary>A collection of column definitions</summary>
    [ConfigurationCollection( typeof( ColConfigDefinition ), AddItemName = "column", CollectionType = ConfigurationElementCollectionType.BasicMap )]
    public class ColumnConfigCollection : ConfigurationElementCollection
    {
        /// <summary></summary>
        public ColConfigDefinition this[int index]
        {
            get
            {
                return (ColConfigDefinition)BaseGet( index );
            }
            set
            {
                if (BaseGet( index ) != null)
                {
                    BaseRemoveAt( index );
                }
                BaseAdd( index, value );
            }
        }

        /// <summary></summary>
        public new ColConfigDefinition this[string key]
        {
            get
            {
                if (IndexOf( key ) < 0)
                    return null;

                return BaseGet( key ) as ColConfigDefinition;
            }
        }

        /// <summary></summary>
        public int IndexOf(string id)
        {
            id = id.ToLower();

            for (int idx = 0; idx < base.Count; idx++)
            {
                if (this[idx].ID.ToLower() == id)
                    return idx;
            }
            return -1;
        }

        /// <summary></summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary></summary>
        public void Add(ColConfigDefinition serviceConfig)
        {
            BaseAdd( serviceConfig );
        }

        /// <summary></summary>
        public void Remove(string name)
        {
            BaseRemove( name );
        }

        /// <summary></summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ColConfigDefinition();
        }

        /// <summary></summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ( (ColConfigDefinition)element ).ID;
        }

    }

    /// <summary>A schema table definition.</summary>
    public class TableConfigDefinition : ConfigurationElement
    {
        public TableConfigDefinition() { }

        /// <summary></summary>
        [ConfigurationProperty( "id", IsRequired = true, IsKey = true )]
        public string ID
        {
            get { return this["id"].ToString(); }
            set { this["id"] = value; }
        }

        /// <summary></summary>
        public string FullName
        {
            get { return String.Format( "[{0}].[{1}]", this.Schema, this.Object ); }
        }

        /// <summary></summary>
        [ConfigurationProperty( "schema" )]
        public string Schema
        {
            get { return this["schema"].ToString(); }
            set { this["schema"] = value; }
        }

        /// <summary></summary>
        [ConfigurationProperty( "object" )]
        public string Object
        {
            get { return this["object"].ToString(); }
            set { this["object"] = value; }
        }

        /// <summary></summary>
        [ConfigurationProperty( "columns", IsDefaultCollection = false )]
        public ColumnConfigCollection Columns
        {
            get { return base["columns"] as ColumnConfigCollection; }
        }
    }

    /// <summary>A schema column definition</summary>
    public class ColConfigDefinition : ConfigurationElement
    {
        public ColConfigDefinition() { }

        /// <summary></summary>
        [ConfigurationProperty( "id", IsRequired = true, IsKey = true )]
        public string ID
        {
            get { return this["id"].ToString(); }
            set { this["id"] = value; }
        }

        [ConfigurationProperty( "name", IsRequired = true )]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }
    }
}
