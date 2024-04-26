namespace Lyt.Avalonia.Orchestrator;

public sealed class WorkflowManager<TState, TTrigger>
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public const int DefaultAnimationDuration = 0; // milliseconds
    public const int MinimumAnimationDuration = 100; // milliseconds

    private readonly ILogger logger;
    private readonly IMessenger messenger;
    private readonly FiniteStateMachine<TState, TTrigger, Bindable> stateMachine;
    private readonly Dictionary<TState, WorkflowPage<TState, TTrigger>> pageIndex;

    public ContentControl HostControl { get; private set; }

    public WorkflowManager(ILogger logger, IMessenger messenger, ContentControl hostControl)
    {
        this.logger = logger;
        this.messenger = messenger;
        this.HostControl = hostControl;
        this.stateMachine = new(this.logger);
        this.pageIndex = [];
    }

    public WorkflowPage<TState, TTrigger>? GetPage(TState state)
        => this.pageIndex.TryGetValue(state, out var workflowPage) ? workflowPage : null;

    public WorkflowPage<TState, TTrigger>? ActivePage { get; private set; }

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
            // TODO 
            //
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
        this.UpdateVisuals();

        // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
        this.messenger.Publish(new NavigationMessage(activated, null));
    }

    public void UpdateVisuals() { /* TODO */ }  // => this.UpdateVisuals();

    public void ClearBackNavigation() => this.stateMachine.ClearBackNavigation();

    public async Task<bool> Next(int fadeDuration = DefaultAnimationDuration)
    {
        if (this.CanGoNext(out var _, out var _))
        {
            return await this.TryGoNext(fadeDuration);
        }

        return false;
    }

    public async Task<bool> Back(int fadeDuration = DefaultAnimationDuration)
    {
        if (this.CanGoBack(out TState _))
        {
            return await this.TryGoBack(fadeDuration);
        }

        return false;
    }

    public async Task<bool> Fire(TTrigger trigger, int fadeDuration = DefaultAnimationDuration)
    {
        if (this.CanFire(trigger))
        {
            return await this.TryFire(trigger, fadeDuration);
        }

        return false;
    }

    public bool CanGoBack(out TState state)
    {
        // We can go back if we are not transitioning and the navigation stack is not empty
        state = this.stateMachine.State;
        return !this.IsTransitioning && this.stateMachine.CanGoBack(out state);
    }

    public bool CanGoNext(out TState state, out TTrigger trigger)
    {
        state = default;
        trigger = default;
        if (this.IsTransitioning)
        {
            return false;
        }

        return this.stateMachine.CanGoNext(out state, out trigger);
    }

    public bool CanFire(TTrigger trigger)
    {
        // TODO 
        return true;
    }

    private async Task<bool> TryGoBack(int fadeDuration = DefaultAnimationDuration)
    {
        this.IsMoving = Move.NotMoving;
        var oldState = this.stateMachine.State;
        if (!this.CanGoBack(out TState newState))
        {
            this.logger.Debug(string.Format("Cannot go back from: {0} ", oldState.ToString()));
            return false;
        }

        this.IsMoving = Move.Backward;
        this.UpdateVisuals();
        this.stateMachine.GoBack();
        var deactivated = await this.DeactivatePage(oldState, fadeDuration);
        var activated = await this.ActivatePage(newState, fadeDuration);
        this.IsMoving = Move.NotMoving;
        this.UpdateVisuals();

        // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
        this.messenger.Publish(new NavigationMessage(activated, deactivated));

        string message =
            string.Format("Backwards workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
        this.logger.Info(message);
        return true;
    }

    private async Task<bool> TryGoNext(int fadeDuration = DefaultAnimationDuration)
    {
        this.IsMoving = Move.NotMoving;
        this.UpdateVisuals();
        var oldState = this.stateMachine.State;
        bool canGoNext = this.stateMachine.CanGoNext(out TState newState, out TTrigger trigger);
        if (canGoNext)
        {
            this.IsMoving = Move.Forward;
            this.UpdateVisuals();
            this.stateMachine.GoNext(trigger);
            var deactivated = await this.DeactivatePage(oldState, fadeDuration);
            var activated = await this.ActivatePage(newState, fadeDuration);
            this.IsMoving = Move.NotMoving;
            this.UpdateVisuals();

            // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
            this.messenger.Publish(new NavigationMessage(activated, deactivated));

            string message =
                string.Format("Forward workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
            this.logger.Info(message);
        }

        return canGoNext;
    }

    private async Task<bool> TryFire(TTrigger trigger, int fadeDuration)
    {
        // TODO 
        await Task.Delay(fadeDuration);
        return true;
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
}