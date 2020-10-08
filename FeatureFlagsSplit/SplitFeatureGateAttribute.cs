using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Splitio.Services.Client.Interfaces;

namespace FeatureFlagsSplit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SplitFeatureGateAttribute : ActionFilterAttribute
    {
        public SplitFeatureGateAttribute(params string[] features) : this(RequirementType.All, features)
        {
        }

        public SplitFeatureGateAttribute(params object[] features) : this(RequirementType.All, features)
        {
        }

        public SplitFeatureGateAttribute(RequirementType requirementType, params string[] features)
        {
            if (features == null || features.Length == 0)
                throw new ArgumentNullException(nameof(features));
            Features = features.ToList();
            RequirementType = requirementType;
        }

        public SplitFeatureGateAttribute(RequirementType requirementType, params object[] features)
        {
            if (features == null || features.Length == 0)
                throw new ArgumentNullException(nameof(features));
            var stringList = new List<string>();
            foreach (var feature in features)
            {
                if (!feature.GetType().IsEnum)
                    throw new ArgumentException("The provided features must be enums.", nameof(features));
                stringList.Add(Enum.GetName(feature.GetType(), feature));
            }
            Features = stringList;
            RequirementType = requirementType;
        }

        public List<string> Features { get; }
        public RequirementType RequirementType { get; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var splitClient = context.HttpContext.RequestServices.GetRequiredService<ISplitClient>();

            var treatments = splitClient.GetTreatments("ANONYMOUS_USER", Features);
            var flag = RequirementType == RequirementType.All ? treatments.Values.All(v => v == "on") : treatments.Values.Any(v => v == "on");

            if (!flag)
                context.Result = new StatusCodeResult(403);
        }
    }
    public enum RequirementType
    {
        Any,
        All,
    }
}
