﻿using System;
using System.Collections.Generic;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Notifications.Webhook
{
    public class WebhookSettingsValidator : AbstractValidator<WebhookSettings>
    {
        public WebhookSettingsValidator()
        {
            RuleFor(c => c.Url).IsValidUrl();
        }
    }

    public class WebhookSettings : IProviderConfig
    {
        private static readonly WebhookSettingsValidator Validator = new WebhookSettingsValidator();

        public WebhookSettings()
        {
            Method = Convert.ToInt32(WebhookMethod.POST);
            Headers = new List<KeyValuePair<string, string>>();
        }

        [FieldDefinition(0, Label = "URL", Type = FieldType.Url)]
        public string Url { get; set; }

        [FieldDefinition(1, Label = "Method", Type = FieldType.Select, SelectOptions = typeof(WebhookMethod), HelpText = "Which HTTP method to use submit to the Webservice")]
        public int Method { get; set; }

        [FieldDefinition(2, Label = "Username", Privacy = PrivacyLevel.UserName)]
        public string Username { get; set; }

        [FieldDefinition(3, Label = "Password", Type = FieldType.Password, Privacy = PrivacyLevel.Password)]
        public string Password { get; set; }

        [FieldDefinition(4, Label = "NotificationsSettingsWebhookHeaders", Type = FieldType.KeyValueList, Advanced = true)]
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
