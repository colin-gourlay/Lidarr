using System;
using System.Linq;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Localization;
using NzbDrone.Core.ThingiProvider.Events;

namespace NzbDrone.Core.HealthCheck.Checks
{
    [CheckOn(typeof(ProviderUpdatedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderDeletedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderStatusChangedEvent<IIndexer>))]
    public class IndexerLongTermStatusCheck : HealthCheckBase
    {
        private readonly IIndexerFactory _providerFactory;
        private readonly IIndexerStatusService _providerStatusService;

        public IndexerLongTermStatusCheck(IIndexerFactory providerFactory,
                                          IIndexerStatusService providerStatusService,
                                          ILocalizationService localizationService)
            : base(localizationService)
        {
            _providerFactory = providerFactory;
            _providerStatusService = providerStatusService;
        }

        public override HealthCheck Check()
        {
            var enabledProviders = _providerFactory.GetAvailableProviders();
            var backOffProviders = enabledProviders.Join(_providerStatusService.GetBlockedProviders(),
                    i => i.Definition.Id,
                    s => s.ProviderId,
                    (i, s) => new { Provider = i, Status = s })
                .Where(p => p.Status.InitialFailure.HasValue &&
                            p.Status.InitialFailure.Value.Before(DateTime.UtcNow.AddHours(-6)))
                .ToList();

            if (backOffProviders.Empty())
            {
                return new HealthCheck(GetType());
            }

            if (backOffProviders.Count == enabledProviders.Count)
            {
                return new HealthCheck(GetType(),
                    HealthCheckResult.Error,
                    _localizationService.GetLocalizedString("IndexerLongTermStatusCheckAllClientMessage"),
                    "#indexers-are-unavailable-due-to-failures");
            }

            return new HealthCheck(GetType(),
                HealthCheckResult.Warning,
                string.Format(_localizationService.GetLocalizedString("IndexerLongTermStatusCheckSingleClientMessage"),
                    string.Join(", ", backOffProviders.Select(v => v.Provider.Definition.Name))),
                "#indexers-are-unavailable-due-to-failures");
        }
    }
}
