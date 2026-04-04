using HomeAutomation.Application.BatteryData;
using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Inverter;
using Microsoft.Extensions.Logging.Abstractions;

namespace HomeAutomation.Application.Tests.BatteryData;

[TestOf(nameof(FallbackInverterRealtimeDataReader))]
public class FallbackInverterRealtimeDataReaderTests
{
    private static readonly InverterRealtimeData SampleData = new()
    {
        BatteryPercentage = 80,
        BatteryPowerUsage = 100,
        SolarInput = 500,
        FeedIn = 50,
        HomeUsage = 300,
        TimeStamp = DateTime.UtcNow,
        Source = "Test"
    };

    private IInverterRealtimeDataReader _localReader = null!;
    private IInverterRealtimeDataReader _cloudReader = null!;
    private InverterCircuitBreaker _circuitBreaker = null!;
    private FallbackInverterRealtimeDataReader _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _localReader = Substitute.For<IInverterRealtimeDataReader>();
        _cloudReader = Substitute.For<IInverterRealtimeDataReader>();
        _circuitBreaker = new InverterCircuitBreaker(failureThreshold: 3);
        _sut = new FallbackInverterRealtimeDataReader(_localReader, _cloudReader, _circuitBreaker, NullLogger<FallbackInverterRealtimeDataReader>.Instance);
    }

    [Test]
    public async Task GetData_WhenLocalSucceeds_ReturnsLocalData()
    {
        SetupLocalSuccess();

        var result = await _sut.GetInverterRealtimeDataAsync(default);

        Assert.That(result, Is.EqualTo(SampleData));
    }

    [Test]
    public async Task GetData_WhenLocalFails_FallsBackToCloud()
    {
        SetupLocalFailure();
        SetupCloudSuccess();

        var result = await _sut.GetInverterRealtimeDataAsync(default);

        Assert.That(result, Is.EqualTo(SampleData));
    }

    [Test]
    public async Task GetData_WhenBothFail_OpensCircuitAfterThreshold()
    {
        SetupLocalFailure();
        SetupCloudFailure();

        for (var i = 0; i < 3; i++)
            await SafeCall();

        Assert.That(_circuitBreaker.IsOpen, Is.True);
    }

    [Test]
    public async Task GetData_WhenCircuitOpen_ThrowsWithoutCallingReaders()
    {
        SetupLocalFailure();
        SetupCloudFailure();

        for (var i = 0; i < 3; i++)
            await SafeCall();

        _localReader.ClearReceivedCalls();
        _cloudReader.ClearReceivedCalls();

        Assert.Multiple(async () =>
        {
            Assert.That(async () => await _sut.GetInverterRealtimeDataAsync(default), Throws.InvalidOperationException);
            _localReader.DidNotReceiveWithAnyArgs().GetInverterRealtimeDataAsync(default);
            _cloudReader.DidNotReceiveWithAnyArgs().GetInverterRealtimeDataAsync(default);
        });
    }

    [Test]
    public async Task GetData_AfterSuccess_ResetsFailureCount_SoCircuitDoesNotReopenPrematurely()
    {
        // Two failures
        SetupLocalFailure();
        SetupCloudFailure();
        await SafeCall();
        await SafeCall();

        // One success resets the count
        SetupLocalSuccess();
        await _sut.GetInverterRealtimeDataAsync(default);

        // Two further failures should not open the circuit (threshold is 3)
        SetupLocalFailure();
        SetupCloudFailure();
        await SafeCall();
        await SafeCall();

        Assert.That(_circuitBreaker.IsOpen, Is.False);
    }

    private void SetupLocalSuccess() =>
        _localReader.GetInverterRealtimeDataAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(SampleData));

    private void SetupCloudSuccess() =>
        _cloudReader.GetInverterRealtimeDataAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(SampleData));

    private void SetupLocalFailure() =>
        _localReader.GetInverterRealtimeDataAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<InverterRealtimeData>(new HttpRequestException("local unreachable")));

    private void SetupCloudFailure() =>
        _cloudReader.GetInverterRealtimeDataAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<InverterRealtimeData>(new HttpRequestException("cloud unreachable")));

    private async Task SafeCall()
    {
        try { await _sut.GetInverterRealtimeDataAsync(default); }
        catch { /* expected */ }
    }
}


