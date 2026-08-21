// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Agents.AI.DurableTask.Workflows;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Agents.AI.DurableTask.UnitTests.Workflows;

/// <summary>
/// Tests for durable workflow superstep limit handling.
/// </summary>
public sealed class DurableWorkflowRunnerSuperstepTests
{
    [Theory]
    [InlineData(99, 1)]
    [InlineData(100, 0)]
    public void ThrowIfMaxSuperstepsExceeded_WithoutLimitViolation_DoesNotThrow(int superstep, int remainingExecutors)
    {
        // Arrange
        const string InstanceId = "workflow-instance";

        // Act
        Exception? exception = Record.Exception(() =>
            DurableWorkflowRunner.ThrowIfMaxSuperstepsExceeded(
                InstanceId,
                superstep,
                remainingExecutors,
                NullLogger.Instance));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ThrowIfMaxSuperstepsExceeded_AtLimitWithPendingWork_Throws()
    {
        // Arrange
        const string InstanceId = "workflow-instance";

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            DurableWorkflowRunner.ThrowIfMaxSuperstepsExceeded(
                InstanceId,
                100,
                2,
                NullLogger.Instance));

        // Assert
        Assert.Equal(
            "Workflow 'workflow-instance' exceeded the maximum superstep limit (100) with 2 executor(s) still queued.",
            exception.Message);
    }
}
