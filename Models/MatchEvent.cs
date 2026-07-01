namespace sports_api.Models;

public enum EventType
{
    Goal,
    Point,
    Foul,
    Card,
    Substitution
}

public class MatchEvent
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid MatchId { get; set; }
    public Match? Match { get; set; }

    public Guid TeamId { get; set; }
    public Team? Team { get; set; }

    public Guid? PlayerId { get; set; }  // opcional: no todo evento tiene un jugador (ej. falta técnica al equipo)
    public Player? Player { get; set; }

    public EventType Type { get; set; }
    public int Points { get; set; }       // cuánto suma al score: 1 gol fútbol, 2/3 basquetbol, 0 para tarjetas/faltas
    public int? Minute { get; set; }      // opcional, nullable porque no todo deporte usa minutos (ej. innings de baseball serían distinto)
}