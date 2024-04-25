using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Utils;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tests.Capture;

public class BitmapCaptureServiceTests
{
    private readonly ICaptureSessionAdapter _captureSessionAdapter;

    private readonly BitmapCaptureService _bitmapCaptureService;

    public BitmapCaptureServiceTests()
    {
        _captureSessionAdapter = Substitute.For<ICaptureSessionAdapter>();

        _bitmapCaptureService = new(_captureSessionAdapter, Substitute.For<ILogger<BitmapCaptureService>>());
    }

    [Fact]
    public void StartCapture_WithHandler_InitializesHandlerAndRaiseEvent()
    {
        var canvasDevice = new CanvasDevice();
        _captureSessionAdapter.CanvasDevice.Returns(canvasDevice);
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);
        handler.ClearReceivedCalls();
        var captureItemRect = new Rect(0, 0, 10, 20);
        var captureItem = Substitute.For<IDisplayCaptureItem>();
        captureItem.OuterBounds.Returns(captureItemRect);
        CaptureStartedEventArgs? eventRaisedArgs = null;
        _bitmapCaptureService.Started += (_, e) => eventRaisedArgs = e;

        _bitmapCaptureService.StartCapture(captureItem);

        Assert.True(_bitmapCaptureService.IsStarted);
        Assert.NotNull(eventRaisedArgs);
        Assert.Equal(captureItemRect.ToSizeInt32(), eventRaisedArgs.Size);
        _captureSessionAdapter.Received(1).Start(captureItem);
        handler.Received(1).Initialize(canvasDevice, captureItemRect);
    }

    [Fact]
    public void StartCapture_AlreadyStarted_ThrowsException()
    {
        var captureItem = Substitute.For<IDisplayCaptureItem>();
        _bitmapCaptureService.StartCapture(captureItem);

        Assert.Throws<InvalidOperationException>(() => _bitmapCaptureService.StartCapture(captureItem));
    }

    [Fact]
    public void StopCapture_Stops()
    {
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);
        handler.ClearReceivedCalls();
        var eventRaised = false;
        _bitmapCaptureService.Stopped += (_, _) => eventRaised = true;
        _bitmapCaptureService.StartCapture(Substitute.For<IDisplayCaptureItem>());

        _bitmapCaptureService.StopCapture();

        Assert.False(_bitmapCaptureService.IsStarted);
        Assert.True(eventRaised);
        _captureSessionAdapter.Received(1).Stop();
        handler.Received(1).Stop();
    }

    [Fact]
    public void Dispose_StopsCapture()
    {
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);
        handler.ClearReceivedCalls();
        var eventRaised = false;
        _bitmapCaptureService.Stopped += (_, _) => eventRaised = true;
        _bitmapCaptureService.StartCapture(Substitute.For<IDisplayCaptureItem>());

        _bitmapCaptureService.Dispose();

        Assert.False(_bitmapCaptureService.IsStarted);
        Assert.True(eventRaised);
        _captureSessionAdapter.Received(1).Stop();
        handler.Received(1).Stop();
    }

    [Fact]
    public void RegisterFrameHandler_AlreadyStarted_InitializesHandler()
    {
        _captureSessionAdapter.CanvasDevice.Returns(new CanvasDevice());
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);
        _bitmapCaptureService.StartCapture(Substitute.For<IDisplayCaptureItem>());

        _bitmapCaptureService.UnregisterFrameHandler(handler);

        handler.Received(1).Stop();
    }

    [Fact]
    public void UnregisterFrameHandler_StopsHandler()
    {
        var canvasDevice = new CanvasDevice();
        _captureSessionAdapter.CanvasDevice.Returns(canvasDevice);
        var handler = Substitute.For<IBitmapFrameHandler>();
        var captureItem = Substitute.For<IDisplayCaptureItem>();
        _bitmapCaptureService.StartCapture(captureItem);

        _bitmapCaptureService.RegisterFrameHandler(handler);

        handler.Received(1).Initialize(canvasDevice, new());
    }

    [Fact]
    public void CaptureSessionAdapterFrameArrived_HandlersHandleFrames()
    {
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);
        var bitmap = CreateTestCanvasBitmap(new CanvasDevice());

        _captureSessionAdapter.FrameArrived += Raise.EventWith(new FrameArrivedEventArgs(bitmap));

        handler.Received(1).HandleFrame(bitmap);
    }

    [Fact]
    public void CaptureSessionAdapterRecreated_NullCanvasDevice_DoesNothing()
    {
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);

        _captureSessionAdapter.Recreated += Raise.Event();

        handler.DidNotReceive().Initialize(Arg.Any<CanvasDevice>());
    }

    [Fact]
    public void CaptureSessionAdapterRecreated_WithCanvasDevice_InitializesHandler()
    {
        var canvasDevice = new CanvasDevice();
        _captureSessionAdapter.CanvasDevice.Returns(canvasDevice);
        var handler = Substitute.For<IBitmapFrameHandler>();
        _bitmapCaptureService.RegisterFrameHandler(handler);

        _captureSessionAdapter.Recreated += Raise.Event();

        handler.Received(1).Initialize(canvasDevice);
    }

    private static CanvasBitmap CreateTestCanvasBitmap(ICanvasResourceCreator resourceCreator)
        => CanvasBitmap.CreateFromColors(resourceCreator,
            [Windows.UI.Color.FromArgb(0, 0, 0, 0)],
            1,
            1,
            1);
}