using System.Collections.Generic;

public class AzurePipelinesSecretStepsAttribute : AzurePipelinesStepsAttribute
{
    public string[] Secrets { get; set; } = new string[0];

    public bool EnableAccessToken { get; set; }

    protected virtual IEnumerable<(string Key, string Value)> GetImports()
    {
        static string GetSecretValue(string secret) => $"$({secret})";

        if (EnableAccessToken)
        {
            yield return ("SYSTEM_ACCESSTOKEN", GetSecretValue("System.AccessToken"));
        }

        foreach (var secret in Secrets)
        {
            yield return (secret, GetSecretValue(secret));
        }
    }
}