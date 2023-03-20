using MediaRemove.Models.Nexu;
using Umbraco.Cms.Infrastructure.Migrations;

namespace MediaRemove.Migrations.Version_2_0_0
{
    internal class CreateRelationTableMigration : MigrationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRelationTableMigration"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public CreateRelationTableMigration(IMigrationContext context)
            : base(context)
        {
        }

        /// <inheritdoc />
        protected override void Migrate()
        {
            Create.Table(DatabaseConstants.TableName)
                .WithColumn(DatabaseConstants.IdColumn).AsGuid().PrimaryKey(DatabaseConstants.PrimaryKey).NotNullable()
                .WithColumn(DatabaseConstants.ParentUdiColumn).AsString(100).NotNullable()
                .WithColumn(DatabaseConstants.ChildUdiColumn).AsString(100).NotNullable()
                .WithColumn(DatabaseConstants.RelationTypeColumn).AsGuid().NotNullable()
                .WithColumn(DatabaseConstants.PropertyAlias).AsString(100).NotNullable()
                .WithColumn(DatabaseConstants.CultureColumn).AsString(32).Nullable()
                .Do();

            Create.Index(DatabaseConstants.ParentUdiIndex).OnTable(DatabaseConstants.TableName).WithOptions().NonClustered()
                .OnColumn(DatabaseConstants.ParentUdiColumn).Ascending().Do();

            Create.Index(DatabaseConstants.ChildUdiIndex).OnTable(DatabaseConstants.TableName).WithOptions().NonClustered()
                .OnColumn(DatabaseConstants.ChildUdiColumn).Ascending().Do();
        }
    }
}
