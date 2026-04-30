using Microsoft.Extensions.Localization;
using Infrastructure.Localization.Resources;
using Application.Common.Interfaces.Localization;

namespace Infrastructure.Localization;

internal sealed class ErrorLocalizer(
    IStringLocalizer<ErrorResource> localizer) : IErrorLocalizer
{
    public string this[string key] => localizer[key].Value;


    public string Format(string key, params object[] args)
    {
        var template = localizer[key].Value;
        return args.Length == 0 ? template : string.Format(template, args);
    }
}
