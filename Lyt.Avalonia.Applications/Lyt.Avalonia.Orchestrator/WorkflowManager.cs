namespace Lyt.Avalonia.Orchestrator;

public sealed class WorkflowManager<TState, TTrigger> 
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public const int DefaultAnimationDuration = 666; // milliseconds
    public const int MinimumAnimationDuration = 100; // milliseconds

    private readonly ILogger logger;
    private readonly FiniteStateMachine<TState, TTrigger, Bindable> stateMachine;
    private readonly Dictionary<TState, WorkflowPage<TState, TTrigger>> pageIndex;
    private readonly Stack<WorkflowPage<TState, TTrigger>> navigationStack;

    public ContentControl HostControl { get; private set; }

    public WorkflowManager(ILogger logger, ContentControl hostControl)
    {
        this.logger = logger;   
        this.HostControl = hostControl;
        this.stateMachine = new (this.logger);
        this.pageIndex = [];
        this.navigationStack = new ();
    }

    public WorkflowPage<TState, TTrigger>? GetPage(TState state)
        => this.pageIndex.TryGetValue(state, out var workflowPage) ? workflowPage : null;

    public WorkflowPage<TState, TTrigger>? ActivePage { get; private set; }

    public WorkflowPage<TState, TTrigger>? PreviousPage
        => this.navigationStack.Count > 0 ? this.navigationStack.Peek() : null;

    public Move IsMoving { get; private set; }

    public bool IsTransitioning => this.IsMoving != Move.NotMoving;

    public void CreateWorkflowPages(IEnumerable<WorkflowPage<TState, TTrigger>> pages)
    {
        if (pages.Count() < 2)
        {
            this.logger.Error("pages is empty or has only one element");
            throw new ArgumentException("Empty or has only one element", nameof(pages));
        }

        try
        {
            //bool isFirst = true;
            //foreach (var page in pages)
            //{
            //    this.machine.CreateState(page.State, isStart: isFirst);
            //    this.pageIndex.Add(page.State, page);
            //    this.navigationGrid.Children.Add(page.FrameworkElement);
            //    page.WorkflowHost = this;
            //    var element = page.FrameworkElement;
            //    if (element != null)
            //    {
            //        element.Visibility = isFirst ? Visibility.Visible : Visibility.Hidden;
            //    }

            //    isFirst = false;
            //}

            //foreach (var page in pages)
            //{
            //    foreach (var triggerKvp in page.Triggers)
            //    {
            //        var transition = triggerKvp.Key;
            //        var func = triggerKvp.Value;
            //        var target = page.Targets[transition];
            //        bool isBackNavigation = page.BackNavigation[transition];
            //        this.machine.CreateTransition(transition, page.State, target, func, isBackNavigation);
            //    }
            //}
        }
        catch (Exception e)
        {
            this.logger.Error("Improperly defined state machine:\n" + e.ToString());
        }
    }

    public void Initialize()
    {
        foreach (var page in this.pageIndex.Values)
        {
            page.OnInitialize();
        }
    }

    public async Task Shutdown()
    {
        foreach (var page in this.pageIndex.Values)
        {
            await page.OnShutdown();
        }
    }

    public async Task Start()
    {
        // this.stateMachine.Start();
        var newState = this.stateMachine.State;
        var activated = await this.ActivatePage(newState);
        // this.OnTransition?.Invoke(null, activated);
        this.UpdateVisuals();
    }

    public void UpdateVisuals() { }  // => this.UpdateVisuals();

    public void ClearBackNavigation() => this.navigationStack.Clear();

    public bool CanGoBack()
    {
        // check first if the state machine has back navigation,
        if (this.stateMachine.HasBackNavigation(out var _))
        {
            // The state machine has back navigation: check if back navigation is valid
            // Discard new state, as this point, it is not needed
            return this.stateMachine.CanGoBack(out _);
        }
        else
        {
            // Regular case, no back navigation: we can go back if we are not transitioning
            // and the navigation stack is not empty
            return !this.IsTransitioning && (this.navigationStack.Count > 0);
        }
    }

    public async Task<bool> TryGoBack(int fadeDuration = 250)
    {
        this.IsMoving = Move.NotMoving;
        var oldState = this.stateMachine.State;
        TState newState;
        if (this.stateMachine.HasBackNavigation(out var _))
        {
            if (!this.stateMachine.CanGoBack(out newState))
            {
                string message1 =
                    string.Format("Cant go back from: {0} to {1}", oldState.ToString(), newState.ToString());
                this.logger.Info(message1);
                return false;
            }

            this.navigationStack.Clear();
        }
        else
        {
            var previousPage = this.navigationStack.Count > 0 ? this.navigationStack.Pop() : null;
            if (previousPage == null)
            {
                string message3 =
                    string.Format("No back navigation and no previous page for: {0} ", oldState.ToString());
                this.logger.Info(message3);
                return false;
            }

            newState = previousPage.State;
        }

        this.IsMoving = Move.Backward;
        this.UpdateVisuals();
        // this.machine.JumpTo(newState);
        var deactivated = await this.DeactivatePage(oldState, fadeDuration);
        var activated = await this.ActivatePage(newState, fadeDuration);
        // this.OnTransition?.Invoke(deactivated, activated);
        this.IsMoving = Move.NotMoving;
        this.UpdateVisuals();

        string message2 =
            string.Format("Backwards workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
        this.logger.Info(message2);
        return true;
    }

    public bool CanAdvance(out TState state)
    {
        if (this.IsTransitioning)
        {
            state = default;
            return false;
        }

        return this.stateMachine.CanAdvance(out state);
    }

    public async Task<bool> TryAdvance(int fadeDuration = 250)
    {
        this.IsMoving = Move.NotMoving;
        this.UpdateVisuals();
        var oldState = this.stateMachine.State;
        bool advance = this.stateMachine.TryAdvance(out TState newState);
        if (advance)
        {
            this.IsMoving = Move.Forward;
            this.UpdateVisuals();
            var deactivated = await this.DeactivatePage(oldState, fadeDuration);
            if (deactivated != null)
            {
                this.navigationStack.Push(deactivated);
            }

            var activated = await this.ActivatePage(newState, fadeDuration);
            // this.OnTransition?.Invoke(deactivated, activated);
            this.IsMoving = Move.NotMoving;
            this.UpdateVisuals();

            string message =
                string.Format("Forward workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
            this.logger.Info(message);
        }

        return advance;
    }

    private async Task<WorkflowPage<TState, TTrigger>?> DeactivatePage(
        TState oldState, int fadeDuration = DefaultAnimationDuration)
    {
        this.logger.Info("Orchestrator: Deactivating " + oldState.ToString());
        var deactivated = this.ActivePage;
        if (this.ActivePage != null)
        {
            await this.ActivePage.OnDeactivateAsync(oldState);
            if (fadeDuration > MinimumAnimationDuration)
            {
                // Fading out hides the control at the end of the animation
                // this.ActivePage.Control!.FadeOut(fadeDuration);
            }
            else
            {
                this.ActivePage.Control!.IsVisible = false;
            }

            // Raise the Navigate weak event so that workflow popups, if any, will dismiss.
            // We have none for now
            // ApplicationEvent.Raise(ApplicationEvent.Kind.Navigate);
        }

        return deactivated;
    }

    private async Task<WorkflowPage<TState, TTrigger>> ActivatePage(
        TState newState, int fadeDuration = DefaultAnimationDuration)
    {
        // Get the new page, activates it
        this.logger.Info("Orchestrator: Activating " + newState.ToString());
        this.ActivePage = this.pageIndex[newState];
        if (fadeDuration > MinimumAnimationDuration)
        {
            // Fading in shows the control at the beginning of the animation
            // this.ActivePage.Control!.FadeIn(fadeDuration);
        }
        else
        {
            this.ActivePage.Control!.IsVisible = true;
        }

        await this.ActivePage.OnActivateAsync(newState);
        return this.ActivePage;
    }

    private void BackButtonClick(object sender, RoutedEventArgs rea)
    {
        if (!this.CanGoBack())
        {
            return;
        }

        var _ = this.TryGoBack();
    }

    private void NextButtonClick(object sender, RoutedEventArgs rea)
    {
        if (!this.CanAdvance(out var _))
        {
            return;
        }

        var _ = this.TryAdvance();
    }

    public void Next(int fadeDuration = 250) => this.Do(Step.Next, fadeDuration);

    public void Back(int fadeDuration = 250) => this.Do(Step.Back, fadeDuration);

    public void Action() => this.Do(Step.Action, 0);

    private async void Do(Step step, int fadeDuration)
    {
        switch (step)
        {
            case Step.Back:
                if (this.CanGoBack())
                {
                    var _ = this.TryGoBack(fadeDuration);
                }

                break;

            case Step.Next:
                if (this.CanAdvance(out var _))
                {
                    var _ = this.TryAdvance(fadeDuration);
                }

                break;

            case Step.Action:
                var activePage = this.ActivePage;
                if (activePage != null)
                {
                    await activePage.OnAction();
                }

                break;
        }
    }
}