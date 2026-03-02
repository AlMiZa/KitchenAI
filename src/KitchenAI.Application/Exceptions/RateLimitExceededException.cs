namespace KitchenAI.Application.Exceptions;

/// <summary>Thrown when a household has exceeded the recipe generation rate limit.</summary>
public class RateLimitExceededException(string message) : Exception(message);
