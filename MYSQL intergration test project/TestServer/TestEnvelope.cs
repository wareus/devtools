namespace Devtools.Test.TestServer;

public record TestEnvelope<T>(int Status, T Response);