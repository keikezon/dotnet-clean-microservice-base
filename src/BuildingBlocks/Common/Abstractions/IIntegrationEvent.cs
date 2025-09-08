namespace Common.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }           // Identificador único do evento
    DateTime OccurredAt { get; } // Data/hora em que o evento ocorreu
}