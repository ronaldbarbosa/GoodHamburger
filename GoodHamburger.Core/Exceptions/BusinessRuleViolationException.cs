namespace GoodHamburger.Core.Exceptions;

public class BusinessRuleViolationException(string message) : Exception(message);