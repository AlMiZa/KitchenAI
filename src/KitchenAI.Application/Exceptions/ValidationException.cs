namespace KitchenAI.Application.Exceptions;

/// <summary>Thrown when input validation fails (e.g., duplicate email, missing required field).</summary>
public class ValidationException(string message) : Exception(message);
