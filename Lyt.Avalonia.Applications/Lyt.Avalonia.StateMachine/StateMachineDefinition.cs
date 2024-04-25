namespace Lyt.Avalonia.StateMachine;

public sealed class StateMachineDefinition<TState, TTrigger>(
    TState initialState,
    List<StateDefinition<TState, TTrigger>> stateDefinitions,
    Action<TState, TState>? onStateChanged = null)
        where TState : struct, Enum
        where TTrigger : struct, Enum
{
    public TState InitialState { get; private set; } = initialState;

    public List<StateDefinition<TState, TTrigger>> StateDefinitions { get; private set; } = stateDefinitions;

    public Action<TState, TState>? OnStateChanged { get; private set; } = onStateChanged;
}
