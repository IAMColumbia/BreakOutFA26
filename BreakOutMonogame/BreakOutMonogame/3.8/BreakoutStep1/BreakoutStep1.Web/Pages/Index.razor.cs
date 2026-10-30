using Microsoft.JSInterop;

namespace BreakoutStep1.Web.Pages;

public partial class Index
{
    private Game1 game;
    private DotNetObjectReference<Index> reference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            reference = DotNetObjectReference.Create(this);
            await JsRuntime.InvokeVoidAsync("breakout.start", reference);
        }
    }

    [JSInvokable]
    public void TickDotNet()
    {
        if (game == null)
        {
            game = new Game1();
            game.Run();
        }
        game.Tick();
    }

    public void Dispose()
    {
        game?.Dispose();
        reference?.Dispose();
    }
}
