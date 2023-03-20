namespace MediaRemove.Constants
{
    public static class DatabaseConstants
    {
        public const string TableName = "Nexu_Relations";

        public const string IdColumn = "id";

        public const string PrimaryKey = "PK_" + TableName;

        public const string ParentUdiColumn = "parent_udi";

        public const string ChildUdiColumn = "child_udi";

        public const string RelationTypeColumn = "relation_type";

        public const string PropertyAlias = "property_alias";

        public const string CultureColumn = "culture_column";

        public const string ParentUdiIndex = "IX_" + TableName + "_" + ParentUdiColumn;

        public const string ChildUdiIndex = "IX_" + TableName + "_" + ChildUdiColumn;
    }
}
