using MediaRemove.Migrations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace MediaRemove.Composing.Components
{
    internal class MigrationComponent : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;

        public MigrationComponent(IScopeProvider scopeProvider, IKeyValueService keyValueService, IMigrationPlanExecutor migrationPlanExecutor)
        {
            _scopeProvider = scopeProvider;
            _keyValueService = keyValueService;
            _migrationPlanExecutor = migrationPlanExecutor;
        }

        public void Initialize()
        {
            var upgrader = new Upgrader(new NexuMigrationPlan());

            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public void Terminate()
        {            
        }
    }
}
