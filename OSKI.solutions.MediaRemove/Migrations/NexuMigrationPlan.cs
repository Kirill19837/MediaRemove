﻿using MediaRemove.Migrations.Version_2_0_0;
using Umbraco.Cms.Infrastructure.Migrations;

namespace MediaRemove.Migrations
{
    internal class NexuMigrationPlan : MigrationPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NexuMigrationPlan"/> class.
        /// </summary>
        public NexuMigrationPlan()
            : base("Our.Umbraco.Nexu")
        {
            this.InitialInstall();
        }

        /// <summary>
        /// Gets the initial state.
        /// </summary>
        public override string InitialState => string.Empty;

        /// <summary>
        /// Runs the migration for a initial install
        /// </summary>
        private void InitialInstall()
        {
            this.From(this.InitialState).To<CreateRelationTableMigration>("2.0.0-Initial");
        }
    }
}
